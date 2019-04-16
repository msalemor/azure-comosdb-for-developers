# Docker .Net Core Image

## Using the dotnet CLI to create and build a solution

The following commands will create a .Net Core solution, a class library project and a MVC project. 
Once the projects are created, a reference is added from the MVC project to the class library. 
Finally, the projects are added to the solution. 
The restore is at the solution level and it will restore all packages for all the projects in the solution.
Finally, the solution is built or published.


```bash
# Create a solution
dotnet new sln -o ContosoCrmSolution
cd ContosoCrmSolution
# or dotnet new sln --name ContosoCrmSolution
# Create a Class Library project
dotnet new classlib --name ContosoCrm.Common
# Create a MVC project
dotnet new mvc --name ContosoCrm.WebApp
# Add a reference from the Class Library to the MVC Project
dotnet add ContosoCrm.WebApp/ContosoCrm.WebApp.csproj reference ContosoCrm.Common/ContosoCrm.Common.csproj
# Add the Class Library to the project
dotnet sln add ContosoCrm.Common/ContosoCrm.Common.csproj
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
```

## Docker file to build

Add the following commands to a Docker file at the solution level to create a docker image with your WebApp.

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