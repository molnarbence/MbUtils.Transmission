namespace MbUtils.Transmission;

public interface ITransmissionClient : IDisposable
{
   Task<GenericRpcResponse> AddTorrentFileAsync(byte[] bytes, string downloadDir);
   Task<GenericRpcResponse> AddTorrentFileAsync(Stream stream, string downloadDir);
   Task<GenericRpcResponse> AddTorrentFileAsync(string base64Content, string downloadDir);
   Task<IEnumerable<NormalizedTorrentInfo>> GetTorrentsAsync();
   Task<GenericRpcResponse> StartTorrentAsync(string id);
   Task<GenericRpcResponse> StopTorrentAsync(string id);
}