using FluentAssertions;
using NetArchTest.Rules;

namespace TechnicalAnalysis.Tests.ArchitectureTests
{
    public class InfrastructurePersistenceTest
    {
        [Fact]
        public void InfrastructurePersistence_ShouldNotHave_DependencyOnAnyProjectExceptDomain()
        {
            var result = Types.InNamespace(typeof(Infrastructure.Persistence.Repositories.RedisRepository).Namespace)
                               .Should().NotHaveDependencyOnAny(BaseArchitectureSetup.ApplicationProject, BaseArchitectureSetup.InfrastructureHostProject)
                               .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Repositories_ShouldHaveNameEndingWithRepository()
        {
            var result = Types.InNamespace(typeof(Infrastructure.Persistence.Repositories.RedisRepository).Namespace)
                                   .Should().HaveNameEndingWith("Repository")
                                   .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void InfrastructureAdapters_ShouldNotHave_DependencyOnAnyProjectExceptDomainAndApplication()
        {
            var result = Types.InNamespace(typeof(Infrastructure.Adapters.Adapters.BinanceAdapter).Namespace)
                               .Should().NotHaveDependencyOnAny(BaseArchitectureSetup.InfrastructureGatewayProject,
                                    BaseArchitectureSetup.InfrastructurePersistenceProject,
                                    BaseArchitectureSetup.InfrastructureHostProject)
                               .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }
    }
}
