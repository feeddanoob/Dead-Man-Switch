del -r ./Assemblies/*

::search all  .csproj and build
for /f "delims=" %%a in ('dir /b /s *.csproj') do (
    echo %%a
    dotnet build "%%a" --configuration Release
)