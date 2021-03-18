# Code Styling & Conventions

All code must comply with the Sphere 10 Software code-styling guidelines outlined in this document. Please apply the below styling to your resharper and/or Visual Studio settings when contributing to Sphere 10 Software repositories.

[Resharper Styles](/resources/xxx)

[Visual Studio Styles](/resources/xxx)

## Overview

An overview of the styling is provided here.

### Java-style braces

Braces should begin 

```csharp
public class SomeClass {
    
   public void SomeMethod() {
        if (condition) {
            if (condition1) {
                foo1();
                foo2();
            } else {
                foo3();
                foo4();
            }
        }
    }   
}
```



## Constructors calls

Invocation of base or sibling constructor should always be on the next line

```csharp
public Constructor(int arg1, string arg2) : base(arg1, arg2, "newArgValue")
```

becomes

```csharp
public Constructor(int arg1, string arg2) 
	: base(arg1, arg2, "newArgValue")
```


### Redundant else

```csharp
if (condition) {
    //...
	return true;   // this could also be a throw
} else {
	_includeListHeader = value;
}
```
becomes

```csharp
if (condition) {
    // ...
    return true;   // this could also be a throw
}
_includeListHeader = value;
```


### Wrapping

A line of code should be wrapped on two conditions:

1. It has gone over **170 characters** on the line;

```csharp
xxx
```


2. The line is complex  as denotes a "functional" block of code (i.e. LINQ or many chained mathod calls).  These should be broken up into a logically readble form.

```csharp
xxx
```

The following situations by themselves do  **NOT** warrant wrapping:

3. Lots of arguments

```csharp
        public Method(int arg1,
			string arg2,
			string arg3,
            IList<int> arg4
            .
            .
            .
         )
```

should be

```csharp
        public Method(int arg1, string arg2, string arg3, IList<int> arg4, ...)
```
