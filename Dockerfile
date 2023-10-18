FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

ARG ver=1.0

COPY ./src ./

RUN ls -l ./ && dotnet publish \
    -c Release \
    -o /publish \
    -v normal \
    /property:Version=${ver} \
    ./web/Swan.csproj

FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /publish ./

ENV TZ=Asia/Shanghai

CMD ["dotnet", "swan.dll"]