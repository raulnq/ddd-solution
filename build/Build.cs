using System;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.Docker;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.Docker.DockerTasks;

[CheckBuildProjectConfigurations]
[ShutdownDotNetAfterServerBuild]
class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution] readonly Solution Solution;

    string DockerPrefix = "task-manager";

    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath TestsDirectory => RootDirectory / "tests";
    AbsolutePath OutputDirectory => RootDirectory / "output";
    AbsolutePath PublishDirectory => RootDirectory / "publish";

    string MigratorProject = "TaskManager.Migrator";

    Target Compile => _ => _
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration));
        });

    Target CompileMigrator => _ => _
    .Executes(() =>
    {
        DotNetBuild(s => s
            .SetProjectFile(SourceDirectory / MigratorProject)
            .SetConfiguration(Configuration));
    });

    Target PublishMigrator => _ => _
    .DependsOn(CompileMigrator)
    .Executes(() =>
    {
        DotNetPublish(s => s
            .SetProject(SourceDirectory / MigratorProject)
            .SetConfiguration(Configuration)
            .DisableNoBuild()
            .DisableNoRestore()
            .SetOutput(PublishDirectory / MigratorProject));
    });

    Target RunMigrator => _ => _
    .DependsOn(PublishMigrator)
    .Executes(() =>
    {
        DotNet(PublishDirectory / MigratorProject / $"{MigratorProject}.dll");
    });

    string SQLPort = "1033";
    string SQLPassword = "Sqlserver123$";

    Target RunOrStartSQLServer => _ => _
    .Executes(() =>
    {
        try
        {
            DockerRun(x => x
            .SetName($"{DockerPrefix}-sqlserver")
            .AddEnv("ACCEPT_EULA=Y", $"MSSQL_SA_PASSWORD={SQLPassword}")
            .SetImage("mcr.microsoft.com/mssql/server:2019-CU14-ubuntu-20.04")
            .EnableDetach()
            .SetPublish($"{SQLPort}:1433")
            );
        }
        catch (Exception)
        {
            DockerStart(x => x
            .AddContainers($"{DockerPrefix}-sqlserver")
            );
        }
    });

    Target StopSQLServer => _ => _
    .Executes(() =>
    {
        DockerStop(x => x
        .AddContainers($"{DockerPrefix}-sqlserver")
        );
    });

    Target RemoveSQLServer => _ => _
    .DependsOn(StopSQLServer)
    .Executes(() =>
    {
        DockerRm(x => x
        .AddContainers($"{DockerPrefix}-sqlserver")
        );
    });

    string SEQPort = "5342";

    Target RunOrStartSeq => _ => _
    .Executes(() =>
    {
        try
        {
            DockerRun(x => x
            .SetName($"{DockerPrefix}-seq")
            .AddEnv("ACCEPT_EULA=Y")
            .SetRestart("unless-stopped")
            .SetImage("datalust/seq:latest")
            .EnableDetach()
            .SetPublish($"{SEQPort}:80")
            );
        }
        catch (Exception)
        {
            DockerStart(x => x
            .AddContainers($"{DockerPrefix}-seq")
            );
        }
    });

    Target StopSeq => _ => _
    .Executes(() =>
    {
        DockerStop(x => x
        .AddContainers($"{DockerPrefix}-seq")
        );
    });

    Target RemoveSeq => _ => _
    .DependsOn(StopSeq)
    .Executes(() =>
    {
        DockerRm(x => x
        .AddContainers($"{DockerPrefix}-seq")
        );
    });

    string RabbitMQUser = "admin";
    string RabbitMQPassword = "Rabbitmq123$";
    string RabbitMQAdminPort = "15671";
    string RabbitMQPort = "5671";

    Target RunOrStartRabbitMQ => _ => _
    .Executes(() =>
    {
        try
        {
            DockerRun(x => x
            .SetName($"{DockerPrefix}-rabbitmq")
            .SetHostname($"{DockerPrefix}-host")
            .AddEnv($"RABBITMQ_DEFAULT_USER={RabbitMQUser}", $"RABBITMQ_DEFAULT_PASS={RabbitMQPassword}")
            .SetImage("rabbitmq:3-management")
            .EnableDetach()
            .AddPublish($"{RabbitMQAdminPort}:15672")
            .AddPublish($"{RabbitMQPort}:5672")
            );
        }
        catch (Exception)
        {
            DockerStart(x => x
            .AddContainers($"{DockerPrefix}-rabbitmq")
            );
        }
    });

    Target StopRabbitMQ => _ => _
    .Executes(() =>
    {
        DockerStop(x => x
        .AddContainers($"{DockerPrefix}-rabbitmq")
        );
    });

    Target RemoveRabbitMQ => _ => _
    .DependsOn(StopRabbitMQ)
    .Executes(() =>
    {
        DockerRm(x => x
        .AddContainers($"{DockerPrefix}-rabbitmq")
        );
    });

    Target RunOrStartJaeger => _ => _
    .Executes(() =>
    {
        try
        {
            DockerRun(x => x
            .SetName($"{DockerPrefix}-jaeger")
            .AddEnv($"COLLECTOR_ZIPKIN_HOST_PORT:9411")
            .SetImage("jaegertracing/all-in-one:latest")
            .EnableDetach()
            .AddPublish($"5775:5775/udp")
            .AddPublish($"6831:6831/udp")
            .AddPublish($"6832:6832/udp")
            .AddPublish($"5778:5778")
            .AddPublish($"16686:16686")
            .AddPublish($"14268:14268")
            .AddPublish($"14250:14250")
            .AddPublish($"9411:9411")
            );
        }
        catch (Exception)
        {
            DockerStart(x => x
            .AddContainers($"{DockerPrefix}-jaeger")
            );
        }
    });

    Target StopJaeger => _ => _
    .Executes(() =>
    {
        DockerStop(x => x
        .AddContainers($"{DockerPrefix}-jaeger")
        );
    });

    Target RemoveJaeger => _ => _
    .DependsOn(StopJaeger)
    .Executes(() =>
    {
        DockerRm(x => x
        .AddContainers($"{DockerPrefix}-jaeger")
        );
    });

    Target StartEnv => _ => _
    .DependsOn(RunOrStartSQLServer)
    .DependsOn(RunOrStartSeq)
    .DependsOn(RunOrStartRabbitMQ)
    .DependsOn(RunOrStartJaeger)
    .Executes(() =>
    {
        Serilog.Log.Information("Development env started");
    });

    Target StopEnv => _ => _
    .DependsOn(StopSQLServer)
    .DependsOn(StopSeq)
    .DependsOn(StopRabbitMQ)
    .DependsOn(StopJaeger)
    .Executes(() =>
    {
        Serilog.Log.Information("Development env stoped");
    });

    Target RemoveEnv => _ => _
    .DependsOn(RemoveSQLServer)
    .DependsOn(RemoveSeq)
    .DependsOn(RemoveRabbitMQ)
    .DependsOn(RemoveJaeger)
    .Executes(() =>
    {
        Serilog.Log.Information("Development env removed");
    });

}
