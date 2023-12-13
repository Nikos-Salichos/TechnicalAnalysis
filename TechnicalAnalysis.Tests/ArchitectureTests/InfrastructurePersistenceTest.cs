using FluentAssertions;
using NetArchTest.Rules;

namespace TechnicalAnalysis.Tests.ArchitectureTests
{
    public class InfrastructurePersistenceTest
    {
        [Fact]
        public void Infrastructure_ShouldNotHave_DependencyOnAnyProjectExceptDomain()
        {
            var result = Types.InNamespace(typeof(Infrastructure.Persistence.Repositories.RedisRepository).Namespace)
                               .Should().NotHaveDependencyOnAny(BaseArchitectureSetup.ApplicationProject, BaseArchitectureSetup.PresentationProject)
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
    }
}
