export PATH := $(realpath usr/bin):$(PATH)
CC = cosmocc
ZC = zc.com
CFLAGS = -Os -s

# Pinned dependency versions
MBEDTLS_VERSION = 3.6.5
CJSON_VERSION = 1.7.18

# Paths
MBEDTLS_DIR = extern/mbedtls
CJSON_DIR = extern/cJSON

# Wrapper script that captures the transpiled C file
CAPTURE_CC = $(realpath .)/capture-cc.sh

all: out/runtimedl.com

# ---- Download external dependencies ----

$(MBEDTLS_DIR)/include/mbedtls/ssl.h:
	@echo "Downloading mbedTLS $(MBEDTLS_VERSION)..."
	@mkdir -p extern
	@curl -sL https://github.com/Mbed-TLS/mbedtls/releases/download/mbedtls-$(MBEDTLS_VERSION)/mbedtls-$(MBEDTLS_VERSION).tar.bz2 | tar xj -C extern
	@mv extern/mbedtls-$(MBEDTLS_VERSION) $(MBEDTLS_DIR)

$(CJSON_DIR)/cJSON.c:
	@echo "Downloading cJSON $(CJSON_VERSION)..."
	@mkdir -p extern
	@curl -sL https://github.com/DaveGamble/cJSON/archive/refs/tags/v$(CJSON_VERSION).tar.gz | tar xz -C extern
	@mv extern/cJSON-$(CJSON_VERSION) $(CJSON_DIR)

# ---- Build mbedTLS ----

# Stamp file to track that mbedTLS objects are built
$(MBEDTLS_DIR)/library/.built: $(MBEDTLS_DIR)/include/mbedtls/ssl.h
	@echo "Compiling mbedTLS..."
	@for f in $(MBEDTLS_DIR)/library/*.c; do \
		$(CC) $(CFLAGS) -I$(MBEDTLS_DIR)/include -c -o "$${f%.c}.o" "$$f"; \
	done
	@touch $@

# ---- Main build ----

out/runtimedl.com: runtimedl.zc capture-cc.sh $(MBEDTLS_DIR)/library/.built $(CJSON_DIR)/cJSON.c
	@mkdir -p out
	@echo "Step 1: Transpiling Zen-C to C..."
	$(ZC) build --cc $(CAPTURE_CC) -o $@ $< 2>/dev/null || true
	@test -f out/transpiled.c || { echo "Error: transpilation failed"; exit 1; }
	@echo "Step 2: Compiling with cosmocc..."
	$(CC) $(CFLAGS) -I$(MBEDTLS_DIR)/include -I$(CJSON_DIR) -o $@ out/transpiled.c $(CJSON_DIR)/cJSON.c $$(find $(MBEDTLS_DIR)/library -maxdepth 1 -name '*.o')
	@echo "Built: $@ ($$(du -h $@ | cut -f1))"

capture-cc.sh:
	@printf '#!/bin/sh\nfor f in "$$@"; do case "$$f" in *.c) cp "$$f" out/transpiled.c 2>/dev/null ;; esac; done\nexec %s/usr/bin/cosmocc "$$@"\n' "$(CURDIR)" > $@
	@chmod +x $@

clean:
	rm -rf out capture-cc.sh

distclean: clean
	rm -rf extern

.PHONY: all clean distclean
