rm *.nupkg
Get-ChildItem -Recurse 'NuGet Libraries/*.csproj' | ForEach-Object { NuGet pack $_ -IncludeReferencedProjects }