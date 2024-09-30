using System.Net;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Images;

namespace UnitTests;
public class FakeApiFixture : IAsyncLifetime
{
   private IFutureDockerImage _fakeApiImage = default!;
   private IContainer _container = default!;

   public async Task DisposeAsync()
   {
      await _fakeApiImage.DisposeAsync();
      if (_container is not null)
         await _container.DisposeAsync();
   }

   public IContainer Container => _container;

   public async Task InitializeAsync()
   {
      _fakeApiImage = new ImageFromDockerfileBuilder()
         .WithDockerfileDirectory(CommonDirectoryPath.GetSolutionDirectory(), string.Empty)
         .WithDockerfile("FakeApi.Dockerfile")
         .WithCleanUp(true)
         .Build();

      await _fakeApiImage.CreateAsync();

      _container = new ContainerBuilder()
         .WithImage(_fakeApiImage)
         .WithPortBinding(8080, assignRandomHostPort: true)
         .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(r => r.ForPort(8080).ForPath("/").ForStatusCode(HttpStatusCode.OK)))
         .Build();

      await _container.StartAsync();
   }
}
