# Azure Blob Storage: band-image configuration

## Stored data

The database stores only stable blob paths (`BandPhotoBlobPath` and `LogoImageBlobPath`), never SAS URLs. The backend generates a read-only SAS URL when an image must be displayed.

Blobs use paths such as `bands/{bandId}/photo/{fileId}.png` and `bands/{bandId}/logo/{fileId}.jpg`.

## Azure resources

1. Create a Storage Account and a private container, for example `diy-music-community`.
2. Do not enable anonymous public access to the container.
3. For the deployed application, create a Managed Identity and assign the **Storage Blob Data Contributor** role only on that Storage Account or container.
4. Keep a development connection only in User Secrets; use Azure Key Vault in production.

## Local development

From `backend/src/DiyMusicCommunity.Api`, run:

```powershell
dotnet user-secrets set "AzureStorage:ConnectionString" "DefaultEndpointsProtocol=https;AccountName=...;AccountKey=...;EndpointSuffix=core.windows.net"
dotnet user-secrets set "AzureStorage:ContainerName" "diy-music-community"
dotnet user-secrets set "AzureStorage:SasLifetimeDays" "7"
dotnet user-secrets set "FileUpload:MaxImageSizeMb" "5"
dotnet user-secrets set "FileUpload:TemporaryFileLifetimeMinutes" "30"
```

Temporary files are stored under `FileUpload:TemporaryDirectory`. When it is not configured, the backend uses `App_Data/temporary-images`. Files are removed immediately after successful confirmation and when they expire.

## Production with Azure Key Vault

1. Store `AzureStorage--ConnectionString`, `AzureStorage--ContainerName`, `AzureStorage--SasLifetimeDays`, `FileUpload--MaxImageSizeMb`, and `FileUpload--TemporaryFileLifetimeMinutes` as Key Vault secrets.
2. Enable the application's Managed Identity and assign it the **Key Vault Secrets User** role on the vault.
3. Configure `AzureKeyVaultEndpoint`, for example `https://<vault>.vault.azure.net/`. At startup, the API detects this value and loads secrets through `DefaultAzureCredential`: Azure CLI or Visual Studio locally, and Managed Identity in Azure.
4. Do not include secrets in `appsettings*.json`, visible CI variables, repositories, or logs.

## Prepared Live configuration

- Backend: `backend/src/DiyMusicCommunity.Api/appsettings.Live.json`. Replace only the public frontend origin and backend hostname. Do not add secrets to this file.
- Frontend: `frontend/src/environments/environment.live.ts`. It contains the production HTTPS API URL.

Run the backend with `ASPNETCORE_ENVIRONMENT=Live`. In Key Vault, use double hyphens for .NET configuration hierarchy; for example: `ConnectionStrings--DefaultConnection`, `Jwt--Key`, `Jwt--Issuer`, `Jwt--Audience`, `AzureStorage--ConnectionString`, `AzureStorage--ContainerName`, `AzureStorage--SasLifetimeDays`, `FileUpload--MaxImageSizeMb`, and `FileUpload--TemporaryFileLifetimeMinutes`.

Build the Live frontend configuration with `npm run build -- --configuration live`. The GitHub Actions workflows in `.github/workflows/` run the relevant test and build steps before deploying the frontend and API.

## Operational security

- SAS tokens are read-only, have a configurable lifetime, and do not grant write, delete, or list permissions.
- Validation checks file size, empty content, and PNG/JPEG magic bytes; client-provided extensions and MIME types are not trusted.
- Use a separate Storage Account or container for each environment, and rotate keys if a local connection string is used.
