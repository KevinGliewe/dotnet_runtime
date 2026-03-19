export PATH := $(realpath usr/bin):$(PATH)
CC = cosmocc
ZC = zc.com
CFLAGS = -Os -s

# Pinned dependency versions
MBEDTLS_VERSION = 3.6.5
ZENC_STD_URL = https://github.com/zenc-lang/zenc/archive/refs/tags/v0.4.3.tar.gz

# Paths
MBEDTLS_DIR = extern/mbedtls
ZENC_STD_DIR = extern/zenc-std

# Post-processing script for transpiled C (works around zc.com duplicate definitions bug)
DEDUP = ./dedup-transpiled.sh

# Source modules (zc.com follows imports from entry point)
ZC_SOURCES = runtimedl.zc runtimedl_helpers.zc runtimedb.zc semver.zc platform.zc http.zc archive.zc

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
	@mv extern/zenc-0.4.3/std $(ZENC_STD_DIR)
	@cp extern/zenc-0.4.3/std.zc $(ZENC_STD_DIR)/
	@rm -rf extern/zenc-0.4.3

# ---- Build mbedTLS ----

$(MBEDTLS_DIR)/library/.built: $(MBEDTLS_DIR)/include/mbedtls/ssl.h
	@echo "Compiling mbedTLS..."
	@for f in $(MBEDTLS_DIR)/library/*.c; do \
		$(CC) $(CFLAGS) -I$(MBEDTLS_DIR)/include -c -o "$${f%.c}.o" "$$f"; \
	done
	@touch $@

# ---- Main build ----

deps: $(MBEDTLS_DIR)/library/.built $(ZENC_STD_DIR)/json.zc

out/runtimedl.com: $(ZC_SOURCES) | deps
	@mkdir -p out
	@echo "Step 1: Transpiling Zen-C to C..."
	$(ZC) transpile -o out/transpiled.c runtimedl.zc
	$(DEDUP) out/transpiled.c
	@echo "Step 2: Compiling with cosmocc..."
	$(CC) $(CFLAGS) -I$(MBEDTLS_DIR)/include -o $@ out/transpiled.c \
		$$(find $(MBEDTLS_DIR)/library -maxdepth 1 -name '*.o')
	@echo "Built: $@ ($$(du -h $@ | cut -f1))"

# ---- Tests ----

out/test_semver.com: test_semver.zc semver.zc | deps
	@mkdir -p out
	@echo "Building semver tests..."
	$(ZC) transpile -o out/transpiled.c test_semver.zc
	$(DEDUP) out/transpiled.c
	$(CC) $(CFLAGS) -o $@ out/transpiled.c
	@echo "Built: $@"

out/test_platform.com: test_platform.zc platform.zc | deps
	@mkdir -p out
	@echo "Building platform tests..."
	$(ZC) transpile -o out/transpiled.c test_platform.zc
	$(DEDUP) out/transpiled.c
	$(CC) $(CFLAGS) -o $@ out/transpiled.c
	@echo "Built: $@"

out/test_archive.com: test_archive.zc archive.zc | deps
	@mkdir -p out
	@echo "Building archive tests..."
	$(ZC) transpile -o out/transpiled.c test_archive.zc
	$(DEDUP) out/transpiled.c
	$(CC) $(CFLAGS) -o $@ out/transpiled.c
	@echo "Built: $@"

out/test_http.com: test_http.zc http.zc | deps
	@mkdir -p out
	@echo "Building http tests..."
	$(ZC) transpile -o out/transpiled.c test_http.zc
	$(DEDUP) out/transpiled.c
	$(CC) $(CFLAGS) -I$(MBEDTLS_DIR)/include -o $@ out/transpiled.c \
		$$(find $(MBEDTLS_DIR)/library -maxdepth 1 -name '*.o')
	@echo "Built: $@"

out/test_runtimedl.com: test_runtimedl.zc runtimedl_helpers.zc | deps
	@mkdir -p out
	@echo "Building runtimedl tests..."
	$(ZC) transpile -o out/transpiled.c test_runtimedl.zc
	$(DEDUP) out/transpiled.c
	$(CC) $(CFLAGS) -o $@ out/transpiled.c
	@echo "Built: $@"

test: out/test_semver.com out/test_platform.com out/test_archive.com out/test_http.com out/test_runtimedl.com
	@echo "Running tests..."
	./out/test_semver.com
	./out/test_platform.com
	./out/test_archive.com
	./out/test_http.com
	./out/test_runtimedl.com
	@echo "All tests passed."

clean:
	rm -rf out

distclean: clean
	rm -rf extern

.PHONY: all clean distclean deps test
