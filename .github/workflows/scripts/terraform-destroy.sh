#!/usr/bin/env bash
set -euo pipefail

cd infra/terraform

terraform init \
  -reconfigure \
  -backend-config="bucket=$TF_STATE_BUCKET" \
  -backend-config="key=$TF_STATE_KEY" \
  -backend-config="region=$AWS_REGION" \
  -backend-config="encrypt=true"

terraform destroy -auto-approve \
  -var="region=$AWS_REGION" \
  -var="ecr_repository_name=$ECR_REPOSITORY_NAME" \
  -var="eks_cluster_name=$EKS_CLUSTER_NAME"
