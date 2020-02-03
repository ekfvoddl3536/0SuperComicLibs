## 7SuperComicLib.Reflection.Patch
### 목차
1. 메소드 패치에 대해  
    1. Prefix
    1. Postfix
    1. Replace
    1. ILOnly
1. 로딩 순서와 실행 순서  
    1. 속성 우선순위
    1. 실행 흐름의 약식 표현
1. ref 반환 메소드를 패치하는 법  
    * Prefix & Postfix
    * Replace  
    
    
    
    
# 메소드 패치에 대해
## Prefix
원래 함수가 실행되기 전, 실행됩니다.  
반환 형식은 반드시 void 또는 bool 이어야하며, 인수의 경우 원래 함수의 인수 목록에서 이름으로 검색합니다.  

모든 매개 변수에 ref 를 사용할 수 있습니다.  

### Prefix bool 반환에 대해
만약 `false`를 반환한다면, 원래 함수의 실행을 건너뜁니다.  
단, 가장 마지막으로 호출된 Prefix 패치가 `false`를 반환하지 않으면 원래 함수는 실행됩니다.  
  
### 특별한 매개 변수
| 이름 | 설명 |
| :--: | :--- |
| @out | 현재 반환 값을 가져오거나 설정합니다, 반드시 ref 와 함께 사용해야 합니다. |
| @this | 메서드가 instance 형식일 때에만 유효하며, ref 은 선택입니다. |

### 올바른 예
````csharp
// 원래 함수
class MyClass1
{
    public int myField;
    
    int Original(int a, int b)
    {
        return a + b;
    }
}

// 패치 함수
...
static bool MyPatch_0(ref int @out, MyClass1 @this, int a, int b)
{
    @out = a - b + @this.myField;
    return false;
}

// 패치 함수
...
static void MyPatch_1(int b)
{
    Console.WriteLine(b.ToString());
}


static void MyPatch_1(ref int a)
{
    a = 7777;
    Console.WriteLine(a.ToString());
}
````

## Postfix
원래 함수가 실행된 이후, 실행됩니다.  

반환은 bool도 허용되지만, 기본적으로 반환 값을 계산하지 않기 때문에 void 반환이 더 유리합니다.    
매개 변수는 Prefix 와 같은 규칙을 따릅니다.  

### 표현
```csharp
static void MyPatch(ref int @out, int a, ref int b)
{
    @out = a + b;
    b = a - 20;
}
```

## Replace
원래 함수를 대체합니다.  
이 패치모드는 모든 Postfix 실행결과에 영향을 줍니다.  
  
**원래 함수의 반환 및 인수 형식과 완전히 일치해야만 정상적으로 호출됩니다.**  
**원래 함수가 static이 아니면, 반드시 @this가 필요합니다.**  

### 올바른 예
````csharp
// 원래 함수
class MyClass1
{
    int Original(int a, int b)
    {
        return a + b;
    }
}

// 패치 함수
...
static int MyPatch(MyClass1 @this, int a, int b)
{
    return a % b;
}
````


## ILOnly
**이 방법은 Prefix, Postfix, Replace와 함께 사용되어야 합니다.**  

패치 함수는 다음중 하나와 같은 시그니처를 사용해야합니다.  
```csharp
void (ILGenerator, **MethodBase**, int, bool, bool)
void (ILGenerator, MethodInfo, int, bool, bool)
void (ILGenerator, ConstructorInfo, int, bool, bool)

// prefix 에서만 유효
bool (ILGenerator, **MethodBase**, int, bool, bool)
bool (ILGenerator, MethodInfo, int, bool, bool)
bool (ILGenerator, ConstructorInfo, int, bool, bool)
```

### 표현
```csharp
// Prefix OR Postfix OR Replace
static bool ABCD(ILGenerator il, MethodBase methodinfo_or_constructorinfo, int argFixupOffset, bool hasReturn, bool hasReturnBuffer)
{
    ...
    // 절대로 ret을 쓰지 마십시오.
    // 다른 패치가 실행되지 않습니다.
    // il.Emit(OpCodes.Ret);
}
```








# 로딩 순서와 실행 순서
## 속성 우선순위
기본적으로 속성을 겹쳐서 사용하는것은 권장하지 않습니다.  

| 속성 이름 | 순위 |
| :---: | :---: |
| Prepare | 1 |
| TargetMethod | 2 |
| RegisterPatchMethod | 3 |
| PatchFinalize | 4 |



## 실행 흐름의 약식 표현
```csharp
public static ...(...,) {
    // call prefixes
    T result = default;
    bool runOriginalMethod = prefixes.Do(...,);
    
    // call original
    if (runOriginalMethod)
        result = original_method.Do(...,);
    
    // call postfixes
    postfixes.Do(...,);
}
```





# ref 반환 메소드를 패치하는 법  
## Prefix & Postfix
`ref T` 는 `T&`로 재해석될 수 있고, `T&`는 `void*`로 재해석될 수 있습니다.  
또, `void*`는 주소를 담고 있으므로, `T&`를 주소를 표현하는 `native int`로도 바꿀 수 있습니다.  
그리고, `native int`는 `IntPtr`를 의미합니다.  

따라서 다음과 같이 Prefix & Postfix 메서드를 정의해야 합니다.  
```csharp
class MyClass10
{
    private int myField;
    
    public ref int Ref() => ref myField;
}

static void PRE_OR_POST(ref IntPtr @out, ...)
{
    ...
}
```

**경고**  
ref 반환을 수정하는 것은 굉장히 위험한 작업입니다.  
따라서, 가급적 패치하는 것을 자제해야합니다.  

fixed (...)는 메소드가 끝나면, 고정을 해제하기 때문에 안전하지 않습니다.  
대신, GCHandle 과 함께 Pinned 모드로 사용하는 것이 더 안전합니다.

## Replace 
ref 반환이 있을때 가장 간단한 방법을 제공합니다.  
원래 함수와 동일한 매개 변수 형식을 정의하고, 반환을 정의하면 됩니다.  

다음과 같습니다.  
```csharp
class MyClass10
{
    private int myField;
    
    public ref int Ref() => ref myField;
}

static ref int REPLACE_(MyClass10 @this)
{
    ...
}
```

