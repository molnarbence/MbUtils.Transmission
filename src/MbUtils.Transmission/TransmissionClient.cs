using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace MbUtils.Transmission;

public sealed class TransmissionClient(HttpClient httpClient) : IDisposable
{
   private readonly HttpClient _httpClient = httpClient;
   private string? _sessionId;

   public readonly IReadOnlyList<string> _listTorrentsFields = [
      "id",
      "addedDate",
      "creator",
      "doneDate",
      "comment",
      "name",
      "totalSize",
      "error",
      "errorString",
      "eta",
      "etaIdle",
      "isFinished",
      "isStalled",
      "isPrivate",
      "files",
      "fileStats",
      "hashString",
      "leftUntilDone",
      "metadataPercentComplete",
      "peers",
      "peersFrom",
      "peersConnected",
      "peersGettingFromUs",
      "peersSendingToUs",
      "percentDone",
      "queuePosition",
      "rateDownload",
      "rateUpload",
      "secondsDownloading",
      "secondsSeeding",
      "recheckProgress",
      "seedRatioMode",
      "seedRatioLimit",
      "seedIdleLimit",
      "sizeWhenDone",
      "status",
      "trackers",
      "downloadDir",
      "downloadLimit",
      "downloadLimited",
      "uploadedEver",
      "downloadedEver",
      "corruptEver",
      "uploadRatio",
      "webseedsSendingToUs",
      "haveUnchecked",
      "haveValid",
      "honorsSessionLimits",
      "manualAnnounceTime",
      "activityDate",
      "desiredAvailable",
      "labels",
      "magnetLink",
      "maxConnectedPeers",
      "peer-limit",
      "priorities",
      "wanted",
      "webseeds",
      ];

   public async Task<IEnumerable<NormalizedTorrentInfo>> GetTorrentsAsync()
   {
      var response = await DoRequestAsync<TorrentGetResponse>("torrent-get", new() { { "fields", _listTorrentsFields } });

      return response?.Arguments.Torrents.Select(NormalizeTorrentInfo) ?? [];
   }

   public async Task StartTorrentAsync(string id)
   {
      await DoRequestAsync<object>("torrent-start", new() { { "ids", new[] { id } } });
   }

   public async Task StopTorrentAsync(string id)
   {
      await DoRequestAsync<object>("torrent-stop", new() { { "ids", new[] { id } } });
   }

   public async Task AddTorrentFileAsync(string base64Content, string downloadDir)
   {
      await DoRequestAsync<object>("torrent-add", new() { { "metainfo", base64Content }, { "download-dir", downloadDir } });
   }

   public async Task AddTorrentFileAsync(byte[] bytes, string downloadDir)
   {
      var base64Content = Convert.ToBase64String(bytes);
      await AddTorrentFileAsync(base64Content, downloadDir);
   }

   public async Task AddTorrentFileAsync(Stream stream, string downloadDir)
   {
      using var memoryStream = new MemoryStream();
      await stream.CopyToAsync(memoryStream);
      await AddTorrentFileAsync(memoryStream.ToArray(), downloadDir);
   }

   private async Task<TResponse?> DoRequestAsync<TResponse>(string operation, Dictionary<string, object>? arguments = null) where TResponse : class
   {
      // create request
      var request = GetRequestMessage(operation, arguments);

      // send request
      var response = await _httpClient.SendAsync(request);

      // check response
      if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
      {
         if (response.Headers.TryGetValues("x-transmission-session-id", out var values))
         {
            _sessionId = values.FirstOrDefault();
            request = GetRequestMessage(operation, arguments);
            response = await _httpClient.SendAsync(request);
         }
      }
      response.EnsureSuccessStatusCode();

      return await response.Content.ReadFromJsonAsync<TResponse>();
   }

   private HttpRequestMessage GetRequestMessage(string method, Dictionary<string, object>? arguments)
   {  
      var request = new HttpRequestMessage(HttpMethod.Post, "/transmission/rpc");

      if (_sessionId.HasValue())
      {
         request.Headers.Add("x-transmission-session-id", _sessionId);
      }

      var content = JsonContent.Create(new { method, arguments });
      
      request.Content = content;

      return request;
   }

   private static NormalizedTorrentInfo NormalizeTorrentInfo(RawTorrentInfo raw)
   {
      var dateAdded = DateTimeOffset.FromUnixTimeSeconds(raw.AddedDate);
      var dateCompleted = DateTimeOffset.FromUnixTimeSeconds(raw.DoneDate);

      var state = raw.Status switch
      {
         6 => TorrentState.Seeding,
         4 => TorrentState.Downloading,
         0 => TorrentState.Paused,
         2 => TorrentState.Checking,
         3 or 5 => TorrentState.Queued,
         _ => TorrentState.Unknown
      };

      return new NormalizedTorrentInfo(
         Id: raw.HashString,
         Name: raw.Name,
         State: state,
         IsCompleted: raw.LeftUntilDone < 1,
         Progress: raw.PercentDone,
         Ratio: raw.UploadRatio,
         DateAdded: dateAdded,
         DateCompleted: dateCompleted,
         Label: raw.Labels.FirstOrDefault(),
         SavePath: raw.DownloadDir,
         UploadSpeed: raw.RateUpload,
         DownloadSpeed: raw.RateDownload,
         Eta: TimeSpan.FromSeconds(raw.Eta),
         QueuePosition: raw.QueuePosition,
         ConnectedPeers: raw.PeersSendingToUs,
         ConnectedSeeds: raw.PeersGettingFromUs,
         TotalPeers: raw.PeersConnected,
         TotalSeeds: raw.PeersConnected,
         TotalSelected: raw.SizeWhenDone,
         TotalSize: raw.TotalSize,
         TotalUploaded: raw.UploadedEver,
         TotalDownloaded: raw.DownloadedEver
         );
   }

   public void Dispose()
   {
      _httpClient.Dispose();
   }
}
