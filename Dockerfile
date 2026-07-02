FROM mcr.microsoft.com/dotnet/sdk:10.0

WORKDIR /src

ENV DALAMUD_HOME=/dalamud

CMD ["dotnet", "build", "Dalamud.FullscreenCutscenes/Dalamud.FullscreenCutscenes.csproj", "--configuration", "Release", "-p:Platform=x64", "-p:OutputPath=/out/"]
