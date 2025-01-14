using System.Text.Json.Serialization;

namespace FakeApi;

public record TorrentRemoveRequest(TorrentRemoveRequestArguments Arguments) : IMinimalApiRequest;

public record TorrentRemoveRequestArguments(string[] Ids, [property: JsonPropertyName("delete-local-data")] bool DeleteLocalData);

public class TorrentRemoveRequestHandler : IMinimalApiRequestHandler<TorrentRemoveRequest>
{
   public async Task<IResult> Handle(TorrentRemoveRequest request, CancellationToken cancellationToken)
   {
      
      var selectedResult = request.Arguments.Ids.Contains("test-target-id") && request.Arguments.DeleteLocalData
         ? await OwnResults.ResponseFromFileAsync("torrent-remove-response.json")
         : Results.NotFound("Torrent not found");
      return selectedResult;
   }
}