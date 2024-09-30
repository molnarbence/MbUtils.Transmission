namespace MbUtils.Transmission;

public interface ITransmissionClient : IDisposable
{
   Task AddTorrentFileAsync(byte[] bytes, string downloadDir);
   Task AddTorrentFileAsync(Stream stream, string downloadDir);
   Task AddTorrentFileAsync(string base64Content, string downloadDir);
   Task<IEnumerable<NormalizedTorrentInfo>> GetTorrentsAsync();
   Task StartTorrentAsync(string id);
   Task StopTorrentAsync(string id);
}