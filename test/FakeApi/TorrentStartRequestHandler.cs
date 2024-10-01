namespace FakeApi;

public record TorrentStartRequest(TorrentStartRequestArguments Arguments) : IMinimalApiRequest;
public record TorrentStartRequestArguments(string[] Ids);

public class TorrentStartRequestHandler : IMinimalApiRequestHandler<TorrentStartRequest>
{
   public Task<IResult> Handle(TorrentStartRequest request, CancellationToken cancellationToken)
   {
      var selectedResult = request.Arguments.Ids.Contains("test-target-id") 
         ? Results.Ok(new RpcResponse()) 
         : Results.NotFound("Torrent not found");
      return Task.FromResult(selectedResult);
   }
}
