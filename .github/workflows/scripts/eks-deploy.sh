#!/usr/bin/env bash
set -euo pipefail

envsubst < infra/k8s/deployment.yaml | kubectl apply -f -
kubectl apply -f infra/k8s/service.yaml
kubectl rollout status deployment/book-manager-api -n book-manager --timeout=180s
