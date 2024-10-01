namespace MbUtils.Transmission;
public record GenericRpcResponse(string Result, Dictionary<string, object> Arguments) : RpcResponseBase<Dictionary<string, object>>(Result, Arguments);
