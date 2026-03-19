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
