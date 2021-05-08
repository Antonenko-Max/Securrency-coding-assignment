# Coding assignment application

## Local development

To run tests you need to start local db:

   ```
   docker-compose up
   ```

## Database upgrade

This mode will upgrade the application database schema to the latest version.
To do that the application should use connection string with admin permissions: User Id=Securrency_TDS_Adm.

```
dotnet run Securrency.TDS.Web.csproj --migrate
```
## Entity Framework migrations

Install the migrations tool

```
dotnet tool install --global dotnet-ef
```

## Add a migration 

```
dotnet ef migrations add -o DataLayer\Migrations -p .\Securrency.TDS.Web\Securrency.TDS.Web.csproj <migration-name>
```