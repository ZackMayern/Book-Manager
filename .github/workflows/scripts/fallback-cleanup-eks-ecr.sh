#!/usr/bin/env bash
set -euo pipefail

if aws eks describe-cluster --region "$AWS_REGION" --name "$EKS_CLUSTER_NAME" >/dev/null 2>&1; then
  NODEGROUPS=$(aws eks list-nodegroups --region "$AWS_REGION" --cluster-name "$EKS_CLUSTER_NAME" --query 'nodegroups' --output text)
  if [ -n "$NODEGROUPS" ] && [ "$NODEGROUPS" != "None" ]; then
    for NG in $NODEGROUPS; do
      aws eks delete-nodegroup --region "$AWS_REGION" --cluster-name "$EKS_CLUSTER_NAME" --nodegroup-name "$NG"
      aws eks wait nodegroup-deleted --region "$AWS_REGION" --cluster-name "$EKS_CLUSTER_NAME" --nodegroup-name "$NG"
    done
  fi

  aws eks delete-cluster --region "$AWS_REGION" --name "$EKS_CLUSTER_NAME"
else
  echo "EKS cluster not found, skipping fallback EKS cleanup."
fi

if aws ecr describe-repositories --region "$AWS_REGION" --repository-names "$ECR_REPOSITORY_NAME" >/dev/null 2>&1; then
  aws ecr delete-repository --region "$AWS_REGION" --repository-name "$ECR_REPOSITORY_NAME" --force
else
  echo "ECR repository not found, skipping fallback ECR cleanup."
fi
