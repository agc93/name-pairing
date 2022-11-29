#!/usr/bin/env bash
set -e

## install latest .NET 7.0 release
pushd /tmp
wget https://dotnet.microsoft.com/download/dotnet/scripts/v1/dotnet-install.sh
chmod u+x /tmp/dotnet-install.sh
/tmp/dotnet-install.sh --channel 7.0
popd



## publish project to known location for subsequent deployment by Netlify
dotnet publish -c Release -o release ./src/NamePairing/

# add variables to appsettings
jq -n --arg appName "Secret Santa" '{"Customization":{"AppName":$appName,"AppHeader":$appName}}' > ./release/wwwroot/appsettings.Production.json