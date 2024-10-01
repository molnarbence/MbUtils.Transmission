namespace MbUtils.Transmission.Internals;
internal record TorrentGetResponse(string Result, TorrentGetResponseArguments Arguments) : RpcResponseBase<TorrentGetResponseArguments>(Result, Arguments);
internal record TorrentGetResponseArguments(IEnumerable<RawTorrentInfo> Torrents);