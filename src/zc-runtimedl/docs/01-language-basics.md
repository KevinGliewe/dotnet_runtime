# Language Basics

## Program Structure

```zc
fn main() {
    println "Hello, World!";
}
```

Implicit shorthand — a bare string literal prints with newline:

```zc
fn main() {
    "Hello, World!"
}
```

## Variables

```zc
let x = 10;                   // mutable, type inferred
let y: int = 20;              // explicit type
x = 30;                       // reassignment OK

let z: const int = 42;        // read-only variable

def MAX_SIZE = 1024;           // compile-time manifest constant
```

## Primitive Types

| Type | Description |
|------|-------------|
| `int` | 32-bit signed integer |
| `float` / `f32` | 32-bit float |
| `double` / `f64` | 64-bit float |
| `bool` | boolean (`true` / `false`) |
| `char` | 8-bit character |
| `rune` | Unicode code point (32-bit) |
| `byte` / `u8` | unsigned 8-bit |
| `string` / `char*` | null-terminated C string pointer |
| `usize` | pointer-sized unsigned integer |
| `u8` `u16` `u32` `u64` `u128` | unsigned integers |
| `i8` `i16` `i32` `i64` `i128` | signed integers |
| `c_int` `c_long` | C interop types |
| `void*` | opaque pointer |

## Integer Literals

```zc
let dec = 42;
let hex = 0xFF;
let oct = 0o77;
let bin = 0b1010;
let suffixed = 42u64;
let float_lit = 3.14f;
```

## Functions

```zc
fn add(a: int, b: int) -> int {
    return a + b;
}

// Default arguments
fn increment(val: int, amount: int = 1) -> int {
    return val + amount;
}

// Named arguments at call site
increment(val: 10, amount: 5);

// Generic function
fn identity<T>(x: T) -> T {
    return x;
}
let n = identity<int>(42);

// Tuple return
fn divide(a: int, b: int) -> (int, int) {
    return (a / b, a % b);
}
let (quot, rem) = divide(17, 5);
```

## Lambdas & Closures

```zc
// Arrow lambda (single expression)
let doubler = x -> x * 2;
let add3 = (a, b, c) -> a + b + c;

// Block lambda
let square = fn(x: int) -> int { return x * x; };

// Higher-order functions
fn apply_twice(f: fn(int) -> int, x: int) -> int {
    return f(f(x));
}
apply_twice(x -> x * 2, 5);   // 20

// Returning a lambda
fn make_adder(n: int) -> fn(int) -> int {
    return x -> x + n;
}

// Capture by reference
let val = 0;
let inc = [&] () -> val += 1;
```

## String Interpolation & I/O

```zc
let name = "World";
println "Hello, {name}!";          // Hello, World!
println "2 + 3 = {2 + 3}";        // 2 + 3 = 5

// Implicit f-string: any string with {} is interpolated
let msg = "Result: {x + y}";

// I/O keywords
println "with newline";            // stdout + newline
print "no newline"..;              // stdout, no newline (note ..)
"implicit println";                // bare string = println
!"error message";                  // stderr + newline (eprintln)
!"error no newline"..;             // stderr, no newline

// User input
let name: char[256];
? "Enter your name: " (name);     // prompt + read
```

## Imports

```zc
// Standard library
import "std/string.zc"
import "std/vec.zc"

// Relative imports
import "./semver.zc"
import "../utils.zc"

// C header with namespace alias
import "raylib.h" as rl;
rl::InitWindow(800, 600, "Title");

// C header direct
include <stdlib.h>

// Plugin imports
import plugin "regex" as re
```

## Arrays

```zc
let nums: int[5] = [1, 2, 3, 4, 5];
let zeros: [int; 10];                  // zero-initialized

nums[0] = 42;                          // element access
for val in nums { println "{val}"; }   // iteration

// Multi-dimensional
let grid: int[16][16];
```

## Tuples

```zc
let pair = (1, "hello");
pair.0;                                // first element
pair.1;                                // second element

// Destructuring
let (a, b) = pair;
let (x: int, y: string) = (10, "hi"); // typed destructuring
```
