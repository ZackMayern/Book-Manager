#!/usr/bin/env bash
set -euo pipefail

VPC_ID=$(aws ec2 describe-vpcs \
  --region "$AWS_REGION" \
  --filters "Name=tag:Name,Values=${EKS_CLUSTER_NAME}-vpc" \
  --query 'Vpcs[0].VpcId' \
  --output text)

if aws eks describe-cluster --region "$AWS_REGION" --name "$EKS_CLUSTER_NAME" >/dev/null 2>&1; then
  VPC_ID=$(aws eks describe-cluster \
    --region "$AWS_REGION" \
    --name "$EKS_CLUSTER_NAME" \
    --query 'cluster.resourcesVpcConfig.vpcId' \
    --output text)

  NODEGROUPS=$(aws eks list-nodegroups --region "$AWS_REGION" --cluster-name "$EKS_CLUSTER_NAME" --query 'nodegroups' --output text)
  if [ -n "$NODEGROUPS" ] && [ "$NODEGROUPS" != "None" ]; then
    for NG in $NODEGROUPS; do
      aws eks delete-nodegroup --region "$AWS_REGION" --cluster-name "$EKS_CLUSTER_NAME" --nodegroup-name "$NG"
      aws eks wait nodegroup-deleted --region "$AWS_REGION" --cluster-name "$EKS_CLUSTER_NAME" --nodegroup-name "$NG"
    done
  fi

  aws eks delete-cluster --region "$AWS_REGION" --name "$EKS_CLUSTER_NAME"
  aws eks wait cluster-deleted --region "$AWS_REGION" --name "$EKS_CLUSTER_NAME"
else
  echo "EKS cluster not found, skipping fallback EKS cleanup."
fi

if [ -n "$VPC_ID" ] && [ "$VPC_ID" != "None" ]; then
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
fi

if aws ecr describe-repositories --region "$AWS_REGION" --repository-names "$ECR_REPOSITORY_NAME" >/dev/null 2>&1; then
  aws ecr delete-repository --region "$AWS_REGION" --repository-name "$ECR_REPOSITORY_NAME" --force
else
  echo "ECR repository not found, skipping fallback ECR cleanup."
fi
