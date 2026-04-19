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

required_vars=(
  ENCRYPTION_KEY
  MONGODB_ENCRYPTED_CONNECTION_STRING
  MONGODB_ENCRYPTED_DATABASE_NAME
  SUPABASE_ENCRYPTED_CONNECTION_STRING
  SUPABASE_ENCRYPTED_API_KEY
  JWT_SECRET_KEY
)

for var_name in "${required_vars[@]}"; do
  if [ -z "${!var_name:-}" ]; then
    echo "Error: Required secret is missing or empty: $var_name"
    exit 1
  fi
done

if [ ! -f "$TEMPLATE_FILE" ]; then
  echo "Error: Template file not found at $TEMPLATE_FILE"
  exit 1
fi

# Escape characters that are special in sed replacement strings:
#   & -> replaced by the matched string (must escape)
#   \ -> escape character (must escape)
#   | -> our delimiter (must escape)
escape_sed() {
  printf '%s' "$1" | sed 's/[&\\|]/\\&/g'
}

echo "Generating $OUTPUT_FILE from $TEMPLATE_FILE..."

cp "$TEMPLATE_FILE" "$OUTPUT_FILE"

# Replace all placeholder values using escaped secret values
sed -i.bak \
  -e "s|YOUR_ENCRYPTION_KEY_HERE|$(escape_sed "$ENCRYPTION_KEY")|g" \
  -e "s|YOUR_ENCRYPTED_MONGODB_CONNECTION_STRING|$(escape_sed "$MONGODB_ENCRYPTED_CONNECTION_STRING")|g" \
  -e "s|YOUR_ENCRYPTED_DATABASE_NAME|$(escape_sed "$MONGODB_ENCRYPTED_DATABASE_NAME")|g" \
  -e "s|YOUR_ENCRYPTED_SUPABASE_URL|$(escape_sed "$SUPABASE_ENCRYPTED_CONNECTION_STRING")|g" \
  -e "s|YOUR_ENCRYPTED_SUPABASE_API_KEY|$(escape_sed "$SUPABASE_ENCRYPTED_API_KEY")|g" \
  -e "s|YOUR_JWT_SECRET_KEY_WITH_MORE_LENGTH_32_CHARS|$(escape_sed "$JWT_SECRET_KEY")|g" \
  "$OUTPUT_FILE"

rm -f "$OUTPUT_FILE.bak"

echo "Successfully generated $OUTPUT_FILE"
