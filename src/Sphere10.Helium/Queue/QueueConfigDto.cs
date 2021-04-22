using System;

namespace Sphere10.Helium.Queue {
	public record QueueConfigDto {
		public Guid FileId { get; init; }
		public string FileName { get; init; }
		public string LocalQueueFilePath { get; init; }
		public string TempQueueFilePath { get; init; }
		public int MaxItems { get; init; }
		public int MaxStorageSizeBytes { get; init; }
		public int AllocatedMemory { get; init; }
		public int TransactionalPageSizeBytes { get; init; }
		public int ClusterSize { get; init; }
		public int ListingClusterCount { get; init; }
		public int StorageClusterCount { get; init; }
		public int InputQueueReadRatePerMinute { get; init; }
		public string ErrorQueueName { get; set; } = "ErrorQueue";
		public string AuditLogQueueName { get; set; } = "AuditLogQueue";
		public int FileMemoryCacheBytes { get; init; }
		public bool ReadOnly { get; init; }
	}
}
