#!/usr/bin/env bash
set -euo pipefail

cd infra/terraform
terraform apply -auto-approve tfplan
