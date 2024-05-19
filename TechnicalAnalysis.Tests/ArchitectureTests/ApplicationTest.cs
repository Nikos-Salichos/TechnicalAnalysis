using FluentAssertions;
using NetArchTest.Rules;

namespace TechnicalAnalysis.Tests.ArchitectureTests
{
    public class ApplicationTest
    {
        [Fact]
        public void Application_ShouldNotHave_DependencyOnInfrastructure()
        {
            var result = Types.InNamespace(typeof(TechnicalAnalysis.Application.Modules.ApplicationModule).Namespace)
                                .Should().NotHaveDependencyOnAny(BaseArchitectureSetup.InfrastructurePersistenceProject,
                                BaseArchitectureSetup.InfrastructureAdaptersProject, BaseArchitectureSetup.InfrastructureHostProject)
                                .GetResult();
            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Services_ShouldHaveNameEndingWithService()
        {
            var result = Types.InNamespace(typeof(TechnicalAnalysis.Application.Services.AnalysisService).Namespace)
                         .That().ResideInNamespace(BaseArchitectureSetup.ApplicationProject)
                         .And().AreClasses()
                         .Should().HaveNameEndingWith("Service")
                         .GetResult();
            result.IsSuccessful.Should().BeTrue();
        }
    }
}
