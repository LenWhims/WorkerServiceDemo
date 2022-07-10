using Microsoft.AspNet.SignalR.Hosting;
using Serilog;
using Serilog.Enrichers;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using WorkerServiceDemo;
IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        const string loggerTemplate = @"{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u4}]<{ThreadId}> [{SourceContext:l}] {Message:lj}{NewLine}{Exception}";
        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        var logfile = Path.Combine(baseDir, "App_Data", "logs", "log.txt");
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .Enrich.With(new ThreadIdEnricher())
            .Enrich.FromLogContext()
            //.WriteTo.Console(LogEventLevel.Information, loggerTemplate, theme: AnsiConsoleTheme.Literate)
            .WriteTo.File(logfile, LogEventLevel.Information, loggerTemplate,
                rollingInterval: RollingInterval.Day, retainedFileCountLimit: 90)
            .CreateLogger();
        Log.Information($"Service Starts. Version: {System.Reflection.Assembly.GetEntryAssembly()?.GetName().Version}");
        Log.Information($"Service Directory: {AppDomain.CurrentDomain.BaseDirectory}");
    })
    .ConfigureServices((hostContext, services) =>
    {
        try
        {
            services.AddHostedService<Worker>();
            services.Configure<AppSettings>(hostContext.Configuration.GetSection("AppSettings"));
        }
        catch (Exception e)
        {
            Log.Fatal(e, "Service terminated unexpectedly");
        }

    })
    .UseWindowsService()
    .UseSerilog()
    .Build();
await host.RunAsync();
