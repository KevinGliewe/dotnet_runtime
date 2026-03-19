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
