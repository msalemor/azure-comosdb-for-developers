# Create a Linux .Net Core Docker Image

## Requirements

This guide is for Linux users, though the steps apply equally in Windows. 
In Visual Studio, you can create docker images easily. For this guide the requirements are that you have installed:

- dotnet CLI
- docker

## Using the dotnet CLI to create and build a solution

The following commands will create a .Net Core solution, two class library projects and a MVC project. 
Once the projects are created, reference are added between the projects. 
Finally, the projects are added to the solution. 
The restore is at the solution level and it will restore all packages for all the projects in the solution.
Finally, the solution is built or published.

> **Note:** There are a lot of examples out on the Internet about publising a single .Net Core project, but this example is more enterpise where the solution has many projects and references.


```bash
# Create a solution
dotnet new sln -o ContosoCrmSolution
cd ContosoCrmSolution
# or dotnet new sln --name ContosoCrmSolution
# Create a Class Library project
dotnet new classlib --name ContosoCrm.Common
# Create a Class Library project
dotnet new classlib --name ContosoCrm.Domain
# Create a MVC project
dotnet new mvc --name ContosoCrm.WebApp
# Add a references to the Domain project
dotnet add ContosoCrm.Domain/ContosoCrm.Domain.csproj reference ContosoCrm.Common/ContosoCrm.Common.csproj
# Add a reference to the Web project
dotnet add ContosoCrm.WebApp/ContosoCrm.WebApp.csproj reference ContosoCrm.Common/ContosoCrm.Common.csproj
dotnet add ContosoCrm.WebApp/ContosoCrm.WebApp.csproj reference ContosoCrm.Domain/ContosoCrm.Domain.csproj
# Add the Class Library to the project
dotnet sln add ContosoCrm.Common/ContosoCrm.Common.csproj
dotnet sln add ContosoCrm.Domain/ContosoCrm.Domain.csproj
# Add the WebApp to the project
dotnet sln add ContosoCrm.WebApp/ContosoCrm.WebApp.csproj

# Restore the packages in the solution
dotnet restore
# build the solution
# Debug
dotnet build ContosoCrmSolution.sln
# Production
dotnet build ContosoCrmSolution.sln --configuration Release
# Publish the solution
# Debug
dotnet publish ContosoCrm.WebApp/ContosoCrm.WebApp.csproj -o ../build-debug
# Production
dotnet publish ContosoCrm.WebApp/ContosoCrm.WebApp.csproj -o ../build-release -c Release

# To run the web app
cd ContosoCrm.WebApp
dotnet run
```

## Docker file to build an image

Add the following commands to a Docker file at the solution level to create a docker image of the ContosoCrm.WebApp:

> **Note:** This is a two step process where solution is first published using a build environment, and then the output it is copied to a runtime environment.

```yaml
FROM microsoft/dotnet:sdk AS build-env
WORKDIR /app

# Copy all the projects and the solution
COPY . ./
# restore all the projects in the solution
RUN dotnet restore

# RUN dotnet publish -c Release -o out
# publish only the MVC app
RUN dotnet publish ContosoCrm.WebApp/ContosoCrm.WebApp.csproj -o out -c Release

# Build runtime image
FROM microsoft/dotnet:aspnetcore-runtime
WORKDIR /app
COPY --from=build-env /app/ContosoCrm.WebApp/out .
ENTRYPOINT ["dotnet", "ContosoCrm.WebApp.dll"]
```

### Build the image

```
docker build -t contosocrmapp:prod-1.0.0 .
```

### Test the image

```
docker run --rm -p 8080:80 --name contosocrm contosocrmapp:prod-1.0.0
```