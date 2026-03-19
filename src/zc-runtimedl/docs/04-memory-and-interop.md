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
