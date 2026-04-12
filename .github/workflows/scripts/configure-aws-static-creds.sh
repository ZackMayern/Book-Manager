#!/usr/bin/env bash
set -euo pipefail

if [ -z "${AWS_ACCESS_KEY_ID:-}" ]; then
  echo "Missing required secret: AWS_ACCESS_KEY_ID"
  exit 1
fi

if [ -z "${AWS_SECRET_ACCESS_KEY:-}" ]; then
  echo "Missing required secret: AWS_SECRET_ACCESS_KEY"
  exit 1
fi

echo "AWS_ACCESS_KEY_ID=${AWS_ACCESS_KEY_ID}" >> "$GITHUB_ENV"
echo "AWS_SECRET_ACCESS_KEY=${AWS_SECRET_ACCESS_KEY}" >> "$GITHUB_ENV"