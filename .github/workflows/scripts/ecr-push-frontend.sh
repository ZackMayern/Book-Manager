#!/usr/bin/env bash
set -euo pipefail

IMAGE_TAG=${IMAGE_TAG:-$GITHUB_RUN_NUMBER}
FRONTEND_IMAGE_URI="$AWS_ACCOUNT_ID.dkr.ecr.$AWS_REGION.amazonaws.com/$ECR_REPOSITORY_NAME:frontend-$IMAGE_TAG"

echo "FRONTEND_IMAGE_URI=$FRONTEND_IMAGE_URI" >> "$GITHUB_ENV"
docker tag "book-manager-ui:$IMAGE_TAG" "$FRONTEND_IMAGE_URI"
docker push "$FRONTEND_IMAGE_URI"