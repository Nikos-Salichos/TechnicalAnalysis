using FluentAssertions;
using NetArchTest.Rules;

namespace TechnicalAnalysis.Tests.ArchitectureTests
{
    [Trait("Category", "Architecture")]
    public class ContractTest
    {
        [Fact]
        public void Contracts_ShouldNotHaveAnyDependency()
        {
            var result = Types.InCurrentDomain()
                               .That().ResideInNamespace(BaseArchitectureSetup.ContractsProject)
                               .And().AreClasses()
                               .Should().NotHaveDependencyOnAny(BaseArchitectureSetup.DomainProject, BaseArchitectureSetup.ApplicationProject,
                               BaseArchitectureSetup.PresentationProject, BaseArchitectureSetup.InfrastructurePersistenceProject,
                               BaseArchitectureSetup.InfrastructureAdaptersProject, BaseArchitectureSetup.InfrastructureHostProject,
                               BaseArchitectureSetup.InfrastructureClientProject)
                               .GetResult();
            result.IsSuccessful.Should().BeTrue();
        }
    }
}
