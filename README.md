# Weave-Plugin-Sample

This repository contains a sample plugin for [WEAVE](https://www.weave.aprismatic.com/) which implements `IDatasetStore` in https://github.com/aprismatic/weave-commons

## Usage
1. Modify `DatasetStore.cs` to match your production environment.
2. Build a docker image with custom plugin from WEAVE's authority base image using `build.ps1`.
3. Start/use WEAVE network as per normal.

## Important Notes
* `UpdateDatasets(byte[] updateKey, byte[] publicKey)` must be synchronous, as that will let WEAVE know if the update is completed.
* The package reference to `Weave.Commons` in the `.csproj` must include the `PrivateAssets="All"` property.
* The plugin project should be published, as that will include all dependency libraries.
* Plugin's path needs to be specified in the `WEAVE_PLUGIN` environment variable. This can be done in the `Dockerfile` as shown in this sample project, or when running the container.
