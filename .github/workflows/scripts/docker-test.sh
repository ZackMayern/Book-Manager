#!/usr/bin/env bash
set -euo pipefail

IMAGE_TAG=${IMAGE_TAG:-$GITHUB_RUN_NUMBER}

# Override the image ENTRYPOINT so this remains a quick, non-blocking smoke test.
docker run --rm --entrypoint sh book-manager-api:$IMAGE_TAG -c "test -f /app/Back.Api.dll"
