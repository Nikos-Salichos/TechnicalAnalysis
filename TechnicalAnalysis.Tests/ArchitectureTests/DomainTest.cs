using FluentAssertions;
using NetArchTest.Rules;

namespace TechnicalAnalysis.Tests.ArchitectureTests
{
    public class DomainTest
    {
        [Fact]
        public void Domain_ShouldNotHaveAnyDependency_OtherThanCommonProject()
        {
            var result = Types.InNamespace(typeof(Domain.Entities.Candlestick).Namespace)
                .That()
                .AreClasses()
                .Should()
                .NotHaveDependencyOnAny(BaseArchitectureSetup.InfrastructurePersistenceProject,
                    BaseArchitectureSetup.InfrastructureAdaptersProject,
                    BaseArchitectureSetup.InfrastructureHostProject,
                    BaseArchitectureSetup.InfrastructureClientProject,
                    BaseArchitectureSetup.PresentationProject)
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }
    }
}
