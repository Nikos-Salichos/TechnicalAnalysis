using FluentAssertions;
using NetArchTest.Rules;
using TechnicalAnalysis.Domain.Interfaces.Infrastructure;

namespace TechnicalAnalysis.Tests.ArchitectureTests
{
    [Trait("Category", "Architecture")]
    public class InfrastructurePersistenceTest
    {
        [Fact]
        public void Infrastructure_ShouldHave_DependencyOnDomain()
        {
            var result = Types.InCurrentDomain()
                       .That().ResideInNamespace(BaseArchitectureSetup.InfrastructurePersistenceProject)
                       .And().AreClasses()
                       .Should().HaveDependencyOn(BaseArchitectureSetup.DomainProject)
                       .GetResult();
            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Infrastructure_ShouldNotHave_DependencyOnAnyProjectExceptDomain()
        {
            var result = Types.InCurrentDomain()
                               .That().ResideInNamespace(BaseArchitectureSetup.InfrastructurePersistenceProject)
                               .And().AreClasses()
                               .Should().NotHaveDependencyOnAny(BaseArchitectureSetup.ApplicationProject, BaseArchitectureSetup.PresentationProject, BaseArchitectureSetup.CommonModelsProject)
                               .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Repositories_ShouldImplementIRepositoryInterface()
        {
            var result = Types.InCurrentDomain()
                         .That().ResideInNamespace(BaseArchitectureSetup.InfrastructurePersistenceProject)
                         .And().AreClasses()
                         .Should().ImplementInterface(typeof(IPostgreSqlRepository))
                         .GetResult();
            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Repositories_ShouldHaveNameEndingWithRepository()
        {
            var result = Types.InCurrentDomain()
                                   .That().ResideInNamespace(BaseArchitectureSetup.InfrastructurePersistenceProject)
                                   .And().AreClasses()
                                   .Should().HaveNameEndingWith("Repository")
                                   .GetResult();
            result.IsSuccessful.Should().BeTrue();
        }
    }
}
