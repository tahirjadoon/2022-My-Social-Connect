# build environment
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
# working dir
WORKDIR /app
EXPOSE 8080

# copy csproj and restor as distinct layers => inside working directory
COPY *.csproj ./
# restore the dependencies listed
RUN dotnet restore

# copy every thing else and build
COPY . ./
# publish 
RUN dotnet publish -c Release -o out 

# build run time image - sdk is quite large and we do not need that inside our container 
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
# copy every thing from the out to the root of the container 
COPY --from=build-env /app/out .
ENTRYPOINT [ "dotnet", "MSC.Api.dll" ]

