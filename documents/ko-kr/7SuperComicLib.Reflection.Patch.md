## 7SuperComicLib.Reflection.Patch
### 목차
1. 메소드 패치에 대해  
    1. Prefix
    1. Postfix
    1. Replace
    1. ILOnly
1. 흐름의 약식 표현  
1. Generic 메소드를 패치하는 법  
    * Prefix  
    * Postfix  
    * Replace  
1. ref 반환 메소드를 패치하는 법  
    * Prefix  
    * Postfix  
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

### 올바른 예
```csharp
// 추가로 Prefix 예제를 참고 하세요

// 원래 함수
static int Original(int a, int b)
{
    return a + b - 200;
}

// 패치 함수
static void MyPatch(ref int @out, ref int a, ref int b)
{
    @out = a + b;
    a = b;
    b = 10;
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
    public int myField;
    
    int Original(int a, int b)
    {
        return a + b;
    }
}

// 패치 함수
...
static int MyPatch_0(MyClass1 @this, int a, int b)
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

### 올바른 예  
```csharp
class MyClass1
{
    static int Original(int a, int b)
    {
        return a + b;
    }
}

// Prefix OR Postfix OR 
static bool ABCD(ILGenerator il, MethodBase methodinfo_or_constructorinfo, int argFixupOffset, bool hasReturn, bool hasReturnBuffer)
{
    ...
    // 절대로 ret을 쓰지 마십시오.
    // 다른 패치가 실행되지 않습니다.
    // il.Emit(OpCodes.Ret);
}
```
