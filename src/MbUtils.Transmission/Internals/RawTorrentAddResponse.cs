namespace MbUtils.Transmission.Internals;

internal record RawTorrentAddResponse(string Result, RawTorrentAddResponseArguments Arguments) : RpcResponseBase<RawTorrentAddResponseArguments>(Result, Arguments);

internal record RawTorrentAddResponseArguments(int Id, string HashString, string Name);
