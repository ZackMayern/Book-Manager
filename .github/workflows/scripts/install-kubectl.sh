#!/usr/bin/env bash
set -euo pipefail

KUBECTL_VERSION=v1.30.2
sudo apt-get update
sudo apt-get install -y gettext-base
curl -sSfLo kubectl "https://dl.k8s.io/release/${KUBECTL_VERSION}/bin/linux/amd64/kubectl"
chmod +x kubectl
sudo mv kubectl /usr/local/bin/kubectl
kubectl version --client
