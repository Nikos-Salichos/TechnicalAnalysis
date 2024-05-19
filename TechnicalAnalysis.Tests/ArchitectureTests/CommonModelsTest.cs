using FluentAssertions;
using NetArchTest.Rules;

namespace TechnicalAnalysis.Tests.ArchitectureTests
{
    public class ContractTest
    {
        [Fact]
        public void CommonModels_ShouldNotHaveAnyDependency()
        {
            var result = Types.InNamespace(typeof(CommonModels.BaseClasses.BaseEntity).Namespace)
                .Should()
                .NotHaveDependencyOnAny(BaseArchitectureSetup.InfrastructurePersistenceProject,
                    BaseArchitectureSetup.InfrastructureAdaptersProject,
                    BaseArchitectureSetup.InfrastructureHostProject,
                    BaseArchitectureSetup.InfrastructureGatewayProject,
                    BaseArchitectureSetup.DomainProject)
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }
    }
}
