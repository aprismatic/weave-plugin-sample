Push-Location $PSScriptRoot

dotnet publish -c Release -o ./bin/out ./WeaveCustomPlugin/WeaveCustomPlugin.csproj

docker build -t aprismatic.azurecr.io/weave-authority-custom ./WeaveCustomPlugin/.

Pop-Location
