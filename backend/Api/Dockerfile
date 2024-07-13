# SPDX-FileCopyrightText: 2022 Simon Wendel
# SPDX-License-Identifier: GPL-3.0-or-later

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
LABEL \
  stage=intermediate

WORKDIR /src
COPY ["Api/Api.csproj", "Api/"]
RUN dotnet restore "Api/Api.csproj"

COPY . .
WORKDIR "/src/Api"

RUN dotnet build "Api.csproj" -c Release -o /app/build
RUN dotnet publish "Api.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
EXPOSE 80
LABEL \
  stage=final \
  name="push-a-secret-api" \
  description="Push-A-Secret Storage API" \
  maintainer='Simon Wendel <mail@simonwendel.se>'

WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Api.dll"]
