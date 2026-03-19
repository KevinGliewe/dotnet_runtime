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
