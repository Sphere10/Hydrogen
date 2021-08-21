using System;

namespace Sphere10.Helium.Queue {
	/// <summary>
	/// IMPORTANT: Every queue has it's own ConfigDto, to allow flexibility to configure every queue independently.
	/// </summary>
	public class LocalQueueConfigDto {

		private const string StrGuid = "56B43C84-043B-4C9D-9013-3231B3E6E453";
		private const string TemporaryQueueName = "Temp_0405D43D-2C38-4174-BEEC-CD497DAA3E46";
		private const int MessageBatchSize = 40;
		private static readonly string QueueTempDir = System.IO.Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Local");
		private static readonly string QueuePath = System.IO.Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), TemporaryQueueName);

		public Guid FileId { get; } = new(StrGuid);
		public string Path { get; } = QueuePath;
		public string TempDirPath { get; } = QueueTempDir;
		public int MaxItems { get; } = 500;
		public int MaxStorageSizeBytes { get; } = 1 << 21; /*2097152 ~ 2MB*/
		public int AllocatedMemory { get; set; } /*Not used yet.*/
		public int TransactionalPageSizeBytes { get; } = 1 << 17; /*DefaultTransactionalPageSize = 1 << 17; => 132071 ~ 128 KB*/
		public int ClusterSize { get; } = 1 << 9; /*512 B*/
		public int ListingClusterCount { get; set; } /*Not used yet.*/
		public int StorageClusterCount { get; set; } /*Not used yet.*/
		public int InputQueueReadRatePerMinute { get; set; } /*Not used yet.*/
		public string ErrorQueueName { get; set; } /*Not used yet.*/
		public string AuditLogQueueName { get; set; } /*Not used yet.*/
		public int FileMemoryCacheBytes { get; } = 1 << 20; /*1048576 ~ 1MB*/
		public bool ReadOnly { get; } = false;
		public string TempQueueName { get; } = TemporaryQueueName;
		public int BatchSize { get; } = MessageBatchSize;
	}
}