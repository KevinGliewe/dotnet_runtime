export PATH := $(realpath usr/bin):$(PATH)
CC = cosmocc
ZC = zc.com
CFLAGS = -Os -s

# Pinned dependency versions
MBEDTLS_VERSION = 3.6.5
ZENC_STD_URL = https://github.com/zenc-lang/zenc/archive/refs/heads/main.tar.gz

# Paths
MBEDTLS_DIR = extern/mbedtls
ZENC_STD_DIR = extern/zenc-std

# Wrapper script that captures the transpiled C file
CAPTURE_CC = $(realpath .)/capture-cc.sh

# Source modules (zc.com follows imports from entry point)
ZC_SOURCES = runtimedl.zc runtimedb.zc semver.zc platform.zc http.zc archive.zc

all: out/runtimedl.com

# ---- Download external dependencies ----

$(MBEDTLS_DIR)/include/mbedtls/ssl.h:
	@echo "Downloading mbedTLS $(MBEDTLS_VERSION)..."
	@mkdir -p extern
	@curl -sL https://github.com/Mbed-TLS/mbedtls/releases/download/mbedtls-$(MBEDTLS_VERSION)/mbedtls-$(MBEDTLS_VERSION).tar.bz2 | tar xj -C extern
	@mv extern/mbedtls-$(MBEDTLS_VERSION) $(MBEDTLS_DIR)

$(ZENC_STD_DIR)/json.zc:
	@echo "Downloading Zen-C std library..."
	@mkdir -p extern
	@curl -sL $(ZENC_STD_URL) | tar xz -C extern
	@mv extern/zenc-main/std $(ZENC_STD_DIR)
	@cp extern/zenc-main/std.zc $(ZENC_STD_DIR)/
	@rm -rf extern/zenc-main

# ---- Build mbedTLS ----

$(MBEDTLS_DIR)/library/.built: $(MBEDTLS_DIR)/include/mbedtls/ssl.h
	@echo "Compiling mbedTLS..."
	@for f in $(MBEDTLS_DIR)/library/*.c; do \
		$(CC) $(CFLAGS) -I$(MBEDTLS_DIR)/include -c -o "$${f%.c}.o" "$$f"; \
	done
	@touch $@

# ---- Main build ----

deps: $(MBEDTLS_DIR)/library/.built $(ZENC_STD_DIR)/json.zc

out/runtimedl.com: $(ZC_SOURCES) capture-cc.sh | deps
	@mkdir -p out
	@echo "Step 1: Transpiling Zen-C to C..."
	$(ZC) build --cc $(CAPTURE_CC) -o $@ runtimedl.zc 2>/dev/null || true
	@test -f out/transpiled.c || { echo "Error: transpilation failed"; exit 1; }
	@echo "Step 2: Compiling with cosmocc..."
	$(CC) $(CFLAGS) -I$(MBEDTLS_DIR)/include -o $@ out/transpiled.c \
		$$(find $(MBEDTLS_DIR)/library -maxdepth 1 -name '*.o')
	@echo "Built: $@ ($$(du -h $@ | cut -f1))"

# ---- Tests ----

out/test_semver.com: test_semver.zc semver.zc capture-cc.sh | deps
	@mkdir -p out
	@echo "Building semver tests..."
	@rm -f out/transpiled.c
	$(ZC) build --cc $(CAPTURE_CC) -o $@ test_semver.zc 2>/dev/null || true
	@test -f out/transpiled.c || { echo "Error: transpilation failed"; exit 1; }
	$(CC) $(CFLAGS) -o $@ out/transpiled.c
	@echo "Built: $@"

out/test_platform.com: test_platform.zc platform.zc capture-cc.sh | deps
	@mkdir -p out
	@echo "Building platform tests..."
	@rm -f out/transpiled.c
	$(ZC) build --cc $(CAPTURE_CC) -o $@ test_platform.zc 2>/dev/null || true
	@test -f out/transpiled.c || { echo "Error: transpilation failed"; exit 1; }
	$(CC) $(CFLAGS) -o $@ out/transpiled.c
	@echo "Built: $@"

test: out/test_semver.com out/test_platform.com
	@echo "Running tests..."
	./out/test_semver.com
	./out/test_platform.com
	@echo "All tests passed."

capture-cc.sh:
	@printf '#!/bin/sh\nfor f in "$$@"; do case "$$f" in *.c) cp "$$f" out/transpiled.c 2>/dev/null ;; esac; done\nexec %s/usr/bin/cosmocc "$$@"\n' "$(CURDIR)" > $@
	@chmod +x $@

clean:
	rm -rf out capture-cc.sh

distclean: clean
	rm -rf extern

.PHONY: all clean distclean deps test
