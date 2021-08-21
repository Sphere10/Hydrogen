using System;

namespace Sphere10.Helium.Queue {
	/// <summary>
	/// IMPORTANT: Every queue has it's own ConfigDto, to allow flexibility to configure every queue independently.
	/// </summary>
	public class PrivateQueueConfigDto {
		public Guid FileId { get; init; }
		public string Path { get; init; }
		public string TempDirPath { get; init; }
		public int MaxItems { get; init; }
		public int MaxStorageSizeBytes { get; init; }
		public int AllocatedMemory { get; init; }
		public int TransactionalPageSizeBytes { get; init; }
		public int ClusterSize { get; init; }
		public int ListingClusterCount { get; init; }
		public int StorageClusterCount { get; init; }
		public int InputQueueReadRatePerMinute { get; init; }
		public string ErrorQueueName { get; init; } = "ErrorQueue";
		public string AuditLogQueueName { get; init; } = "AuditLogQueue";
		public int FileMemoryCacheBytes { get; init; }
		public bool ReadOnly { get; init; }
	}
}