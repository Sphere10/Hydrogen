# Code Styling Guidelines

Sphere 10 Software employs it's own code-styling approach which all contributions must adhere to. This document provides styling guidelines for common situations. To help contributors, Visual Studio and Resharper code-styling definitions are also provided and which can be adopted quickly by simply copying the `.editorconfig` file to the solution folder. However, the guidelines in this document go beyond what can be auto-formatted and contributors are required to read, understand and apply these guidelines. 

[Visual Studio & Resharper Editor Config)](resources/.editorconfig)

**NOTE:** This document is still evolving and contributors should refresh themselves periodically.

**NOTE:** If a styling element / aspect is not addressed by this document (or editor config) then the contributor has artistic license by default.

### Braces at end-of-line

Braces should begin at end of line and finish at beginning of line (with corresponding tab offsets)

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

The naming conventions should follow the default C# conventions as suggested by Visual Studio IDE and/or Resharper intellisense. These include
1. Pascal-cased naming convention for all field, argument, variable members.
2. Capitalized for all public, protected and internal members.

And most importantly
3. Members are named using "self-describing" names so that the code is a "living documentation". This means **generally** avoiding short, cryptic and abbreviated names.

**Example:**

```csharp

public class PascalCasedClassName { 
    private int _privateFieldName;          // private fields start with underscore
    protected int NonPrivateFieldName;      // non-private fields start with capitals

    public PascalCasedClassname() { }       // Pascal-cased constructor
    
    public int PrivateMethodName() { }      // Pascal-cased private methods
    
    public int PublicMethodName() { }       // Pascal-cased public methods
}

```



### Member Ordering

Class members should be ordered according to the following guide

```csharp

public class SomeClass {
    
    // 1. Event declarations
    
    // 2. Private field declarations
    
    // 3. Protected field declarations 
    
    // 4. Constructors (basic constructors first, complex constructors last)
    
    // 4b. Finalizers, if any
    
    // 5. Public properties
    
    // 6. Internal/protected/private properties
    
    // 7. Public methods
    
    // 8. Internal/protected/private methods
    
    // 9. Inner class/type declarations
    
}

```



### Namespaces

Namespaces are **NOT** necessarily organized by the strict `ProjectName.folder.subfolder.subsubfolder` convention suggested by Visual Studio and Resharper.  Care must be taken by the developer to ensure the namespaces are logically thought out and not overly granular. Expecting framework consumers to know the namespaces can be unrealistic and the preference is to employ more granular namespaces.

For example, the `Sphere10.Framework` module contains a myriad of system-tier domains which are all encumbered within a single `Sphere10.Framework` namespace (this may change in future).

For system-tier modules that is acceptable, however for other tiers the namespaces should be decomposed into a logical structuring that matches the `CompanyName.ProductName.Tier.Domain` pattern. If a domain is decomposed into sub-folders then it does **NOT** automatically mean the namespace should also be decomposed that way. It's up to developers discretion. Please refer to Sphere 10's [3-tier architecture guidelines](3-tier-Architecture.md) for understanding how tiers, domains and modules are structured according to the Sphere 10 Software Engineering methodology.

**Example**

```
Sphere10.Framework
Sphere10.Framework.Application
Sphere10.Hydrogen.Core
Sphere10.Hydrogen.Core.Kademlia
Sphere10.Hydrogen.Presentation.Host
Sphere10.Hydrogen.Presentation.Node
Sphere10.Hydrogen.Presentation.UI
```



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



### Use `var` 

Explicit type declarations should be avoided where possible in preference of `var`.

**Example**:

```csharp
IEnumerable<ISomeInterface<string>> sequence = GetSequence();
foreach(ISomeInterface<string> item in sequence) {
    //...
}
```

becomes

```csharp
var sequence = GetSequence();
foreach(var item in sequence) {
    //...
}
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

Redundant else's should always be removed.

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



### Parenthesis Wrapping

When wrapping a line after an open-parenthesis `(` ensure the close-parenthesis `)` is placed on new line at the indentation of the line which originated the `(` (i.e. as if it were begin/end braces `{` `}`). 

**Example:**

```csharp
if (condition) {
	SomeComplexMethod(
		SomeArgument1,
        SomeOtherMethod(
			SomeArgument2, 
			SomeArgument3));            
}
```

becomes

```csharp
if (condition) {
	SomeComplexMethod(
		SomeArgument1,
        SomeOtherMethod(
			SomeArgument2, 
			SomeArgument3
		)
	);
}
```



### Indentation & Horizonal Spaces

Indentation for code should be tab-based with 4-character length whereas indentation for comments should be space-basee **not** tab-based.

**Example**

```csharp
[Flags]
public enum SomeEnum {
	SomeVal1      = 1 << 0,      // Line starts with tab but = and comments are spaced
	SomeVal2      = 1 << 1,      // comment
}	
```



### Code Comments & Vertical Spaces

Developers should add code-comments inside their code so as to assist auditors on what the purpose of the code is. XMLDOC code comments need not be done during development but can be retrofitted later using tooling. Spaces between lines of code should **only** be used to group the "logical segments" of the code block. All code within a "logical segment" should have no spaces. Each logical segment should have a comment. 

**Example:**

```csharp
public void Foo() {
  var x = 10;
    
  var y = GetSomeValue(x);
  
  System.Console.WriteLine(y);
    
  var obj = new SomeObject {
      Prop1 = x + y,
      Prop2 = 11
  }
    
  var z = CallMethod(obj);
    
  return z;
}
```

should be

```csharp
public void Foo() {
  // Comment for logical segment 1 of code block
  var x = 10;
  var y = GetSomeValue(x);
  System.Console.WriteLine(y);

  // Comment for logical segment 2of code block
  var obj = new SomeObject {
      Prop1 = x + y,
      Prop2 = 11
  }
  var z = CallMethod(obj);
    
  return z;           // note: the space before return denotes it is separate from both segments
}
```

This rule also applies to class declarations, methods, etc. Only 1 single line of space between

```csharp
namespace SomeNamespace {
                                   // Notice line (OPTIONAL)
    public class SomeClass() {
                                   // Notice line   
        public Method1() { 
            /// ...
        }
                                   // Notice line
        public Method2() { 
            /// ...
        }
                                   // Notice line  
    }
                                   // Notice line (OPTIONAL)                     
}

```



