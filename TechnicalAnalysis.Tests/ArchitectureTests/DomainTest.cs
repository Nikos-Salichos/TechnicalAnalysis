using FluentAssertions;
using NetArchTest.Rules;

namespace TechnicalAnalysis.Tests.ArchitectureTests
{
    [Trait("Category", "Architecture")]
    public class DomainTest
    {
        [Fact]
        public void Domain_ShouldNotHaveAnyDependency()
        {
            var result = Types.InCurrentDomain()
                               .That().ResideInNamespace(BaseArchitectureSetup.ContractsProject)
                               .And().AreClasses()
                               .Should().HaveDependenciesOtherThan(BaseArchitectureSetup.InfrastructurePersistenceProject,
                               BaseArchitectureSetup.InfrastructureAdaptersProject, BaseArchitectureSetup.InfrastructureHostProject,
                               BaseArchitectureSetup.InfrastructureClientProject, BaseArchitectureSetup.PresentationProject,
                               BaseArchitectureSetup.ContractsProject)
                               .GetResult();
            result.IsSuccessful.Should().BeTrue();
        }
    }
}
