#!/usr/bin/env bash
set -euo pipefail

IMAGE_TAG=${IMAGE_TAG:-$GITHUB_RUN_NUMBER}

docker build \
  -f Back/Back.Api/Dockerfile \
  -t book-manager-api:$IMAGE_TAG \
  ./Back
