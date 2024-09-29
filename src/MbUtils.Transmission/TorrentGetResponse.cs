namespace MbUtils.Transmission;
public class TorrentGetResponse
{
   public required TorrentGetResponseArguments Arguments { get; set; }
}

public class TorrentGetResponseArguments
{
   public required IEnumerable<RawTorrentInfo> Torrents { get; set; }
}