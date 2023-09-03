using Serilog;
using TechnicalAnalysis.Application.Modules;
using TechnicalAnalysis.Infrastructure.Adapters.Modules;
using TechnicalAnalysis.Infrastructure.Host.Serilog;
using TechnicalAnalysis.Infrastructure.Persistence.Modules;
using TechnicalAnalysis.StartUpService;

IHost host = Host.CreateDefaultBuilder(args)
    .UseSerilog(SerilogRegistration.SerilogConfiguration)
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