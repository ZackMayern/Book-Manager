#!/usr/bin/env bash
set -euo pipefail

# This script generates appsettings.json from appsettings.template.json
# by injecting GitHub Actions secrets via environment variables.
#
# Required environment variables (secrets):
#   - ENCRYPTION_KEY
#   - MONGODB_ENCRYPTED_CONNECTION_STRING
#   - MONGODB_ENCRYPTED_DATABASE_NAME
#   - SUPABASE_ENCRYPTED_CONNECTION_STRING
#   - SUPABASE_ENCRYPTED_API_KEY
#   - JWT_SECRET_KEY

TEMPLATE_FILE="Back/Back.Api/appsettings.template.json"
OUTPUT_FILE="Back/Back.Api/appsettings.json"

if [ ! -f "$TEMPLATE_FILE" ]; then
  echo "Error: Template file not found at $TEMPLATE_FILE"
  exit 1
fi

echo "Generating $OUTPUT_FILE from $TEMPLATE_FILE..."

# Use sed to replace placeholders with environment variables
# Fallback to template values if env vars are not set
cp "$TEMPLATE_FILE" "$OUTPUT_FILE"

# Replace all placeholder values
sed -i.bak \
  -e "s|YOUR_ENCRYPTION_KEY_HERE|${ENCRYPTION_KEY:-YOUR_ENCRYPTION_KEY_HERE}|g" \
  -e "s|YOUR_ENCRYPTED_MONGODB_CONNECTION_STRING|${MONGODB_ENCRYPTED_CONNECTION_STRING:-YOUR_ENCRYPTED_MONGODB_CONNECTION_STRING}|g" \
  -e "s|YOUR_ENCRYPTED_DATABASE_NAME|${MONGODB_ENCRYPTED_DATABASE_NAME:-YOUR_ENCRYPTED_DATABASE_NAME}|g" \
  -e "s|YOUR_ENCRYPTED_SUPABASE_URL|${SUPABASE_ENCRYPTED_CONNECTION_STRING:-YOUR_ENCRYPTED_SUPABASE_URL}|g" \
  -e "s|YOUR_ENCRYPTED_SUPABASE_API_KEY|${SUPABASE_ENCRYPTED_API_KEY:-YOUR_ENCRYPTED_SUPABASE_API_KEY}|g" \
  -e "s|YOUR_JWT_SECRET_KEY_WITH_MORE_LENGTH_32_CHARS|${JWT_SECRET_KEY:-YOUR_JWT_SECRET_KEY_WITH_MORE_LENGTH_32_CHARS}|g" \
  "$OUTPUT_FILE"

rm -f "$OUTPUT_FILE.bak"

echo "Successfully generated $OUTPUT_FILE"
