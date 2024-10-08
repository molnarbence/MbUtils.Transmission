﻿namespace MbUtils.Transmission.Internals;
internal record RawTorrentInfo(
   int Id,
   string HashString,
   string Name,
   short Status,
   long LeftUntilDone,
   double PercentDone,
   double UploadRatio,
   int AddedDate,
   int DoneDate,
   string[] Labels,
   string DownloadDir,
   double RateUpload,
   double RateDownload,
   long Eta,
   int QueuePosition,
   int PeersSendingToUs,
   int PeersGettingFromUs,
   int PeersConnected,
   long SizeWhenDone,
   long TotalSize,
   long UploadedEver,
   long DownloadedEver
   );
