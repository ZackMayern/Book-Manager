#!/usr/bin/env bash
set -euo pipefail

envsubst < infra/k8s/deployment.yaml | kubectl apply -f -
kubectl apply -f infra/k8s/service.yaml
envsubst < infra/k8s/frontend-deployment.yaml | kubectl apply -f -
kubectl apply -f infra/k8s/frontend-service.yaml
kubectl rollout status deployment/book-manager-api -n book-manager --timeout=180s
kubectl rollout status deployment/book-manager-ui -n book-manager --timeout=180s

echo "Waiting for frontend LoadBalancer external IP/hostname..."
LB_READY=false
for i in $(seq 1 60); do
  LB_HOSTNAME=$(kubectl get svc book-manager-ui-service -n book-manager \
    -o jsonpath='{.status.loadBalancer.ingress[0].hostname}' 2>/dev/null || true)
  LB_IP=$(kubectl get svc book-manager-ui-service -n book-manager \
    -o jsonpath='{.status.loadBalancer.ingress[0].ip}' 2>/dev/null || true)
  ENDPOINT="${LB_HOSTNAME:-$LB_IP}"
  if [ -n "$ENDPOINT" ]; then
    echo "Frontend is reachable at: http://$ENDPOINT"
    LB_READY=true
    break
  fi
  echo "Attempt $i/60: LoadBalancer not ready yet, retrying in 10s..."
  sleep 10
done

if [ "$LB_READY" != "true" ]; then
  echo "Error: Timed out waiting for frontend LoadBalancer external address."
  exit 1
fi
