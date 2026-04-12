#!/usr/bin/env bash
set -euo pipefail

cd Front
npm ci
npm run build