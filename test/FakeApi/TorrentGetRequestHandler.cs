namespace FakeApi;

public class TorrentGetRequestHandler : IMinimalApiRequestHandler<TorrentGetRequest>
{
   public Task<IResult> Handle(TorrentGetRequest request, CancellationToken cancellationToken)
   {
      if (!request.Arguments.Fields.Contains("id"))
      {
         return Task.FromResult(Results.BadRequest("Missing 'id' field in request"));
      }
      return Task.FromResult(OwnResults.ResponseFromFile("torrent-get.json"));
   }
}

public record TorrentGetRequest(TorrentGetRequestArguments Arguments) : IMinimalApiRequest;

public record TorrentGetRequestArguments(string[] Fields);