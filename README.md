# eShopOnWeb ASP.NET Core Reference Application

This is a ASP.NET 5.0 application, demonstrating a single-process (monolithic), tested to run with in a docker container running on Linux.

The application is currently hosted on Microsoft Azure temporarily until HarshiCorp is set up. 

The application is running using .net 5.0 image, docker container running on Linux.

https://ec2-3-17-156-187.us-east-2.compute.amazonaws.com/

## Running the code

After cloning or downloading the sample you should be able to run it using an In Memory database immediately. The store's home page should look like this:

![eShopOnWeb home page screenshot](https://user-images.githubusercontent.com/782127/88414268-92d83a00-cdaa-11ea-9b4c-db67d95be039.png)

Most of the site's functionality works with just the web application running. However, the site's Admin page relies on Blazor WebAssembly running in the browser, and it must communicate with the server using the site's PublicApi web application. You'll need to also run this project. You can configure Visual Studio to start multiple projects, or just go to the PublicApi folder in a terminal window and run `dotnet run` from there. Note that if you use this approach, you'll need to stop the application manually in order to build the solution (otherwise you'll get file locking errors).

If you wish to use the sample with a persistent database, you will need to run its Entity Framework Core migrations before you will be able to run the app, and update the `ConfigureServices` method in `Startup.cs` (see below).

You can also run the samples in Docker (see below).

### Configuring the sample to use SQL Server

1. Update `Startup.cs`'s `ConfigureDevelopmentServices` method as follows:

    ```csharp
    public void ConfigureDevelopmentServices(IServiceCollection services)
    {
        // use in-memory database
        //ConfigureTestingServices(services);

        // use real database
        ConfigureProductionServices(services);

    }
    ```

1. Ensure your connection strings in `appsettings.json` point to a local SQL Server instance.
1. Ensure the tool EF was already installed. You can find some help [here](https://docs.microsoft.com/ef/core/miscellaneous/cli/dotnet)

    ```
    dotnet tool install --global dotnet-ef
    ```

1. Open a command prompt in the Web folder and execute the following commands:

    ```
    dotnet restore
    dotnet tool restore
    dotnet ef database update -c catalogcontext -p ../Infrastructure/Infrastructure.csproj -s Web.csproj
    dotnet ef database update -c appidentitydbcontext -p ../Infrastructure/Infrastructure.csproj -s Web.csproj
    ```

    These commands will create two separate databases, one for the store's catalog data and shopping cart information, and one for the app's user credentials and identity data.

1. Run the application.

    The first time you run the application, it will **seed both databases** with data such that you should see products in the store, and you should be able to log in using the faademo@karsun-llc.com account.

   
## Running the sample using Docker

You can run the Web sample by running these commands from the root folder (where the .sln file is located):

```
docker-compose build
docker-compose up
```
You should be able to make requests to localhost:5106 for the Web project, and localhost:5200 for the Public API project once these commands complete. If you have any problems, especially with login, try from a new guest or incognito browser instance.

You can also run the applications by using the instructions located in their `Dockerfile` file in the root of each project. Again, run these commands from the root of the solution (where the .sln file is located).

## Connection Strings:
1. Catalog Connection: The catalog data is stored in the database.
"CatalogConnection": "Server=tcp:sql-faa-zero-trust.cb36wyhp22xr.us-east-2.rds.amazonaws.com,1433;Initial Catalog=CatalogDb;Persist Security Info=False;User ID=admin;Password=##########;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;"

2. Identity Connection: The app shipped with this database to support credential management with encrypted/hashed passwords.
"IdentityConnection": "Server=tcp:sql-faa-zero-trust.cb36wyhp22xr.us-east-2.rds.amazonaws.com,1433;Initial Catalog=Identity;Persist Security Info=False;User ID=admin;Password=##########;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;"

## Path to deployment Script used for Azure deployment to Linux containers:
eShopOnWeb-master/src/Infrastructure/Powershell Scripts/FAA_DeploytoACR_Azure.ps1
