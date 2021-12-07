FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build

# final stage/image
FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine
COPY ./src/ /src/
COPY ./.config/ /.config/
COPY ./paket.dependencies /
COPY ./paket.lock /
RUN ["dotnet","tool","restore"]
RUN ["dotnet","paket","restore"]
RUN ["dotnet", "paket", "generate-load-scripts", "--type", "fsx"]

ENTRYPOINT ["dotnet","fsi","/src/wserver.fsx"]