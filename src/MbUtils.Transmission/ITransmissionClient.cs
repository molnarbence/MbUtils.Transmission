namespace MbUtils.Transmission;

public interface ITransmissionClient : IDisposable
{
   Task<AddTorrentFileResponse> AddTorrentFileAsync(byte[] bytes, string downloadDir);
   Task<AddTorrentFileResponse> AddTorrentFileAsync(Stream stream, string downloadDir);
   Task<AddTorrentFileResponse> AddTorrentFileAsync(string base64Content, string downloadDir);
   Task<IEnumerable<NormalizedTorrentInfo>> GetTorrentsAsync();
   Task<GenericRpcResponse> StartTorrentAsync(string id);
   Task<GenericRpcResponse> StopTorrentAsync(string id);
   Task<GenericRpcResponse> RemoveTorrentAsync(string id, bool deleteLocalData);
}