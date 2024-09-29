
namespace FakeApi;

public class TorrentStartRequestHandler : IMinimalApiRequestHandler<TorrentStartRequest>
{
   public Task<IResult> Handle(TorrentStartRequest request, CancellationToken cancellationToken)
   {
      return Task.FromResult(Results.Ok(new RpcResponse()));
   }
}

public class TorrentStartRequest : IMinimalApiRequest
{
   public required TorrentStartRequestArguments Arguments { get; set; }
}

public class TorrentStartRequestArguments
{
   public string[] Ids { get; set; } = [];
}
