namespace MbUtils.Transmission;
public record NormalizedTorrentInfo(
    string Id,
    string Name,
    TorrentState State,
    bool IsCompleted,
    double Progress,
    double Ratio,
    DateTimeOffset DateAdded,
    DateTimeOffset DateCompleted,
    string? Label,
    string SavePath,
    double UploadSpeed,
    double DownloadSpeed,
    string DownloadDirectory,
    TimeSpan Eta,
    int QueuePosition,
    int ConnectedPeers,
    int ConnectedSeeds,
    int TotalPeers,
    int TotalSeeds,
    long TotalSelected,
    long TotalSize,
    long TotalUploaded,
    long TotalDownloaded
    );

public enum TorrentState
{
   Unknown = 0,
   Downloading = 1,
   Seeding = 2,
   Paused = 3,
   Queued = 4,
   Checking = 5,
   Warning = 6,
   Error = 7
}
