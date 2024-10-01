namespace FakeApi;

public class TorrentGetRequestHandler : IMinimalApiRequestHandler<TorrentGetRequest>
{
   public async Task<IResult> Handle(TorrentGetRequest request, CancellationToken cancellationToken)
   {
      if (!request.Arguments.Fields.Contains("id"))
      {
         return Results.BadRequest("Missing 'id' field in request");
      }
      return await OwnResults.ResponseFromFileAsync("torrent-get.json");
   }
}

public record TorrentGetRequest(TorrentGetRequestArguments Arguments) : IMinimalApiRequest;

public record TorrentGetRequestArguments(string[] Fields);