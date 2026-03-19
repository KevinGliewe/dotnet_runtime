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
