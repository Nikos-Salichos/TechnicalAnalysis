using FluentAssertions;
using NetArchTest.Rules;
using TechnicalAnalysis.Domain.Interfaces.Application;

namespace TechnicalAnalysis.Tests.ArchitectureTests
{
    [Trait("Category", "Architecture")]
    public class ApplicationTest
    {
        [Fact]
        public void Application_ShouldNotHave_DependencyOnInfrastructure()
        {
            var result = Types.InCurrentDomain()
                               .That().ResideInNamespace(BaseArchitectureSetup.ApplicationProject)
                               .And().AreClasses()
                               .Should().NotHaveDependencyOnAny(BaseArchitectureSetup.InfrastructurePersistenceProject,
                               BaseArchitectureSetup.InfrastructureAdaptersProject, BaseArchitectureSetup.InfrastructureHostProject,
                               BaseArchitectureSetup.InfrastructureClientProject, BaseArchitectureSetup.PresentationProject,
                               BaseArchitectureSetup.CommonModelsProject)
                               .GetResult();
            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Application_ShouldHave_DependencyOnDomain()
        {
            var result = Types.InCurrentDomain()
                       .That().ResideInNamespace(BaseArchitectureSetup.ApplicationProject)
                       .And().AreClasses()
                       .Should().HaveDependencyOn(BaseArchitectureSetup.DomainProject)
                       .GetResult();
            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Services_ShouldImplementIBaseServiceInterface()
        {
            var result = Types.InCurrentDomain()
                         .That().ResideInNamespace(BaseArchitectureSetup.ApplicationProject)
                         .And().AreClasses()
                         .Should().ImplementInterface(typeof(ISyncService))
                         .GetResult();
            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Services_ShouldHaveNameEndingWithService()
        {
            var result = Types.InCurrentDomain()
                         .That().ResideInNamespace(BaseArchitectureSetup.ApplicationProject)
                         .And().AreClasses()
                         .Should().HaveNameEndingWith("Service")
                         .GetResult();
            result.IsSuccessful.Should().BeTrue();
        }
    }
}
