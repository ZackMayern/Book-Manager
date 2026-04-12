#!/usr/bin/env bash
set -euo pipefail

aws eks update-kubeconfig --region "$AWS_REGION" --name "$EKS_CLUSTER_NAME"
