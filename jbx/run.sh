#! /bin/bash

dotnet restore

dotnet build

dotnet tool install --global dotnet-ef

dotnet ef database update --startup-project ".\jbx.api.chat"

dotnet watch run --project "jbx.api.chat"

#!dotnet watch run --project "jbx.ui.chat"

#!start dotnet watch run --project .\jbx.service.bus

echo "Browser setup"

timeout 5

open  -a "Google Chrome" https://localhost:8283/swagger

#!open  -a "Google Chrome" https://localhost:7070/
