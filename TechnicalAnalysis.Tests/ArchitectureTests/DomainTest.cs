using FluentAssertions;
using NetArchTest.Rules;

namespace TechnicalAnalysis.Tests.ArchitectureTests
{
    [Trait("Category", "Architecture")]
    public class DomainTest
    {
        [Fact]
        public void Domain_ShouldNotHaveAnyDependency_OtherThanCommonProject()
        {
            var noDependenciesResult = Types.InNamespace(BaseArchitectureSetup.DomainProject)
                .That()
                .AreClasses()
                .Should()
                .NotHaveDependencyOnAny(BaseArchitectureSetup.InfrastructurePersistenceProject,
                    BaseArchitectureSetup.InfrastructureAdaptersProject,
                    BaseArchitectureSetup.InfrastructureHostProject,
                    BaseArchitectureSetup.InfrastructureClientProject,
                    BaseArchitectureSetup.PresentationProject)
                .GetResult();

            var dependency = Types.InNamespace(BaseArchitectureSetup.DomainProjectEntities)
                              .Should()
                              .HaveDependencyOn(BaseArchitectureSetup.CommonModelsProject)
                              .GetResult();

            noDependenciesResult.IsSuccessful.Should().BeTrue();
            dependency.IsSuccessful.Should().BeTrue();
        }
    }
}
