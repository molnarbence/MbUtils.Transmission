
namespace FakeApi;

public record TorrentStopResponse;
public record TorrentStopRequest(TorrentStopRequestArguments Arguments) : IMinimalApiRequest;
public record TorrentStopRequestArguments(string[] Ids);

public class TorrentStopRequestHandler : IMinimalApiRequestHandler<TorrentStopRequest>
{
   public Task<IResult> Handle(TorrentStopRequest request, CancellationToken cancellationToken)
   {
      var selectedResult = request.Arguments.Ids.Contains("test-target-id")
         ? Results.Ok(new RpcResponse())
         : Results.NotFound("Torrent not found");

      return Task.FromResult(selectedResult);
   }
}
