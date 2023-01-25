# DDD

## Running locally

Run the following command to prepare the development enviroment:

```
dotnet tool install Nuke.GlobalTool --global
nuke StartEnv
nuke RunMigrator
```

- **Jaeger UI**: http://localhost:16686/
- **Seq**: http://localhost:5342/
- **RabbitMQ**: http://localhost:15671/
  - User: admin
  - Password: Rabbitmq123$
- **SQLServer**: localhost,1033
  - User: sa
  - Password: Sqlserver123$

Open the solution and run the `TaskManager.WebApi` project.


