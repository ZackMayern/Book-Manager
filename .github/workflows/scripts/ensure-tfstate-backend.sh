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

HEAD_OUTPUT=$(aws s3api head-bucket --bucket "$TF_STATE_BUCKET" 2>&1) && BUCKET_EXISTS=true || BUCKET_EXISTS=false

if [ "$BUCKET_EXISTS" = "true" ]; then
  echo "Terraform state bucket already exists: $TF_STATE_BUCKET"
elif echo "$HEAD_OUTPUT" | grep -q "404\|NoSuchBucket"; then
  echo "Bucket not found, creating: $TF_STATE_BUCKET"
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
else
  echo "ERROR: Could not access bucket '$TF_STATE_BUCKET'."
  echo "AWS response: $HEAD_OUTPUT"
  echo ""
  echo "This usually means the bucket name was created as an account-namespace or"
  echo "directory bucket (e.g. ending in --az-id--x-s3), which is not supported"
  echo "for Terraform state storage."
  echo ""
  echo "Fix: Create or use a standard global-namespace S3 bucket with a name like:"
  echo "  my-tfstate-book-manager-<aws-account-id>"
  echo "  (lowercase letters, numbers and hyphens only, 3-63 characters)"
  exit 1
fi

echo "Terraform state key: $TF_STATE_KEY"