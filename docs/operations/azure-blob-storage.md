# Azure Blob Storage: configuración de imágenes de banda

## Qué se guarda

La base de datos guarda solo rutas estables de blob (`BandPhotoBlobPath` y `LogoImageBlobPath`), nunca una URL SAS. El backend crea una URL SAS de solo lectura cuando debe mostrar una imagen.

Los blobs usan rutas como `bands/{bandId}/photo/{fileId}.png` y `bands/{bandId}/logo/{fileId}.jpg`.

## Recursos de Azure

1. Crea un Storage Account y un contenedor privado, por ejemplo `diy-music-community`.
2. No habilites acceso público anónimo al contenedor.
3. Para la aplicación desplegada, crea una Managed Identity y asígnale el rol **Storage Blob Data Contributor** únicamente sobre ese Storage Account o contenedor.
4. Conserva una conexión de desarrollo local solo en User Secrets; en producción usa Azure Key Vault.

## Desarrollo local

Desde `backend/src/DiyMusicCommunity.Api` ejecuta:

```powershell
dotnet user-secrets set "AzureStorage:ConnectionString" "DefaultEndpointsProtocol=https;AccountName=...;AccountKey=...;EndpointSuffix=core.windows.net"
dotnet user-secrets set "AzureStorage:ContainerName" "diy-music-community"
dotnet user-secrets set "AzureStorage:SasLifetimeDays" "7"
dotnet user-secrets set "FileUpload:MaxImageSizeMb" "5"
dotnet user-secrets set "FileUpload:TemporaryFileLifetimeMinutes" "30"
```

Los archivos temporales se almacenan bajo `FileUpload:TemporaryDirectory`; si no se indica, el backend usa `App_Data/temporary-images`. Se eliminan inmediatamente después de una confirmación satisfactoria y también cuando han expirado.

## Producción con Azure Key Vault

1. Guarda `AzureStorage--ConnectionString`, `AzureStorage--ContainerName`, `AzureStorage--SasLifetimeDays`, `FileUpload--MaxImageSizeMb` y `FileUpload--TemporaryFileLifetimeMinutes` como secretos de Key Vault.
2. Habilita la Managed Identity de la aplicación y concede a esa identidad el rol **Key Vault Secrets User** sobre el vault.
3. Configura `AzureKeyVaultEndpoint` (por ejemplo `https://<vault>.vault.azure.net/`). La API detecta este valor al arrancar y carga los secretos mediante `DefaultAzureCredential`: Azure CLI/Visual Studio en local y Managed Identity en Azure.
4. No incluyas secretos en `appsettings*.json`, variables de CI visibles, repositorios ni logs.

## Seguridad operativa

- SAS exclusivamente de lectura, con duración configurable y sin permisos de escritura, borrado o listado.
- La validación comprueba tamaño, vacío y magic bytes para PNG/JPEG; la extensión y MIME proporcionados por el cliente no son fuente de verdad.
- Usa una cuenta/contenedor distinto por entorno y rota las claves si se usa una cadena de conexión local.
