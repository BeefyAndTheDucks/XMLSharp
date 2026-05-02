# Syntax
Here I'll be outlining the syntax for our programming language XML#. Abbreviated `xmls`.

When I'm writing code examples, anything UPPERCASE means it is a substitute for actual code, for example `NAME` could be `foo`, `bar`, `myReallyCoolThing`, etc.
Writing in lowercase means it is a literal and cannot change.

## Quick things
- Each statement **MUST** end with a semicolon!!

## Variables
Variables in XML# are identical to other C-like languages. You define a variable by doing `TYPE NAME = VALUE`.
You cannot make uninitialized variables though, but you *can* set it to `null`.

## Booleans
Boolean logic in XML# is a bit different from other C-like languages.
The names of `true` and `false` has changed to `yes` and `no` respectively, 
but the symbols are a bit different, and we only have one symbol instead of 2.

Here are the changes:

#### AND
The and operator still uses the ampersand, but we only use one ampersand. For example:

`false & false` -> `false`

`false & true` -> `false`

`true & false` -> `false`

`true & true` -> `true`

#### OR
The or operator uses a single `?` instead of `||`. For example:

`false ? false` -> `false`

`true ? false` -> `true`

`false ? true` -> `true`

`true ? true` -> `true`

#### XOR
The xor operator uses a single `!`. This might confuse you at first since in other C-like languages it works like `not`.
Here are some examples:

`false ! false` -> `false`

`false ! true` -> `true`

`true ! false` -> `true`

`true ! true` -> `false`

#### NOT
And finally, we have the not operator. Here we use a single `~` sign instead of the `!` you might be used to:

`~false` -> `true`

`~true` -> `false`

## Branching
Branching in XML# is like you're used to from other C-like languages.
For example:
```
if (CONDITION) {
    SOME CODE
} else {
    SOME OTHER CODE
}
```

## Classes and structs
Like other C-like languages, we have classes and structs.

There are some differences between classes and structs:

A class is nullable, a struct is not.

A class passes by reference, a struct is copied.

That said, you define a class as such:
```
class NAME {
    ACCESS TYPE NAME = DEFAULT (optional);
}
```

And a struct like this:
```
struct NAME {
    ACCESS TYPE NAME = DEFAULT;
}
```

Access can either be `public` or `private`. Public means that anybody can access/change the value.
Private means that only methods on the `class` or `struct` can access/change the value.

You can use two access modifiers.

`private public` means `private` access, `public` change.

`public private` means `public` access, `private` change.

Creating instances is really easy:
```
CLASS_OR_STRUCT_NAME VARIABLE_NAME = create(CONSTRUCTOR_ARGUMENTS);
```

## Functions & Methods
This might be the biggest difference between XML# and other C-like languages.

A `function` is a freestanding function not associated with a type. For example, defining:
```
function NAME(PARAM_1_TYPE PARAM_1_NAME, PARAM_2_TYPE PARAM_2_NAME) -> RET_TYPE {
    SOME CODE;
    return SOME VALUE;
}
```
Calling that function is as easy as
```RET_TYPE RESULT = NAME(PARAMETER_1, PARAMETER_2)```

Now the fun part is `method`s. Each method is always associated for a type. For example, this code:
```
class SOME_CLASS {
    SOME FIELD;
    ANOTHER FIELD;
}

ACCESS method SOME_CLASS NAME(PARAM_1_TYPE PARAM_1_NAME, PARAM_2_TYPE PARAM_2_NAME) -> RET_TYPE {
    SOME CODE
    return SOME VALUE;
}
```

You see how it's associated with the class. Access is like with fields in classes, `private` means only other methods can execute this method, `public` means anyone can execute it.

Calling the method is easy:
```
SOME_CLASS VARIABLE_NAME = create(CONSTRUCTOR_ARGUMENTS);
RETURN_TYPE RETURN_VARIABLE_NAME = VARIABLE_NAME.METHOD_NAME(METHOD_PARAMETERS);
```

Function overloading is not possible, give it a separate name. Although, if I get enough begging, I might implement overloading in the future.

Constructors for classes are identical to creating methods, except you omit `NAME` and `RET_TYPE`. For example:

```
class SOME_CLASS {
    SOME FIELD;
    ANOTHER FIELD;
}

ACCESS method SOME_CLASS(PARAM_1_TYPE PARAM_1_NAME, PARAM_2_TYPE PARAM_2_NAME) {
    SOME CODE
}
```

You can use the `this` keyword to refer to fields in the `struct` or `class` if there's another `function` or variable with the same name as the thing you want to access on the `class` or `struct`.

## Built-in datatypes (primitives)

- A `bool` is called `yes_no`
- An `int` is called `number`
- A `float` is called `decimals`
- A `double` is called `many_decimals`

## Maths
Maths is right-to-left. That means 1 + 2 + 3 becomes 1 + (2 + 3).
This is definitely not an oversight in the compiler (it is).