using System.Net;
using System.Net.Http.Json;

namespace MbUtils.Transmission.Internals;

internal sealed class TransmissionClient(HttpClient httpClient) : ITransmissionClient
{
   private static string? _sessionId;

   private readonly IReadOnlyList<string> _listTorrentsFields =
   [
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
      "webseeds"
   ];

   public async Task<IEnumerable<NormalizedTorrentInfo>> GetTorrentsAsync()
   {
      var response = await DoRequestAsync<TorrentGetResponse>("torrent-get",
         new Dictionary<string, object> { { "fields", _listTorrentsFields } });
      return response.Arguments.Torrents.Select(NormalizeTorrentInfo);
   }

   public async Task<GenericRpcResponse> StartTorrentAsync(string id)
   {
      return await DoRequestAsync<GenericRpcResponse>("torrent-start",
         new Dictionary<string, object> { { "ids", new[] { id } } });
   }

   public async Task<GenericRpcResponse> StopTorrentAsync(string id)
   {
      return await DoRequestAsync<GenericRpcResponse>("torrent-stop",
         new Dictionary<string, object> { { "ids", new[] { id } } });
   }

   public async Task<GenericRpcResponse> RemoveTorrentAsync(string id, bool deleteLocalData)
   {
      return await DoRequestAsync<GenericRpcResponse>("torrent-remove",
         new Dictionary<string, object>
         {
            { "ids", new[] { id } }, 
            { "delete-local-data", deleteLocalData }
         });
   }

   public async Task<AddTorrentFileResponse> AddTorrentFileAsync(string base64Content, string downloadDir)
   {
      var response = await DoRequestAsync<RawTorrentAddResponse>("torrent-add",
         new Dictionary<string, object> { { "metainfo", base64Content }, { "download-dir", downloadDir } });
      if (response.Result != "success")
      {
         throw new InvalidOperationException($"Failed to add torrent: {response.Result}");
      }

      if (response.Arguments.TorrentAdded is not null)
      {
         return new AddTorrentFileResponse("added",
            new MinimalTorrentInfo(response.Arguments.TorrentAdded.HashString, response.Arguments.TorrentAdded.Name));
      }

      if (response.Arguments.TorrentDuplicate is not null)
      {
         return new AddTorrentFileResponse("duplicate",
            new MinimalTorrentInfo(response.Arguments.TorrentDuplicate.HashString,
               response.Arguments.TorrentDuplicate.Name));
      }

      throw new InvalidOperationException("Failed to add torrent: unknown error");
   }

   public async Task<AddTorrentFileResponse> AddTorrentFileAsync(byte[] bytes, string downloadDir)
   {
      var base64Content = Convert.ToBase64String(bytes);
      return await AddTorrentFileAsync(base64Content, downloadDir);
   }

   public async Task<AddTorrentFileResponse> AddTorrentFileAsync(Stream stream, string downloadDir)
   {
      using var memoryStream = new MemoryStream();
      await stream.CopyToAsync(memoryStream);
      return await AddTorrentFileAsync(memoryStream.ToArray(), downloadDir);
   }

   public void Dispose()
   {
      httpClient.Dispose();
   }

   private async Task<TResponse> DoRequestAsync<TResponse>(string operation,
      Dictionary<string, object>? arguments = null) where TResponse : class
   {
      // create request
      var request = GetRequestMessage(operation, arguments);

      // send request
      var response = await httpClient.SendAsync(request);

      // check response
      if (response.StatusCode == HttpStatusCode.Conflict)
      {
         if (response.Headers.TryGetValues("x-transmission-session-id", out var values))
         {
            _sessionId = values.FirstOrDefault();
            request = GetRequestMessage(operation, arguments);
            response = await httpClient.SendAsync(request);
         }
      }

      response.EnsureSuccessStatusCode();

      var body = await response.Content.ReadFromJsonAsync<TResponse>();
      return body ?? throw new InvalidOperationException("Failed to deserialize response body");
   }

   private static HttpRequestMessage GetRequestMessage(string method, Dictionary<string, object>? arguments)
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
         raw.HashString,
         raw.Name,
         state,
         raw.LeftUntilDone < 1,
         raw.PercentDone,
         raw.UploadRatio,
         dateAdded,
         dateCompleted,
         raw.Labels.FirstOrDefault(),
         raw.DownloadDir,
         raw.RateUpload,
         raw.RateDownload,
         raw.DownloadDir,
         TimeSpan.FromSeconds(raw.Eta),
         raw.QueuePosition,
         raw.PeersSendingToUs,
         raw.PeersGettingFromUs,
         raw.PeersConnected,
         raw.PeersConnected,
         raw.SizeWhenDone,
         raw.TotalSize,
         raw.UploadedEver,
         raw.DownloadedEver
      );
   }
}