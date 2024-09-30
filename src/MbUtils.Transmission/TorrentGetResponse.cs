namespace MbUtils.Transmission;
internal class TorrentGetResponse
{
   public required TorrentGetResponseArguments Arguments { get; set; }
}

internal class TorrentGetResponseArguments
{
   public required IEnumerable<RawTorrentInfo> Torrents { get; set; }
}