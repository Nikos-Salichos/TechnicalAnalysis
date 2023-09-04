using Serilog;
using TechnicalAnalysis.Application.Modules;
using TechnicalAnalysis.Infrastructure.Adapters.Modules;
using TechnicalAnalysis.Infrastructure.Host.Serilog;
using TechnicalAnalysis.Infrastructure.Persistence.Modules;
using TechnicalAnalysis.StartUpService;

IHost host = Host.CreateDefaultBuilder(args)
     .ConfigureAppConfiguration((hostingContext, config) =>
     {
         var env = hostingContext.HostingEnvironment;
         config.AddJsonFile("appsettings.prod.json", optional: true, reloadOnChange: true);
     }).UseSerilog(SerilogRegistration.SerilogConfiguration)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddSingleton(hostContext.Configuration);
        services.AddApplicationModule(hostContext.Configuration);
        services.AddInfrastructurePersistenceModule(hostContext.Configuration);
        services.AddInfrastructureAdapterModule(hostContext.Configuration);
        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();