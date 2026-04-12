#!/usr/bin/env bash
set -euo pipefail

if [ -z "${TF_STATE_BUCKET:-}" ]; then
  echo "Missing required workflow input: tf_state_bucket"
  exit 1
fi

if [ -z "${TF_STATE_KEY:-}" ]; then
  echo "Missing required workflow input: tf_state_key"
  exit 1
fi

if aws s3api head-bucket --bucket "$TF_STATE_BUCKET" 2>/dev/null; then
  echo "Terraform state bucket already exists: $TF_STATE_BUCKET"
else
  if [ "$AWS_REGION" = "us-east-1" ]; then
    aws s3api create-bucket --bucket "$TF_STATE_BUCKET"
  else
    aws s3api create-bucket \
      --bucket "$TF_STATE_BUCKET" \
      --create-bucket-configuration "LocationConstraint=$AWS_REGION"
  fi

  aws s3api put-bucket-encryption \
    --bucket "$TF_STATE_BUCKET" \
    --server-side-encryption-configuration '{"Rules":[{"ApplyServerSideEncryptionByDefault":{"SSEAlgorithm":"AES256"}}]}'

  aws s3api put-bucket-versioning \
    --bucket "$TF_STATE_BUCKET" \
    --versioning-configuration Status=Enabled

  echo "Created Terraform state bucket: $TF_STATE_BUCKET"
fi

echo "Terraform state key: $TF_STATE_KEY"