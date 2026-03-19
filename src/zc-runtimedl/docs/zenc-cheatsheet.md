# Zen-C Language & Standard Library Cheatsheet

> Comprehensive reference for Zen-C syntax and standard library APIs.
> Sources: [awesome-zenc](https://github.com/zenc-lang/awesome-zenc), [zenc test suite](https://github.com/zenc-lang/zenc/tree/main/tests), [zenc-std source](https://github.com/z-libs/Zen-C/tree/main/std).

## Table of Contents

1. [Language Basics](#language-basics) — Variables, types, functions, lambdas, I/O, imports, arrays
2. [Control Flow](#control-flow) — If/else, match, guard, loops, break/continue
3. [Structs, Enums & Traits](#structs-enums--traits) — OOP, operator overloading, generics, iterators
4. [Memory & C Interop](#memory-management--c-interop) — Pointers, raw blocks, defer, extern, embed
5. [Collections](#standard-library-collections) — Vec, String, Map, Set, Stack, Queue, Slice
6. [Error Handling & Testing](#standard-library-error-handling--testing) — Option, Result, test blocks, assert
7. [I/O, Files & Environment](#standard-library-io-files-paths-environment--process) — File, Path, Env, Command
8. [Concurrency & Networking](#standard-library-concurrency--networking) — Thread, Mutex, TCP, HTTP, WebSocket
9. [JSON, Regex, Math & More](#standard-library-json-regex-time-math--more) — JSON, Regex, Time, SIMD, Crypto

---


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

---

# Control Flow

## If / Else

```zc
if x > 5 {
    println "big";
} else if x > 0 {
    println "small";
} else {
    println "non-positive";
}

// If-expression (returns value)
let max = if (a > b) { a } else { b };

// Unless (negated if)
unless x > 0 {
    return -1;
}
```

## Match (Pattern Matching)

```zc
// Basic match
match n {
    0 => { return "zero"; },
    1 => { return "one"; },
    _ => { return "other"; }
}

// OR patterns
match n {
    1 || 2 => { println "one or two"; },
    3 or 4 => { println "three or four"; },
    _      => { println "other"; }
}

// Range patterns
match n {
    0..<5   => { println "0-4"; },          // exclusive
    5..=10  => { println "5-10"; },         // inclusive
    _       => { println "other"; }
}

// Enum destructuring
enum Shape { Circle(float), Rect(float, float) }
match shape {
    Shape::Circle(r)  => { println "radius={r}"; },
    Shape::Rect(w, h) => { println "w={w} h={h}"; },
    _ => {}
}

// Match on Result
match divide(10, 0) {
    Ok(val) => println "Result: {val}",
    Err(e)  => println "Error: {e}"
}

// Ref binding (allows mutation)
match result {
    Ok(ref w) => { w.value = 44; },
    Err(e)    => {}
}
```

## Guard

```zc
guard ptr != NULL else {
    return -1;
}
// ptr is guaranteed non-null here
```

## Ternary

```zc
let sign = (x >= 0) ? "positive" : "negative";
```

## Loops

### For (C-style)

```zc
for (let i = 0; i < 10; i++) {
    println "{i}";
}
// Parentheses optional:
for let i = 0; i < 10; i++ {
    println "{i}";
}
```

### For-in (Ranges)

```zc
for i in 0..5     { }    // 0, 1, 2, 3, 4    (exclusive end)
for i in 0..<5    { }    // same as above
for i in 0..=5    { }    // 0, 1, 2, 3, 4, 5  (inclusive end)
for i in 0..10 step 2 { }  // 0, 2, 4, 6, 8
for i in 10..=0 step -1 { }  // countdown
```

### For-in (Collections)

```zc
let arr: int[3] = [10, 20, 30];
for val in arr { println "{val}"; }

// With index
for i, val in arr { println "{i}: {val}"; }

// Vec iteration
for item in vec { }

// By reference (allows mutation)
for ptr in &vec { (*ptr).x += 1; }
for ptr in vec.iter_ref() { }   // explicit ref iter

// Map iteration
for entry in map {
    println "{entry.key}: {entry.val}";
}

// String iteration (UTF-8 runes)
for c in str { println "{c}"; }
```

### While / Do-While / Loop

```zc
while x > 0 {
    x = x / 2;
}

do {
    x++;
} while x < 10;

loop {
    if done { break; }
}
```

### Repeat

```zc
repeat 5 { println "hello"; }
```

### Break, Continue, Labels

```zc
for i in 0..100 {
    if i % 2 == 0 { continue; }
    if i > 50 { break; }
}

// Labeled break
outer: loop {
    for i in 0..10 {
        if i == 5 { break outer; }
    }
}
```

---

# Structs, Enums & Traits

## Structs

```zc
struct Point {
    x: int;
    y: int;
}

let p = Point { x: 10, y: 20 };
println "({p.x}, {p.y})";
```

### Impl Blocks

```zc
struct Vec2 {
    x: float;
    y: float;
}

impl Vec2 {
    // Static constructor
    fn new(x: float, y: float) -> Vec2 {
        return Vec2 { x: x, y: y };
    }

    // Instance method (self passed by reference)
    fn length_squared(self) -> float {
        return self.x * self.x + self.y * self.y;
    }

    // Self shorthand: .field instead of self.field
    fn scale(self, factor: float) {
        .x = .x * factor;
        .y = .y * factor;
    }

    // to_string enables {} interpolation
    fn to_string(self) -> char* {
        return format("(%f, %f)", self.x, self.y);
    }
}

let v = Vec2::new(3.0, 4.0);    // static call
v.scale(2.0);                    // instance call
println "v = {v}";               // uses to_string
```

### Operator Overloading

| Operator | Method | Example |
|----------|--------|---------|
| `+` | `add(self, other) -> T` | `a + b` |
| `-` | `sub(self, other) -> T` | `a - b` |
| `*` | `mul(self, other) -> T` | `a * b` |
| `/` | `div(self, other) -> T` | `a / b` |
| `+=` | `add_assign(self, other)` | `a += b` |
| `==` | `eq(self, other) -> bool` | `a == b` |
| `!=` | `neq(self, other) -> bool` | `a != b` |
| `<` | `lt(self, other) -> bool` | `a < b` |
| `>` | `gt(self, other) -> bool` | `a > b` |
| `<=` | `le(self, other) -> bool` | `a <= b` |
| `>=` | `ge(self, other) -> bool` | `a >= b` |
| `!` | `not(self) -> T` | `!a` |
| `&` | `bitand(self, other) -> T` | `a & b` |
| `\|` | `bitor(self, other) -> T` | `a \| b` |
| `^` | `bitxor(self, other) -> T` | `a ^ b` |
| `<<` | `shl(self, item)` | `v << 42` |
| `>>` | `shr(self, out)` | `v >> &x` |
| `[]` | `index(self, idx) -> T` | `v[0]` |
| `{}` | `to_string(self) -> char*` | `"{v}"` |

### Derive Macros

```zc
@derive(Eq)           // generates eq/neq
@derive(Clone)        // generates clone
@derive(Debug)        // generates debug print
@derive(Eq, Clone)    // multiple derives

@derive(Eq)
struct Point { x: int; y: int; }
```

### Opaque Structs

```zc
opaque struct User {
    id: int;        // fields private outside module
    name: string;
}
```

### Mixin Composition

```zc
struct Base { value: int; }
impl Base {
    fn get_value(self) -> int { return self.value; }
}

// Anonymous mixin: fields flattened into parent
struct Widget {
    use Base;           // Widget gets .value and .get_value()
    label: string;
}

// Named mixin: accessed via field name
struct Container {
    use inner: Base;    // access via container.inner.value
    size: int;
}
```

## Enums

```zc
// Simple enum
@derive(Eq)
enum Color { Red, Green, Blue }

// Enum with payloads
enum Shape {
    Circle(float),
    Rect(float, float),
    None
}

let s = Shape::Circle(5.0);

// Pattern matching
match s {
    Shape::Circle(r)  => println "circle r={r}",
    Shape::Rect(w, h) => println "rect {w}x{h}",
    Shape::None       => println "none"
}
```

## Traits

```zc
trait Drawable {
    fn draw(self);
    fn area(self) -> float;
}

struct Circle { radius: float; }

impl Drawable for Circle {
    fn draw(self) { println "drawing circle r={self.radius}"; }
    fn area(self) -> float { return 3.14159 * self.radius * self.radius; }
}

// Trait as parameter (dynamic dispatch)
fn render(shape: Drawable) {
    shape.draw();
    println "area = {shape.area()}";
}

let c = Circle { radius: 5.0 };
render(&c);    // auto-converts to trait object
```

## Generics

```zc
struct Wrapper<T> {
    item: T;
    id: int;
}

impl Wrapper<T> {
    fn new(item: T, id: int) -> Self {
        return Self { item: item, id: id };
    }
    fn get(self) -> T { return self.item; }
}

let w = Wrapper<int>::new(42, 1);
```

## Type Aliases

```zc
alias ID = int;
alias Callback = fn(int) -> bool;
opaque alias Handle = int;     // hides underlying type
```

## Custom Iterator Protocol

Any type with `iterator()` returning an object with `next() -> Option<T>` works with `for-in`:

```zc
struct Range { start: int; end: int; }
struct RangeIter { current: int; stop: int; }

impl RangeIter {
    fn next(self) -> Option<int> {
        if self.current < self.stop {
            let v = self.current;
            self.current += 1;
            return Option<int>::Some(v);
        }
        return Option<int>::None();
    }
}

impl Range {
    fn iterator(self) -> RangeIter {
        return RangeIter { current: self.start, stop: self.end };
    }
}

let r = Range { start: 0, end: 5 };
for i in r { println "{i}"; }   // 0, 1, 2, 3, 4
```

---

# Memory Management & C Interop

## Pointers

```zc
let x = 42;
let ptr: int* = &x;         // address-of
*ptr = 100;                  // dereference

// Struct pointer auto-deref (no -> needed)
let p = Point { x: 1, y: 2 };
let pp = &p;
pp.x;                        // auto-deref, same as (*pp).x

// Null check
let p: int* = 0;
if ((usize)p == 0) { println "null"; }
```

## Heap Allocation

```zc
// C-style
let p: int* = malloc(sizeof(int));
*p = 42;
free(p);

// Zen-C typed allocators (from std/mem.zc)
let p = alloc<int>();           // single item
let p = zalloc<int>();          // zero-initialized
let arr = alloc_n<int>(100);    // array of 100

// Utilities
mem_zero<int>(ptr, count);
mem_copy<int>(dst, src, count);
swap<int>(&a, &b);
```

## Box\<T\> (Smart Pointer)

```zc
import "std/mem.zc"

let b = Box<int>::new();         // heap-allocated, zero-init
let val = *b.get();              // get underlying pointer
b.is_null();                     // check
// auto-freed when b goes out of scope (Drop trait)
```

## Arena Allocator

```zc
import "std/arena.zc"

let arena = Arena::new(4096);
let arr: int* = arena.alloc_n<int>(10);
let s = arena.dup_str("hello");

let mark = arena.save();        // savepoint
// ... allocate more ...
arena.restore(mark);            // roll back
arena.reset();                  // reset all
arena.free();                   // or let Drop handle it
```

## Drop Trait (RAII)

```zc
impl Drop for MyStruct {
    fn drop(self) {
        self.free();
    }
}
// drop() called automatically at scope exit
```

## Defer

```zc
fn process() {
    let f = File::open("data.txt", "r").unwrap();
    defer { f.close(); }         // runs at scope exit

    // Multiple defers run in LIFO order
    defer { println "second"; }
    defer { println "first"; }
    // prints: first, then second
}

// Defers run on break, continue, and return
for i in 0..10 {
    defer { cleanup(); }
    if i == 5 { break; }        // defer still runs
}
```

## Autofree

```zc
{
    autofree let buf = malloc(1024);
    // buf automatically freed when scope exits
}
```

## Raw Blocks (C Interop)

```zc
// Embed raw C code inline
raw {
    #include <sys/utsname.h>
    struct utsname un;
    uname(&un);
    printf("OS: %s\n", un.sysname);
}

// Common pattern: C calls wrapped in raw, Zen-C logic outside
fn detect_platform() -> char* {
    raw {
        struct utsname un;
        if (uname(&un) == 0) {
            if (strstr(un.sysname, "Linux")) return "linux";
            if (strstr(un.sysname, "Darwin")) return "macos";
        }
    }
    return "unknown";
}
```

## Extern Functions

```zc
extern fn strlen(s: const char*) -> usize;
extern fn strcasecmp(s1: const char*, s2: const char*) -> int;
extern fn exit(code: int);
```

## C Header Imports

```zc
// Direct include
include <stdlib.h>
include <string.h>

// Namespaced import
import "raylib.h" as rl;
rl::InitWindow(800, 600, "Game");

// Aliased C lib import
import "stdlib.h" as c_stdlib;
c_stdlib::srand(seed);
```

## Compile-Time File Embedding

```zc
let texture_data = embed "assets/dirt.png";
// texture_data.data -> char* (raw bytes)
// texture_data.len  -> int (byte count)
```

## Comptime

```zc
// Compile-time function
@comptime
fn double_ct(x: int) -> int { return x * 2; }

// Compile-time code generation
comptime {
    printf("fn generated() { println \"generated!\"; }\n");
}
```

## Attributes

| Attribute | Description |
|-----------|-------------|
| `@inline` | Inline hint |
| `@export` | Export symbol |
| `@comptime` | Compile-time evaluation |
| `@derive(Eq, Clone)` | Auto-derive traits |
| `@cfg(apple)` | Conditional compilation |
| `@cfg(not(__TINYC__))` | Negative condition |
| `@vector(N)` | SIMD vector type |

## Compiler Directives (in comments)

```zc
//> link: -lraylib -lm
//> include: ./raylib/include
//> libs: ./raylib
```

## Gotchas from Real Usage

- `raw { return; }` works for void functions, but `return` alone in Zen-C may cause parse errors in some contexts
- `continue` works natively in Zen-C `for` loops — no need for `raw { continue; }`
- Access `String` internal buffer via `.vec.data` (it's a `Vec<char>`)
- `Vec.get(idx)` returns the value directly (panics on OOB), NOT an Option
- Forward declarations in raw blocks may be needed when Zen-C functions are called before their definition in transpiled output

---

# Standard Library: Collections

## Vec\<T\> — Dynamic Array

```zc
import "std/vec.zc"
```

### Construction

```zc
let v = Vec<int>::new();
let v = Vec<int>::with_capacity(100);
```

### Adding & Removing

```zc
v.push(42);                    // append
v.pop();                       // remove last (panics if empty)
v.pop_opt();                   // -> Option<T> (safe)
v.insert(0, 99);               // insert at index
v.remove(2);                   // remove at index, returns value
v.append(other_vec);           // append all from other (consumes it)
v.clear();                     // remove all elements
```

### Access

```zc
v.get(0);                     // value copy (panics if OOB)
v.get_ref(0);                 // pointer to element (panics if OOB)
v.set(0, 99);                 // set value at index
v.first();                    // first element (panics if empty)
v.last();                     // last element (panics if empty)
v[0];                         // bracket access (via index())
```

### Query

```zc
v.length();                   // -> usize
v.is_empty();                 // -> bool
v.contains(42);               // -> bool (uses memcmp)
v.eq(&other);                 // structural equality
```

### Transform

```zc
v.reverse();
let v2 = v.clone();
```

### Operators

```zc
let v3 = v1 + &v2;            // concatenate -> new Vec
v1 += &v2;                    // append in place
v << 42;                      // push
v >> &out;                    // pop into variable
let v4 = v * 3;               // repeat contents 3 times
v *= 2;                       // repeat in place
v1 == &v2;                    // equality
v1 != &v2;                    // inequality
```

### Iteration

```zc
for item in v { }              // value copies
for ptr in &v { }              // reference iteration
for ptr in v.iter_ref() { }   // explicit ref iter (VecIterRef)
```

### Memory

```zc
v.free();                     // manual free
v.forget();                   // prevent Drop from freeing (transfer ownership)
// Vec implements Drop — auto-freed at scope exit
```

---

## String — Heap-Allocated UTF-8

```zc
import "std/string.zc"
```

### Construction

```zc
let s = String::new("hello");
let s = String::from("hello");       // alias for new
let s = String::from_rune('A');      // from single rune
let s = String::from_runes(ptr, n);  // from rune array
```

### Properties

```zc
s.c_str();                    // -> char* (null-terminated)
s.length();                   // byte length (excludes null)
s.is_empty();                 // -> bool
s.utf8_len();                 // character count (not byte count)
```

### Mutation

```zc
s.append(&other);             // append String
s.append_c("world");          // append char*
s.push_rune('!');             // append single rune
s.insert_rune(0, 'H');        // insert rune at char index
s.remove_rune_at(0);          // remove rune at char index -> rune
```

### Creating New Strings

```zc
let s2 = s.add(&other);       // concatenate -> new String
s.add_assign(&other);         // += in place
let sub = s.substring(0, 5);  // start, byte_length -> new String
let parts = s.split(',');     // -> Vec<String>
let t = s.trim();             // strip whitespace -> new String
let r = s.replace("old", "new");  // -> new String
let lo = s.to_lowercase();
let hi = s.to_uppercase();
let padded = s.pad_left(20, ' ');
let padded = s.pad_right(20, ' ');
```

### Search

```zc
s.find('o');                   // -> Option<usize> (byte index)
s.find_str("world");           // -> Option<usize>
s.find_all_str("ll");          // -> Vec<usize>
s.contains('o');               // -> bool
s.contains_str("world");      // -> bool
s.starts_with("hello");       // -> bool
s.ends_with("world");         // -> bool
```

### Comparison

```zc
s.eq(&other);                 // String == String
s.eq_str("literal");          // String == char*
s.compare(&other);            // -> int (strcmp)
s.eq_ignore_case(&other);     // case-insensitive
s.lt(&other);                 // <
s.gt(&other);                 // >
```

### UTF-8

```zc
s.utf8_len();                  // character count
s.utf8_at(0);                  // char at index -> String
s.utf8_get(0);                 // char at index -> rune
s.utf8_substr(1, 3);           // char-based substring
s.runes();                     // -> Vec<rune>
for c in s { }                 // iterate runes via chars()
```

### Memory

```zc
s.free();                      // manual free
s.destroy();                   // alias for free
s.forget();                    // prevent Drop from freeing
// Internal buffer: s.vec.data (char*)
```

---

## Map\<V\> — Hash Map (string keys)

```zc
import "std/map.zc"
```

```zc
let m = Map<int>::new();
m.put("one", 1);
m.put("two", 2);

m.get("one");                 // -> Option<int>
m.contains("one");            // -> bool
m.remove("two");
m.length();                   // -> usize
m.is_empty();                 // -> bool

// Iteration
for entry in m {
    println "{entry.key}: {entry.val}";
}
// entry is MapEntry<V> { key: char*, val: V }

// Low-level slot access
m.capacity();
m.is_slot_occupied(idx);
m.key_at(idx);
m.val_at(idx);

m.free();                     // manual free (implements Drop)
```

---

## Set\<T\> — Hash Set

```zc
import "std/set.zc"
```

```zc
let s = Set<int>::new();
s.add(1);                     // -> bool (false if duplicate)
s.add(2);
s.add(1);                     // false, already present

s.contains(1);                // -> bool
s.remove(1);                  // -> bool
s.length();
s.is_empty();
s.clear();
s.free();
```

---

## Stack\<T\> — LIFO

```zc
import "std/stack.zc"
```

```zc
let st = Stack<int>::new();
st.push(1);
st.push(2);
let val = st.pop();           // -> Option<int>
st.length();
st.is_empty();
let st2 = st.clone();
st.clear();
st.free();                    // implements Drop
```

---

## Queue\<T\> — FIFO (Ring Buffer)

```zc
import "std/queue.zc"
```

```zc
let q = Queue<int>::new();
q.push(1);
q.push(2);
let val = q.pop();            // -> Option<int>
q.length();
q.is_empty();
let q2 = q.clone();
q.clear();
q.free();                     // implements Drop
```

---

## Slice\<T\> — Non-Owning View

```zc
import "std/slice.zc"
```

```zc
let sl = Slice<int>::from_array(ptr, len);
sl.get(0);                    // -> Option<T>
sl.at(0);                     // -> Option<T>
sl.length();
sl.is_empty();
for item in sl { }            // iteration via SliceIter
```

---

# Standard Library: Error Handling & Testing

## Option\<T\>

```zc
import "std/option.zc"
```

Represents an optional value — either `Some(value)` or `None`.

```zc
// Construction
let some = Option<int>::Some(42);
let none = Option<int>::None();

// Query
some.is_some();               // true
some.is_none();               // false

// Extract value
let val = some.unwrap();      // panics if None
let ref = some.unwrap_ref();  // -> T* (panics if None)
let val = none.unwrap_or(0);  // default if None
let val = some.expect("should have a value");  // panics with msg

// Chaining
let result = opt1.or_else(opt2);  // returns opt2 if opt1 is None

// Prevent Drop from cleaning up inner value
some.forget();
```

### Common Usage

```zc
let pos = s.find('x');         // -> Option<usize>
if pos.is_some() {
    let idx = pos.unwrap();
    println "found at {idx}";
}

// Or more concisely
let val = map.get("key").unwrap_or(default_val);
```

---

## Result\<T\>

```zc
import "std/result.zc"
```

Represents success (`Ok`) or failure (`Err`) with an error message.

```zc
// Construction
let ok = Result<int>::Ok(42);
let err = Result<int>::Err("something went wrong");

// Query
ok.is_ok();                   // true
ok.is_err();                  // false
err.err;                      // -> char* error message

// Extract value
let val = ok.unwrap();        // panics if Err
let ref = ok.unwrap_ref();    // -> T* (panics if Err)
let val = ok.expect("need this");  // panics with custom msg

// Prevent Drop
ok.forget();
```

### Pattern Matching on Result

```zc
fn divide(a: int, b: int) -> Result<int> {
    if b == 0 { return Result<int>::Err("division by zero"); }
    return Result<int>::Ok(a / b);
}

match divide(10, 0) {
    Ok(val) => println "result: {val}",
    Err(e)  => println "error: {e}"
}

// Imperative style
let res = divide(10, 2);
if res.is_ok() {
    let val = res.unwrap();
    println "result: {val}";
} else {
    println "error: {res.err}";
}
```

---

## Assertions

```zc
assert(x == 42);                        // panics if false
assert(x > 0, "x must be positive");   // with message
```

---

## Test Blocks

Zen-C has built-in test support via the `test` keyword. No custom harness needed.

```zc
import "./my_module.zc"

test "basic arithmetic" {
    assert(2 + 2 == 4, "math works");
}

test "string operations" {
    let s = String::new("hello");
    assert(s.length() == 5, "length should be 5");
    assert(s.starts_with("hel"), "starts with hel");
}

test "option handling" {
    let opt = Option<int>::Some(42);
    assert(opt.is_some(), "should be some");
    assert(opt.unwrap() == 42, "should be 42");
}

test "c interop via raw block" {
    raw {
        int x = 10;
        assert(x == 10, "raw block works");
    }
}
```

- No `main()` function needed — `test` blocks auto-generate `_z_run_tests()`
- Each test is independent
- Failed assertions print file, line, and message then exit
- Build test files separately from main binary

---

## Panic

```zc
panic("fatal error");          // prints message and exits

// Implicit panic via !
!"Fatal: something broke";    // prints to stderr and continues
// (Note: ! alone doesn't exit — use panic() or exit(1) for that)
```

---

# Standard Library: I/O, Files, Paths, Environment & Process

## I/O (std/io.zc)

```zc
import "std/io.zc"
```

### Output

```zc
print("Hello %s\n", name);    // printf-style
println("Hello %s", name);    // printf-style + newline

// Preferred: use println keyword with interpolation
println "Hello, {name}!";
```

### Formatting

```zc
let s = format("x=%d", x);            // -> char* (static buffer, not thread-safe)
let s = format_new("x=%d", x);        // -> char* (heap, caller must free)
format_into(buf, size, "x=%d", x);    // into existing buffer
```

### Input

```zc
let line = readln();                   // -> char* (heap, NULL on EOF)
let r = read_rune();                   // -> rune (single UTF-8 char)
```

### Conversions

```zc
let s = itos(42);                      // int -> char* (static buffer)
let s = itos_new(42);                  // int -> char* (heap, must free)
let s = utos(42);                      // uint -> char*
```

---

## File System (std/fs.zc)

```zc
import "std/fs.zc"
```

### Reading Files

```zc
// Read entire file as String
let content = File::read_all("input.txt");   // -> Result<String>
if content.is_ok() {
    let text = content.unwrap();
    println "{text.c_str()}";
}

// Read lines
let lines = File::read_lines("data.txt");    // -> Result<Vec<String>>

// Manual file handle
let res = File::open("data.bin", "rb");      // -> Result<File>
let f = res.unwrap();
let content = f.read_to_string();            // -> Result<String>
f.close();
```

### Writing Files

```zc
let res = File::open("output.txt", "w");
let f = res.unwrap();
f.write_string("Hello, World!\n");          // -> Result<bool>
f.close();

// Write lines
let lines = Vec<String>::new();
lines.push(String::from("line 1"));
lines.push(String::from("line 2"));
File::write_lines("output.txt", &lines);    // -> Result<bool>
```

### File Operations

```zc
File::exists("myfile.txt");                 // -> bool
File::create_dir("new_dir");               // -> Result<bool>
File::remove_file("temp.txt");             // -> Result<bool>
File::remove_dir("empty_dir");             // -> Result<bool>
File::current_dir();                       // -> Result<String>
```

### Metadata & Directory Listing

```zc
let meta = File::metadata("file.txt");      // -> Result<Metadata>
let m = meta.unwrap();
m.size;                                     // U64
m.is_file;                                  // bool
m.is_dir;                                   // bool

let entries = File::read_dir(".");          // -> Result<Vec<DirEntry>>
for entry in entries.unwrap() {
    println "{entry.name.c_str()} dir={entry.is_dir}";
}
```

---

## Path (std/path.zc)

```zc
import "std/path.zc"
```

```zc
let p = Path::new("/var/log");
let p2 = p.join("syslog");                // -> Path ("/var/log/syslog")
p.c_str();                                // -> char*

let ext = p.extension();                   // -> Option<String>
let name = p.file_name();                  // -> Option<String>
let parent = p.parent();                   // -> Option<Path>

let p3 = p.clone();
p.free();
```

---

## Environment (std/env.zc)

```zc
import "std/env.zc"
```

```zc
// Get (borrowed char* — do not free)
let home = Env::get("HOME");              // -> Option<string>
if home.is_some() {
    println "HOME={home.unwrap()}";
}

// Get (owned String — auto-freed)
let home = Env::get_dup("HOME");          // -> Option<String>

// Set / Unset
Env::set("MY_VAR", "value");             // -> EnvRes (OK or ERR)
Env::unset("MY_VAR");                    // -> EnvRes
```

---

## Process (std/process.zc)

```zc
import "std/process.zc"
```

```zc
// Run command and capture output
let cmd = Command::new("echo");
cmd.arg("hello").arg("world");            // builder pattern

let output = cmd.output();                // -> Output
println "exit: {output.exit_code}";
println "stdout: {output.std_out.c_str()}";

// Run command, get exit code only
let status = cmd.status();                // -> int

cmd.free();
```

---

# Standard Library: Concurrency & Networking

## Threading (std/thread.zc)

```zc
import "std/thread.zc"
```

```zc
// Spawn thread with closure
let t = Thread::spawn(fn() {
    println "running in thread";
});

// Wait for completion
let result = t.unwrap();
result.join();                // -> Result<bool>

// Or detach
result.detach();              // -> Result<bool>

// Cancel
result.cancel();              // -> Result<bool>

// Sleep
sleep_ms(100);                // milliseconds
```

---

## Synchronization (std/sync.zc)

```zc
import "std/sync.zc"
```

### Mutex

```zc
let mtx = Mutex::new();
mtx.lock();
// critical section
mtx.unlock();

if mtx.try_lock() {
    // got the lock
    mtx.unlock();
}
mtx.free();                   // or let Drop handle it
```

### RwLock

```zc
let rw = RwLock::new();
rw.rdlock();                  // shared read lock
// ... read ...
rw.unlock();

rw.wrlock();                  // exclusive write lock
// ... write ...
rw.unlock();

rw.try_rdlock();              // -> bool
rw.try_wrlock();              // -> bool
rw.free();
```

### CondVar

```zc
let cond = CondVar::new();
let mtx = Mutex::new();

// Waiting thread
mtx.lock();
cond.wait(&mtx);             // atomically unlock + wait + relock
mtx.unlock();

// Signaling thread
cond.signal();                // wake one waiter
cond.broadcast();             // wake all waiters
cond.free();
```

### Semaphore

```zc
let sem = Semaphore::new(3);  // initial count
sem.wait();                   // decrement (blocks if 0)
sem.try_wait();               // -> bool (non-blocking)
sem.post();                   // increment
sem.value();                  // -> int (current count)
sem.free();
```

### Barrier

```zc
let barrier = Barrier::new(4);  // number of threads
barrier.wait();               // blocks until all threads arrive -> bool
barrier.free();
```

### Once

```zc
let once = Once::new();
once.call(fn() {
    println "runs exactly once";
});
once.free();
```

All synchronization primitives implement `Drop` for automatic cleanup.

---

## TCP Networking (std/net/tcp.zc)

```zc
import "std/net/tcp.zc"
```

### TCP Server

```zc
let listener = TcpListener::bind("127.0.0.1", 8080);  // -> Result<TcpListener>
let server = listener.unwrap();

loop {
    let client = server.accept();               // -> Result<TcpStream>
    if client.is_ok() {
        let stream = client.unwrap();
        let buf: char[1024];
        let n = stream.read(&buf[0], 1024);     // -> Result<usize>
        stream.write((u8*)&buf[0], n.unwrap()); // -> Result<usize>
        stream.close();
    }
}
server.close();
```

### TCP Client

```zc
let conn = TcpStream::connect("example.com", 80);  // -> Result<TcpStream>
let stream = conn.unwrap();
stream.write((u8*)"GET / HTTP/1.0\r\n\r\n", 18);
let buf: char[4096];
let n = stream.read(&buf[0], 4096);
stream.close();
```

---

## UDP Networking (std/net/udp.zc)

```zc
import "std/net/udp.zc"
```

```zc
let sock = UdpSocket::bind("0.0.0.0", 9000).unwrap();

// Receive
let buf: char[1024];
let result = sock.recv_from(&buf[0], 1024);     // -> Result<UdpRecvResult>
let recv = result.unwrap();
// recv.n: usize (bytes read)
// recv.host: String (sender IP)
// recv.port: c_int (sender port)

// Send
sock.send_to("hello", 5, "127.0.0.1", 9001);   // -> Result<usize>
sock.close();
```

---

## HTTP (std/net/http.zc)

```zc
import "std/net/http.zc"
```

### HTTP Server

```zc
fn handler(req: Request*, res: Response*) {
    res.set_header_str("Content-Type", "text/plain");
    res.set_body_str("Hello, World!");
}

let server = Server::new(8080, handler);
server.start();                // blocks, serving requests
```

### HTTP Client

```zc
let url = String::new("http://example.com");
let response = fetch(url);     // -> Response
// response.status: int
// response.body: String
// response.headers: Vec<Header>
```

---

## URL Parsing (std/net/url.zc)

```zc
import "std/net/url.zc"
```

```zc
let raw = String::new("http://example.com:8080/path?query=1");
let result = Url::parse(raw);  // -> Result<Url>
let url = result.unwrap();
// url.scheme: String ("http")
// url.host: String ("example.com")
// url.port: int (8080)
// url.path: String ("/path")
// url.query: String ("query=1")
url.destroy();
```

---

## DNS (std/net/dns.zc)

```zc
import "std/net/dns.zc"
```

```zc
let ip = Dns::resolve("example.com");  // -> Result<String>
println "IP: {ip.unwrap().c_str()}";
```

---

## WebSocket (std/net/websocket.zc)

```zc
import "std/net/websocket.zc"
```

```zc
let ws = WebSocket::handshake(stream, key);  // -> Result<WebSocket>
let socket = ws.unwrap();
socket.send(String::from("hello"));          // -> Result<int>
let msg = socket.recv();                     // -> Result<String>
```

---

# Standard Library: JSON, Regex, Time, Math & More

## JSON (std/json.zc)

```zc
import "std/json.zc"
```

### Parsing

```zc
let result = JsonValue::parse(json_str);     // -> Result<JsonValue*>
let root: JsonValue* = result.unwrap();

// Or parse to stack value
let result = JsonValue::parse_val(json_str); // -> Result<JsonValue>
```

### Reading Values

```zc
// Type checks
(*root).is_object();
(*root).is_array();
(*root).is_string();
(*root).is_number();
(*root).is_bool();
(*root).is_null();

// Object access
(*root).get("key");                    // -> Option<JsonValue*>
(*root).get_string("name");           // -> Option<char*>
(*root).get_int("count");             // -> Option<int>
(*root).get_float("price");           // -> Option<double>
(*root).get_bool("active");           // -> Option<bool>
(*root).get_object("nested");         // -> Option<JsonValue*>
(*root).get_array("items");           // -> Option<JsonValue*>

// Array access
(*arr).at(0);                         // -> Option<JsonValue*>
(*arr).len();                         // -> usize

// Direct value extraction
(*val).as_string();                   // -> Option<char*>
(*val).as_int();                      // -> Option<int>
(*val).as_float();                    // -> Option<double>
(*val).as_bool();                     // -> Option<bool>
```

### Building JSON

```zc
// Primitives
let v = JsonValue::null();
let v = JsonValue::bool(true);
let v = JsonValue::number(3.14);
let v = JsonValue::string("hello");

// Array
let arr = JsonValue::array();
arr.push(JsonValue::number(1.0));
arr.push(JsonValue::string("two"));

// Object
let obj = JsonValue::object();
obj.set("name", JsonValue::string("Alice"));
obj.set("age", JsonValue::number(30.0));
obj.set("tags", arr);

// Heap-allocated variants (for nesting)
let p = JsonValue::string_ptr("hello");  // -> JsonValue*
```

### Serialization

```zc
let json_string = obj.to_string();     // -> String
println "{json_string.c_str()}";
```

### Cleanup

```zc
(*root).free();                        // recursive free
// JsonValue implements Drop
```

---

## Regex (std/regex.zc)

```zc
import "std/regex.zc"
```

### Quick Helpers

```zc
regex_match("[0-9]+", "abc123");       // -> bool
regex_find("[0-9]+", "abc123");        // -> Option<Match>
regex_count("[0-9]", "a1b2c3");        // -> int (3)
regex_split(",", "a,b,c");            // -> Vec<String>
```

### Compiled Regex

```zc
let re = Regex::compile("^[a-z]+$");
re.is_valid();                         // -> bool
re.match("hello");                     // -> bool
re.find("hello world");               // -> Option<Match>
re.count("hello");                    // -> int
re.split("a-b-c");                    // -> Vec<String>
re.destroy();

// Match object
let m = re.find("year 2024").unwrap();
m.start;                              // int (byte offset)
m.len;                                // int (byte length)
m.end();                              // -> int (start + len)
let text = m.as_string();             // -> char* (must free)
```

### Validation

```zc
Regex::is_valid_pattern("[a-z]+");    // -> bool (static)
```

---

## Random (std/random.zc)

```zc
import "std/random.zc"
```

```zc
let rng = Random::new();              // seeded from Time::now()
let rng = Random::from_seed(12345);   // manual seed

rng.next_int();                       // [0, RAND_MAX]
rng.next_int_range(1, 100);           // [1, 100] inclusive
rng.next_double();                    // [0.0, 1.0)
rng.next_bool();                      // true or false
```

---

## Time (std/time.zc)

```zc
import "std/time.zc"
```

```zc
let start = Time::now();               // -> U64 (milliseconds)
// ... work ...
let elapsed = Time::now() - start;

Time::sleep_ms(500);                   // sleep 500ms
Time::sleep(Duration::from_secs(2));   // sleep 2 seconds

let d = Duration::from_ms(100);
let d = Duration::from_secs(5);
d.millis;                              // U64
```

---

## Hash (std/hash.zc)

```zc
import "std/hash.zc"
```

FNV-1a hashing:

```zc
let h = hash_compute(&data, sizeof(data));   // -> usize
let h = hash_combine(h1, h2);               // -> usize
hash_set_seed(42);                           // set global seed
let seed = hash_get_seed();                  // get current seed
```

---

## Sort (std/sort.zc)

```zc
import "std/sort.zc"
```

Quicksort for primitive types:

```zc
let arr: int[5] = [5, 3, 1, 4, 2];
sort_int(&arr[0], 5);                  // in-place sort
sort_long(arr, len);
sort_float(arr, len);
sort_double(arr, len);
```

---

## Math (std/math.zc)

```zc
import "std/math.zc"
```

```zc
Math::PI();  Math::E();
Math::abs(x);  Math::sqrt(x);  Math::pow(base, exp);
Math::sin(x);  Math::cos(x);  Math::tan(x);
Math::asin(x); Math::acos(x); Math::atan(x); Math::atan2(y, x);
Math::exp(x);  Math::log(x);  Math::log10(x);
Math::ceil(x); Math::floor(x); Math::round(x);
Math::max(a, b); Math::min(a, b);
Math::mod(x, y);
```

---

## Bits (std/bits.zc)

```zc
import "std/bits.zc"
```

All operations available for 8/16/32/64/128-bit widths:

```zc
Bits::rotl32(n, count);      Bits::rotr32(n, count);
Bits::popcount32(n);         // population count (hamming weight)
Bits::clz32(n);              // count leading zeros
Bits::ctz32(n);              // count trailing zeros
Bits::bswap32(n);            // byte swap (endianness)
Bits::reverse_bits32(n);     // reverse all bits
```

---

## UTF-8 (std/utf8.zc)

```zc
import "std/utf8.zc"
```

```zc
Utf8::is_digit(r);           Utf8::is_alpha(r);
Utf8::is_whitespace(r);      Utf8::is_upper(r);
Utf8::is_lower(r);
Utf8::to_upper(r);           Utf8::to_lower(r);
Utf8::encode(r, buf);        // -> usize (bytes written)
Utf8::decode(data, len, &consumed);  // -> rune
Utf8::rune_len(r);           // -> usize (1-4)
Utf8::is_valid(data, len);   // -> bool
```

---

## BigInt (std/bigint.zc)

```zc
import "std/bigint.zc"
```

```zc
let a = BigInt::from_int(123456789);
let b = BigInt::from_int(987654321);
let sum = a.add(&b);
let s = sum.to_string();     // -> char*
let c = a.clone();
a.free_mem();
```

---

## BigFloat (std/bigfloat.zc)

```zc
import "std/bigfloat.zc"
```

```zc
let a = BigFloat::from_int(123);
a.scale = 2;                 // 2 decimal places
let b = BigFloat::from_int(456);
b.scale = 2;
let sum = a.add(b);
let s = sum.to_string();     // -> char*
```

---

## Complex (std/complex.zc)

```zc
import "std/complex.zc"
```

```zc
let c1 = Complex::new(3.0, 4.0);    // 3 + 4i
let c2 = Complex::new(1.0, -2.0);
let sum = c1.add(c2);
let prod = c1.mul(c2);
c1.magnitude();                      // -> double (5.0)
c1.phase();                         // -> double (atan2)
c1.eq(c2);                          // -> bool
let s = c1.to_string();             // -> String
```

---

## SIMD (std/simd.zc)

```zc
import "std/simd.zc"
```

128-bit vectors (SSE/NEON):

```zc
let a = f32x4{1.0, 2.0, 3.0, 4.0};
let b = f32x4{v: 2.0};              // broadcast
let c = a + b;                       // element-wise
let d = a * b;
let x = c[0];                       // lane access

// Types: f32x4, f64x2, i32x4, u32x4, i64x2, u64x2,
//        i16x8, u16x8, i8x16, u8x16
```

256-bit vectors (AVX):

```zc
let v = i32x8{1, 2, 3, 4, 5, 6, 7, 8};
// Types: f32x8, f64x4, i32x8, u32x8, i64x4, u64x4, etc.
```

---

## CUDA (std/cuda.zc)

```zc
import "std/cuda.zc"
// Compile with: zc build --cuda file.zc
```

```zc
let d = cuda_alloc<float>(1024);
defer cuda_free(d);
cuda_zero(d, 1024);
cuda_copy_to_device(d, host_ptr, bytes);
cuda_copy_to_host(host_ptr, d, bytes);
cuda_sync();

// Kernel builtins
thread_id();  block_id();  local_id();
block_size(); grid_size();
```

---

## Encoding (std/encoding/)

```zc
import "std/encoding/base64.zc"
import "std/encoding/hex.zc"
```

```zc
let encoded = Base64::encode((u8*)data, len);  // -> String
let hex = Hex::encode((u8*)data, len);         // -> String
let decoded = Hex::decode(hex_string);         // -> Result<Vec<u8>>
```

---

## Crypto (std/crypto/)

```zc
import "std/crypto/sha256.zc"
import "std/crypto/sha1.zc"
```

```zc
let hash = Sha256::hash("hello");     // -> String (hex digest)

let digest = Sha1::hash((u8*)data, len);  // -> Sha1Digest
digest.bytes[0..19];                       // 20-byte hash
```

---

## Signal Handling (std/sys/signal.zc)

```zc
Signal::set_handler(Z_SIGINT, my_handler);
// Z_SIGINT=2, Z_SIGTERM=15, Z_SIGSEGV=11, etc.
```

---

## Memory Mapping (std/sys/mman.zc)

```zc
let ptr = Memory::mmap(4096, Z_PROT_READ | Z_PROT_WRITE, Z_MAP_PRIVATE | Z_MAP_ANONYMOUS);
Memory::munmap(ptr, 4096);
Memory::mprotect(ptr, 4096, Z_PROT_READ);
```
