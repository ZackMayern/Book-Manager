#!/usr/bin/env bash
set -euo pipefail

IMAGE_TAG=${IMAGE_TAG:-$GITHUB_RUN_NUMBER}
IMAGE_URI="$AWS_ACCOUNT_ID.dkr.ecr.$AWS_REGION.amazonaws.com/$ECR_REPOSITORY_NAME:$IMAGE_TAG"

echo "IMAGE_URI=$IMAGE_URI" >> "$GITHUB_ENV"
docker tag "book-manager-api:$IMAGE_TAG" "$IMAGE_URI"
docker push "$IMAGE_URI"
