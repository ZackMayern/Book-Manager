#!/usr/bin/env bash
set -euo pipefail

VPC_ID=""

if aws eks describe-cluster --region "$AWS_REGION" --name "$EKS_CLUSTER_NAME" >/dev/null 2>&1; then
  VPC_ID=$(aws eks describe-cluster \
    --region "$AWS_REGION" \
    --name "$EKS_CLUSTER_NAME" \
    --query 'cluster.resourcesVpcConfig.vpcId' \
    --output text)

  aws eks update-kubeconfig --region "$AWS_REGION" --name "$EKS_CLUSTER_NAME"

  kubectl delete service book-manager-ui-service -n book-manager --ignore-not-found=true || true
  kubectl delete service book-manager-api-service -n book-manager --ignore-not-found=true || true
  kubectl delete namespace book-manager --ignore-not-found=true --wait=true --timeout=300s || true
else
  echo "EKS cluster not found, skipping Kubernetes resource cleanup."
fi

if [ -z "$VPC_ID" ] || [ "$VPC_ID" = "None" ]; then
  VPC_ID=$(aws ec2 describe-vpcs \
    --region "$AWS_REGION" \
    --filters "Name=tag:Name,Values=${EKS_CLUSTER_NAME}-vpc" \
    --query 'Vpcs[0].VpcId' \
    --output text)
fi

if [ -z "$VPC_ID" ] || [ "$VPC_ID" = "None" ]; then
  echo "No matching VPC found, skipping ELB cleanup."
  exit 0
fi

ELBV2_ARNS=$(aws elbv2 describe-load-balancers \
  --region "$AWS_REGION" \
  --query "LoadBalancers[?VpcId=='$VPC_ID'].LoadBalancerArn" \
  --output text)

if [ -n "$ELBV2_ARNS" ] && [ "$ELBV2_ARNS" != "None" ]; then
  for ARN in $ELBV2_ARNS; do
    aws elbv2 delete-load-balancer --region "$AWS_REGION" --load-balancer-arn "$ARN"
  done
fi

CLASSIC_LBS=$(aws elb describe-load-balancers \
  --region "$AWS_REGION" \
  --query "LoadBalancerDescriptions[?VPCId=='$VPC_ID'].LoadBalancerName" \
  --output text)

if [ -n "$CLASSIC_LBS" ] && [ "$CLASSIC_LBS" != "None" ]; then
  for LB_NAME in $CLASSIC_LBS; do
    aws elb delete-load-balancer --region "$AWS_REGION" --load-balancer-name "$LB_NAME"
  done
fi