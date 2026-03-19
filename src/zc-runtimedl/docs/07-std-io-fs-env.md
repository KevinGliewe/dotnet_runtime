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
