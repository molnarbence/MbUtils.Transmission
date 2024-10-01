using System.Text;
using FluentAssertions;
using MbUtils.Transmission;

namespace UnitTests;
public sealed class TestsAgainstFakeApi : IDisposable, IClassFixture<FakeApiFixture>
{
   private readonly ITransmissionClient _transmissionClient;

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
   public async Task Test_When_non_duplicate_AddTorrentFileAsync()
   {
      // arrange
      const string content = "abc123";
      var bytes = Encoding.UTF8.GetBytes(content);

      // act
      var response = await _transmissionClient.AddTorrentFileAsync(bytes, "/mnt/downloads");

      // assert
      response.Result.Should().Be("added");
      response.TorrentInfo.Id.Should().Be("def456");
      response.TorrentInfo.Name.Should().Be("Test torrent 101");
   }

   [Fact]
   public async Task Test_When_duplicate_AddTorrentFileAsync()
   {
      // arrange
      const string content = "abc123";
      var bytes = Encoding.UTF8.GetBytes(content);

      // act
      var response = await _transmissionClient.AddTorrentFileAsync(bytes, "/mnt/duplicate");

      // assert
      response.Result.Should().Be("duplicate");
      response.TorrentInfo.Id.Should().Be("ghi789");
      response.TorrentInfo.Name.Should().Be("Test torrent duplicate");
   }

   [Fact]
   public async Task Test_StartTorrentAsync()
   {
      // arrange
      const string targetId = "test-target-id";

      // act
      var response = await _transmissionClient.StartTorrentAsync(targetId);

      // assert
      response.Result.Should().Be("success");
   }

   [Fact]
   public async Task Test_StopTorrentAsync()
   {
      // arrange
      const string targetId = "test-target-id";

      // act
      var response = await _transmissionClient.StopTorrentAsync(targetId);

      // assert
      response.Result.Should().Be("success");
   }
}
