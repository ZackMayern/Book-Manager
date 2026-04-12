#!/usr/bin/env bash
set -euo pipefail

cd Back
dotnet restore Back.sln
dotnet build Back.sln --configuration Release --no-restore