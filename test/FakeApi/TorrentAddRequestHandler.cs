using System.Text;
using System.Text.Json.Serialization;

namespace FakeApi;

public class TorrentAddRequestHandler : IMinimalApiRequestHandler<TorrentAddRequest>
{
   public async Task<IResult> Handle(TorrentAddRequest request, CancellationToken cancellationToken)
   {
      if (request.Arguments.Metainfo != Convert.ToBase64String(Encoding.UTF8.GetBytes("abc123")))
      {
         return Results.BadRequest();
      }

      return request.Arguments.DownloadDir switch
      {
         "/mnt/downloads" => await OwnResults.ResponseFromFileAsync("torrent-add.json"),
         "/mnt/duplicate" => await OwnResults.ResponseFromFileAsync("torrent-add-duplicate.json"),
         _ => Results.BadRequest()
      };
   }
}

public record TorrentAddRequest(TorrentAddRequestArguments Arguments) : IMinimalApiRequest;
public record TorrentAddRequestArguments(string Metainfo, [property: JsonPropertyName("download-dir")] string DownloadDir);
