export PATH := $(realpath usr/bin):$(PATH)
CC = cosmocc
ZC = zc.com
CFLAGS = -Os -s

# Wrapper script that captures the transpiled C file
CAPTURE_CC = $(realpath .)/capture-cc.sh

all: out/runtimedl.com

# Two-step build:
# 1. Transpile .zc -> .c via zc.com (link step will fail due to missing cJSON, that's OK)
# 2. Compile transpiled .c + cJSON.c together with cosmocc
out/runtimedl.com: runtimedl.zc cJSON.c cJSON.h capture-cc.sh
	@mkdir -p out
	@echo "Step 1: Transpiling Zen-C to C..."
	$(ZC) build --cc $(CAPTURE_CC) -o $@ $< 2>/dev/null || true
	@test -f out/transpiled.c || { echo "Error: transpilation failed"; exit 1; }
	@echo "Step 2: Compiling with cosmocc..."
	$(CC) $(CFLAGS) -I. -o $@ out/transpiled.c cJSON.c
	@echo "Built: $@ ($$(du -h $@ | cut -f1))"

capture-cc.sh:
	@printf '#!/bin/sh\nfor f in "$$@"; do case "$$f" in *.c) cp "$$f" out/transpiled.c 2>/dev/null ;; esac; done\nexec %s/usr/bin/cosmocc "$$@"\n' "$(CURDIR)" > $@
	@chmod +x $@

clean:
	rm -rf out capture-cc.sh

.PHONY: all clean
