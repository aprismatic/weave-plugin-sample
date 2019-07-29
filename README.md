# Weave-Plugin-Sample

This repository contains a sample plugin for [WEAVE](https://www.weave.aprismatic.com/) which implements `IDatasetStore` in https://github.com/aprismatic/weave-commons

## Usage
1. Modify `DatasetStore.cs` to match your production environment.
2. Build a docker image with custom plugin from WEAVE's authority base image using `build.ps1`.
3. Start/use WEAVE network as per normal.

## Important Notes
* `UpdateDatasets(byte[] updateKey, byte[] publicKey)` MUST be synchronous, as that will let WEAVE know if the update is completed.
* The package reference to `Weave.Commons` in the `.csproj` MUST include the `PrivateAssets="All"` property.
* Plugin library's filename MUST end with `*Plugin.dll`, as WEAVE searches for libraries that matches that filename.
* Plugin MUST be stored in the `/app/Plugins` folder in the docker image.
