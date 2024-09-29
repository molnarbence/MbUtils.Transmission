using System.Security.Cryptography;
using System.Text;
using FluentAssertions;
using MbUtils.Transmission;

namespace UnitTests;
public sealed class TestsAgainstFakeApi : IDisposable, IClassFixture<FakeApiFixture>
{
   private readonly TransmissionClient _transmissionClient;

   public TestsAgainstFakeApi(FakeApiFixture fakeApiFixture)
   {
      _transmissionClient = TransmissionClientFactory.Create(new TransmissionClientConfiguration
      {
         BaseUrl = new UriBuilder(
            Uri.UriSchemeHttp,
            fakeApiFixture.Container.Hostname,
            fakeApiFixture.Container.GetMappedPublicPort(8080)).ToString(),
         Username = "transmission",
         Password = "password"
      });
   }

   public void Dispose()
   {
      _transmissionClient.Dispose();
   }

   [Fact]
   public async Task Test_GetTorrentsAsync()
   {
      // arrange
      const string targetId = "63e4f145bcb301e6870287e903ef676fe08d4230";
      const string expectedName = "KAPCSOLAT.mkv";

      // act
      var torrents = await _transmissionClient.GetTorrentsAsync();

      // assert
      var target = torrents.First(x => x.Id == targetId);
      target.Name.Should().Be(expectedName);
   }

   [Fact]
   public async Task Test_AddTorrentFileAsync()
   {
      // arrange
      const string content = "abc123";
      var bytes = Encoding.UTF8.GetBytes(content);

      // act
      await _transmissionClient.AddTorrentFileAsync(bytes, "/mnt/downloads");
   }

   [Fact]
   public async Task Test_StartTorrentAsync()
   {
      // arrange
      const string targetId = "63e4f145bcb301e6870287e903ef676fe08d4230";

      // act
      await _transmissionClient.StartTorrentAsync(targetId);
   }
}
