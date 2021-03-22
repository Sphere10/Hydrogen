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



### Naming Conventions

The standard C# naming convention is employed at all times as suggested by Visual Studio IDE and/or Resharper. These include
1. PascalCase naming covention for field, argument, variable members.
2. Capitalized for all public, protected and internal members.

And most importantly
3. Members are named using "self-describing" names so that the code is a "living documentation". This means **generally** avoiding short, cryptic and abbreviated names.

### Constructors calls

Invocation of base or sibling constructor should always be on the next line

```csharp
public Constructor(int arg1, string arg2) : base(arg1, arg2, "newArgValue")
```

becomes

```csharp
public Constructor(int arg1, string arg2) 
	: base(arg1, arg2, "newArgValue")
```

### Avoid Unnecessary Braces

Single-line scopes should avoid using begin/end braces.

```csharp
if (condition) {
	foreach (TItem item in items) {
		Serializer.Serialize(item, Writer);
	}
}	
```

becomes

```csharp
if (condition) 
	foreach (TItem item in items) 
		Serializer.Serialize(item, Writer);
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

A line of code should not be wrapped unless doing so **significantly** improves readability. As a guide, the following rules should be employed:

1. It has gone over **170 characters** on the line;

2. The line is complex and invokes functions/constructors/property setters, etc. These scenarios are typically when constructing an object graph or using a LINQ query. 


   The following situations by themselves do  **NOT** warrant wrapping:

3. Many arguments

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

**unless** the argument count was gratuitously long then it can be 1-arg per line.


### Comments

Developers should add code-comments inside their code so as to convey meaning of what is being done. XMLDOC code comments need not be done during development but can be retrofitted later using tooling.

