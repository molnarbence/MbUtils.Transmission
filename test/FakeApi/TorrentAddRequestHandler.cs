
using System.Text.Json.Serialization;

namespace FakeApi;

public class TorrentAddRequestHandler : IMinimalApiRequestHandler<TorrentAddRequest>
{
   public Task<IResult> Handle(TorrentAddRequest request, CancellationToken cancellationToken)
   {
      return Task.FromResult(Results.Ok(new RpcResponse()));
   }
}

public class TorrentAddRequest : IMinimalApiRequest
{
   public required TorrentAddRequestArguments Arguments { get; init; }

}

public class TorrentAddRequestArguments
{
   public required string Metainfo { get; init; }

   [JsonPropertyName("download-dir")]
   public required string DownloadDir { get; init; }
}
