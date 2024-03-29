#!/bin/bash

if ! [ -d ".git" ]; then
    echo "Not in a git repository" >&2
    exit 1
fi

SCRIPT_DIR=$(cd "${0%/*}" && pwd)

SCRIPTS=(pre-commit post-checkout post-merge pre-push post-commit)

mkdir -p .git/hooks

for SCRIPT in "${SCRIPTS[@]}"; do
    cp "$SCRIPT_DIR/$SCRIPT" ".git/hooks/$SCRIPT"
done

echo "Installed ${SCRIPTS[*]}"
