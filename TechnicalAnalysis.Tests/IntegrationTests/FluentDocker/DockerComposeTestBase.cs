using Ductus.FluentDocker.Services;

namespace TechnicalAnalysis.Tests.IntegrationTests.FluentDocker
{
    public abstract class DockerComposeTestBase : IDisposable
    {
        protected ICompositeService CompositeService;
        protected IHostService? DockerHost;

        public DockerComposeTestBase()
        {
            EnsureDockerHost();

            CompositeService = Build();
            try
            {
                CompositeService.Start();
            }
            catch
            {
                CompositeService.Dispose();
                throw;
            }

            OnContainerInitialized();
        }

        public void Dispose()
        {
            OnContainerTearDown();
            var compositeService = CompositeService;
            CompositeService = null;
            try
            {
                compositeService?.Dispose();
            }
            catch
            {
                // ignored
            }
        }

        protected abstract ICompositeService Build();

        protected virtual void OnContainerTearDown()
        {
        }

        protected virtual void OnContainerInitialized()
        {
        }

        private void EnsureDockerHost()
        {
            if (DockerHost?.State == ServiceRunningState.Running) return;

            var hosts = new Hosts().Discover();
            DockerHost = hosts.FirstOrDefault(x => x.IsNative) ?? hosts.FirstOrDefault(x => x.Name == "default");

            if (DockerHost != null)
            {
                if (DockerHost.State != ServiceRunningState.Running) DockerHost.Start();

                return;
            }

            if (hosts.Count > 0)
            {
                DockerHost = hosts[0];
            }

            if (DockerHost != null)
            {
                return;
            }

            EnsureDockerHost();
        }
    }
}
