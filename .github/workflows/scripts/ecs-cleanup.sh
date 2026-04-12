#!/usr/bin/env bash
set -euo pipefail

CLUSTER_ARN=$(aws ecs list-clusters --query "clusterArns[?contains(@, '$ECS_CLUSTER_NAME')] | [0]" --output text)
if [ "$CLUSTER_ARN" = "None" ] || [ -z "$CLUSTER_ARN" ]; then
  echo "ECS cluster not found, skipping cleanup."
  exit 0
fi

SERVICES=$(aws ecs list-services --cluster "$ECS_CLUSTER_NAME" --query 'serviceArns' --output text)
if [ -n "$SERVICES" ]; then
  for SERVICE in $SERVICES; do
    aws ecs update-service --cluster "$ECS_CLUSTER_NAME" --service "$SERVICE" --desired-count 0
    aws ecs delete-service --cluster "$ECS_CLUSTER_NAME" --service "$SERVICE" --force
  done
fi

aws ecs delete-cluster --cluster "$ECS_CLUSTER_NAME"
