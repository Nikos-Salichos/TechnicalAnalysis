using Ductus.FluentDocker.Model.Compose;
using Ductus.FluentDocker.Services;
using Ductus.FluentDocker.Services.Impl;

namespace TechnicalAnalysis.Tests.IntegrationTests.FluentDocker
{
    public class BuildDocker : DockerComposeTestBase
    {
        protected override ICompositeService Build()
        {
            // var file = Path.Combine(Directory.GetCurrentDirectory(), (TemplateString)"Fixture/docker-compose.yml");

            const string dockerComposeFilePath = "C:\\Users\\Nikos\\source\\repos\\TechnicalAnalysis\\docker-compose-test.yml";

            using var svc = new Ductus.FluentDocker.Builders.Builder()
                .UseContainer()
                .UseCompose()
                .FromFile(dockerComposeFilePath)
                .RemoveOrphans()
                .Build().Start();

            return new DockerComposeCompositeService(DockerHost,
                                                     new DockerComposeConfig
                                                     {
                                                         ComposeFilePath = new List<string> { dockerComposeFilePath },
                                                         ForceRecreate = true,
                                                         RemoveOrphans = true,
                                                         StopOnDispose = true
                                                     });
        }
    }
}
