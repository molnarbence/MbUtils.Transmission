namespace FakeApi;

public class RpcResponse
{
   public string Result { get; init; } = "success";
   public Dictionary<string, object> Arguments { get; init; } = [];
}
