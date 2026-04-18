#!/usr/bin/env bash
set -euo pipefail

IMAGE_TAG=${IMAGE_TAG:-$GITHUB_RUN_NUMBER}

docker build \
  -f Front/Dockerfile \
  -t book-manager-ui:$IMAGE_TAG \
  ./Front