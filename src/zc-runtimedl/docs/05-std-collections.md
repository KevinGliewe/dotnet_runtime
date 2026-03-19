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
