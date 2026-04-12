#!/usr/bin/env bash
set -euo pipefail

IMAGE_TAG=${IMAGE_TAG:-$GITHUB_RUN_NUMBER}

docker run --rm book-manager-api:$IMAGE_TAG sh -c "test -f /app/Back.Api.dll"
