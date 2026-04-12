# Tools

Developer utilities used by Book-Manager.

## Available tool

### EncryptionUtility

Path: EncryptionUtility

A console app that encrypts and decrypts text values using AES. It is intended to help generate encrypted configuration values used by backend settings.

## Requirements

- .NET SDK 10.0 (see EncryptionUtility/global.json)

## Run the utility

From this folder (Tools):

```bash
cd EncryptionUtility
dotnet restore EncryptionUtility.sln
dotnet run --project EncryptionUtility/EncryptionUtility.csproj
```

The app prompts for plaintext input, prints the encrypted value, and then prints the decrypted value to verify correctness.

## Expected usage

Use this utility to encrypt sensitive values before placing them into backend configuration fields such as:

- MongoDBSettings.EncryptedConnectionString
- MongoDBSettings.EncryptedDatabaseName
- SupabaseSettings.EncryptedConnectionString
- SupabaseSettings.EncryptedApiKey

## Security note

- Do not commit real secrets.
- Treat any generated encrypted values and keys as sensitive.
- If you change the encryption key strategy, ensure backend decryption logic stays compatible.
