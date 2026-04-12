#!/usr/bin/env bash
set -euo pipefail

TERRAFORM_VERSION=1.8.5
curl -sSfLo /tmp/terraform.zip "https://releases.hashicorp.com/terraform/${TERRAFORM_VERSION}/terraform_${TERRAFORM_VERSION}_linux_amd64.zip"
unzip -o /tmp/terraform.zip -d /tmp
sudo mv /tmp/terraform /usr/local/bin/terraform
terraform -version
