FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
ARG TARGETPLATFORM
ARG BUILDPLATFORM
ARG TARGETARCH
WORKDIR /app

COPY . ./
RUN dotnet restore -a $TARGETARCH

RUN dotnet publish -a $TARGETARCH --no-restore -c Release -o out

FROM --platform=$TARGETPLATFORM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .

ENTRYPOINT ["./hubitat2prom", "--urls=http://0.0.0.0:80"]
