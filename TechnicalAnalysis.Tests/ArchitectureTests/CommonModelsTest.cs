using FluentAssertions;
using NetArchTest.Rules;

namespace TechnicalAnalysis.Tests.ArchitectureTests
{
    [Trait("Category", "Architecture")]
    public class ContractTest
    {
        [Fact]
        public void CommonModels_ShouldNotHaveAnyDependency()
        {
            var noDependenciesResult = Types.InNamespace(BaseArchitectureSetup.CommonModelsProject)
                .Should()
                .NotHaveDependencyOnAny(BaseArchitectureSetup.InfrastructurePersistenceProject,
                    BaseArchitectureSetup.InfrastructureAdaptersProject,
                    BaseArchitectureSetup.InfrastructureHostProject,
                    BaseArchitectureSetup.InfrastructureClientProject,
                    BaseArchitectureSetup.PresentationProject,
                    BaseArchitectureSetup.DomainProject)
                .GetResult();

            noDependenciesResult.IsSuccessful.Should().BeTrue();
        }
    }
}
