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
