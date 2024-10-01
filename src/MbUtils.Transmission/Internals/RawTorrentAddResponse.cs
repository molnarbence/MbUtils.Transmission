using System.Text.Json.Serialization;

namespace MbUtils.Transmission.Internals;

internal record RawTorrentAddResponse(string Result, RawTorrentAddResponseArguments Arguments) : RpcResponseBase<RawTorrentAddResponseArguments>(Result, Arguments);

internal record RawTorrentAddResponseArguments(
   [property: JsonPropertyName("torrent-added")] RawMinimalTorrentInfo? TorrentAdded,
   [property: JsonPropertyName("torrent-duplicate")] RawMinimalTorrentInfo? TorrentDuplicate);
internal record RawMinimalTorrentInfo(int Id, string HashString, string Name);
