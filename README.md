# Clean .NET Web API Solution

This is .NET Web API solution template using Minimal API design with vertical slices. It includes out of the box authentication endpoints for logging in using "Magic link".

Since we are using vertical slices, solution contains only one project (REST API).
The point of this template is simplicity so we don't have any complicated or overengineered layers.

Docker compose is included to run containers. Containers are running API itself, local database (SQL Server), SEQ for logging and optionally RabbitMQ message broker.

## Getting started

Prerequisites: .NET 8 SDK or higher and Docker installed (you can still install this template with lower version of .NET but you have to manually edit .csproj files and downgrade specific packages that are installed by default)

Run `dotnet new install TH.MinimalApiTemplate` to install the solution template.

### Create new project with Visual Studio 2022

* Open Visual Studio and in "Create New Project" window search for ".NET Minimal API" (should be marked as "new" if template was just installed)
* Give your project a name and make sure "Place solution and project in the same directory" is checked
* In next window select data access you want to use (Entity Framework or Dapper) and if you want to use RabbitMQ message broker
    - Template conditionaly pulls code, nuget packages and configuration based on your choice

### Create new project with dotnet CLI

* In folder in which you want to create your project run `dotnet run min-api`
    - add `-o "{ProjectName}"` parameter to name your project
    - add `--dataaccess` or `-dba` parameter with either "Entity Framework" or "Dapper" value to choose database (Entity Framework is default if parameter is not provided)
    - add `--rabbitmq` or `-rmq`parameter if you want to use RabbitMQ message broker


Project/Namespaces in solution will be named based on name you entered (through Visual Studio or through -o parameter in CLI). Project name should be in format {NameYouEntered}.REST.

### Setup before running

After project is created you can modify code, settings, configuration etc. to your needs, but this setup is just to get default solution up and running.

Set "docker-compose" project as startup project and you can run project using docker-compose command and it should open swagger page when it starts.

NOTE: appsettings.json file with some default settings is provided with the template, if you plan to use it for storing sensitive information like secrets, api keys etc. for your local environment, you'll wanna put appsetting.json file in .gitignore

### Project structure

* Common folder
    - Any common stuff that will be used throughout application (interfaces for services, endpoint filters, exceptions etc.)
* Entities and Enum folders are pretty much self explanatory. You can structure your entities in DDD style of how ever you prefer.
* Features folder
    - Main folder for adding new features. Inside feature folder there should be "feature group" folder (for example "auth") inside which are specific features/endpoints. Inside feature group folder are services that are used throughout this feature group (fox example JWT for auth) and inside specific feature folder are files that define minimal api endpoint and it's business logic.
* Persistance folder
    - Database related stuff. If entity framework is used here is where DbContext resides. If dapper is used then here we put .sql migration scripts that are applied with db-up tool.
* Services folder
    - Any other service classes or most likely 3rd party integrations go here
* MessageBroker folder
    - If on project creation we decide to use RabbitMQ this folder is created which holds classes and configuration for publisher, consumers and messages.

This structure is just a suggestion so feel free to modify it based on your needs.

## Troubleshooting


Sometimes some errors might occurr which prevent api/containers to start. It can be because of missing first migration, appsetting.json options beeing incorrect, docker caching or any other reason.

I found that most of the time some (or most likely all) of the following things help:
* delete and restart docker containers
* close solution and delete .vs, bin and obj folders
* delete docker volumes used by this project and allow them to be recreated