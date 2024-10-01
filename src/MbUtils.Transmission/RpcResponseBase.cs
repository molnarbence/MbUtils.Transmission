namespace MbUtils.Transmission;
public abstract record RpcResponseBase<TArguments>(string Result, TArguments Arguments);