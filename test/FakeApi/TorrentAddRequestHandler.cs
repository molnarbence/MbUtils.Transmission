using System.Text;
using System.Text.Json.Serialization;

namespace FakeApi;

public class TorrentAddRequestHandler : IMinimalApiRequestHandler<TorrentAddRequest>
{
   public Task<IResult> Handle(TorrentAddRequest request, CancellationToken cancellationToken)
   {
      var selectedResult = 
         request.Arguments.DownloadDir == "/mnt/downloads" 
         && request.Arguments.Metainfo == Convert.ToBase64String(Encoding.UTF8.GetBytes("abc123"))
         ? Results.Ok(new RpcResponse())
         : Results.BadRequest();
      return Task.FromResult(selectedResult);
   }
}

public record TorrentAddRequest(TorrentAddRequestArguments Arguments) : IMinimalApiRequest;
public record TorrentAddRequestArguments(string Metainfo, [property: JsonPropertyName("download-dir")] string DownloadDir);
