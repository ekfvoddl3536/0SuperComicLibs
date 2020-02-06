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
## 특별한 매개 변수
| 이름 | 설명 |
| :--: | :--- |
| @out | 현재 반환 값을 가져오거나 설정합니다, 반드시 ref 와 함께 사용해야 합니다. |
| @this | 메서드가 instance 형식일 때에만 유효하며, ref 은 선택입니다. |
| `param_`으로 시작 | 특별한 매개 변수 이름을 무시하고 검색합니다. |

## 매개 변수 검색 규칙
`Prefix` 및 `Postfix` 패치는 매개 변수 이름으로 매개 변수를 검색합니다.  
특별한 매개 변수를 제외한, 나머지 매개 변수의 배치된 순서는 고려하지 않습니다.  

이해를 돕기 위한 코드:  
```csharp
// 원래 함수
static int Original(int a, int b) {...}

// 패치 함수
static void PATCH(int b, int a)
{
    // b는 원래 함수의 1번째 인수인 int b 의 값을 가집니다
    // a는 원래 함수의 0번째 인수인 int a 의 값을 가집니다
    
    // 만약 Original(100, 200) 을 호출한다면
    // PATCH(200, 100) 으로 연결됩니다
}

// 오류!
// 패치 함수
static void Invalid_PATCH(int b, ref int @out)
{
    // out 이라는 이름을 가진 매개 변수를 찾을 수 없습니다.
    // @out 은 현재 반환 값을 가져오는 특별한 매개 변수 이름이며
    // 반드시 0번째에 배치되어야 합니다
}

static void Valid_PATCH(ref int @out, int b)
{
    // 이것은 정상적으로 작동할 것입니다!
}
```
<br/>  
  
매개 변수 이름이 `@out`또는 `@this`인 경우:  
```csharp
static void Original(int @this, int @out) {...}

static void PATCH(int param_this, int param_out) {...}
```


## Prefix
원래 함수가 실행되기 전, 실행됩니다.  
반환 형식은 반드시 `void` 또는 `bool` 이어야하며, 인수의 경우 원래 함수의 인수 목록에서 이름으로 검색합니다.  

모든 매개 변수에 `ref` 를 사용할 수 있습니다.  

### Prefix bool 반환에 대해
만약 `false`를 반환한다면, 원래 함수의 실행을 건너뜁니다.  
단, 가장 마지막으로 호출된 Prefix 패치가 `false`를 반환하지 않으면 원래 함수는 실행됩니다.  


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

여러개의 `Replace` 패치 함수는 지원되지 않습니다.  
가장 마지막으로 지정된 `Replace` 패치 함수를 사용합니다.  

  
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
매개 변수의 이름은 자유이며, 타입만 일치하면 됩니다.  
```csharp
void (ILGenerator, MethodBase, int, bool)
void (ILGenerator, MethodInfo, int, bool)
void (ILGenerator, ConstructorInfo, int, bool)

// prefix 에서만 유효
bool (ILGenerator, MethodBase, int, bool)
bool (ILGenerator, MethodInfo, int, bool)
bool (ILGenerator, ConstructorInfo, int, bool)
```

### 표현
```csharp
// Prefix OR Postfix OR Replace
static void ABCD(ILGenerator il, MethodBase methodinfo_or_constructorinfo, int argFixupOffset, bool hasReturn)
{
    ...
    // 절대로 ret을 쓰지 마십시오.
    // 다른 패치가 실행되지 않으며, 결과에 오류가 발생하여 패치에 실패할 수도 있습니다.
    // il.Emit(OpCodes.Ret);
}
```

### argFixupOffset 매개 변수에 대해
3번째 매개 변수이며, 원래 함수의 시그니처에서 첫번째 매개 변수가 시작되는 `offset`을 보정하는 용도입니다.  
예를 들어, `instance 메소드`이고 반환이 `bool`인 경우 `offset`은 `1`입니다.  

이 값은 항상, `0`또는 `1`, `2`의 값만 가집니다.  

이것은 다음과 같이 사용될 수 있습니다.
```csharp
static void MY_PRE(...)
{
    // load first parameter
    il.Emit(OpCodes.Ldarg, argFixupOffset);
}
```



# 로딩 순서와 실행 순서
## 속성 우선순위
기본적으로 속성을 겹쳐서 사용하는것은 권장하지 않으며, 가장 먼저 확인된 `Attribute`만 적용됩니다.  

| 속성 이름 | 순위 |
| :---: | :---: |
| Prepare | 1 |
| TargetMethod | 2 |
| RegisterPatchMethod | 3 |
| PatchFinalize | 4 |

```csharp
[Prepare]
[PatchFinalize]
static void NAME()
{
    // Prepare 만 유효한 것으로 평가되며, PatchFinalize 는 무시됩니다
}
```



## 실행 흐름의 약식 표현
실제로 이런 흐름을 갖지는 않습니다.  
단순히 이해를 돕기 위한 예시입니다.  


```csharp
public static METHOD_0000(...,) {
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
`7SuperComicLib.Reflection.Patch`는 `ref 반환`메서드에 대한 Patch를 지원합니다.  

## Prefix & Postfix
`ref T` 는 `T&`로 재해석될 수 있고, `T&`는 `void*`로 재해석될 수 있습니다.  
또, `void*`는 주소를 담고 있으므로, `T&`를 주소를 표현하는 `native int`로도 바꿀 수 있습니다.  
그리고, `native int`는 `IntPtr`를 의미합니다.  

따라서, 다음과 같이 Prefix & Postfix 메서드를 정의해야 합니다.  
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
이러한 방법으로 `ref 반환`을 강제로 수정하는 것은 굉장히 위험한 작업입니다.  
가급적 `Prefix` & `Postfix`패치하는 것을 자제해야합니다.  

`fixed (...)`는 메소드가 끝나면, 고정을 해제하기 때문에 안전하지 않습니다.  
대신, `GCHandle` 과 함께 `Pinned 모드`로 사용하는 것이 더 안전합니다.  

## Replace 
`ref 반환`이 있을때 가장 간단한 방법을 제공합니다.  
원래 함수와 동일한 매개 변수 형식을 정의하고, 반환을 정의하면 됩니다.  
`Prefix` & `Postfix` 패치와 비교해, 매우 안전합니다.  

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

