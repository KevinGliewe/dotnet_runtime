#!/bin/sh
# Remove duplicate function definitions from zc.com transpiled C output.
# zc.com v0.4.3 emits duplicate definitions (e.g. _zen_panic) when multiple
# modules are compiled together. This script keeps only the first definition.
#
# Usage: dedup-transpiled.sh <file.c>

set -e

FILE="$1"
if [ -z "$FILE" ] || [ ! -f "$FILE" ]; then
    echo "Usage: $0 <file.c>" >&2
    exit 1
fi

awk '
# Track function signatures we have seen defined (not just declared).
# A function definition looks like: <type> <name>(<params>)\n{
# A declaration ends with ;

/^[a-zA-Z_].*\)$/ {
    # Potential function definition header (no semicolon, no opening brace on same line)
    sig = $0
    getline next_line
    if (next_line == "{") {
        # This is a function definition
        if (sig in seen) {
            # Skip this duplicate: read until matching closing brace at column 0
            depth = 1
            while (depth > 0 && (getline skip_line) > 0) {
                if (skip_line == "{") depth++
                if (skip_line == "}") depth--
            }
            next
        }
        seen[sig] = 1
        print sig
        print next_line
    } else {
        print sig
        print next_line
    }
    next
}

{ print }
' "$FILE" > "${FILE}.dedup"

mv "${FILE}.dedup" "$FILE"
