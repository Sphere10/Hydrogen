using System;
using Sphere10.Framework.Application;

namespace Sphere10.Helium.Queue {
	/// <summary>
	/// IMPORTANT: Every queue has it's own Settings, to allows flexibility to configure every queue independently.
	/// </summary>
	public class RouterQueueSettings : SettingsObject {

		private const string StrGuid = "539DA682-580A-42CA-B275-0143659D0AF7";
		private const string TemporaryQueueName = "Temp_75F40999-C3F1-43D6-93D7-1BEFFE7859F2";
		private const int MessageBatchSize = 40;
		private static readonly string QueueTempDir = System.IO.Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Private");
		private static readonly string QueuePath = System.IO.Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), TemporaryQueueName);

		public Guid FileId { get; } = new(StrGuid);
		public string Path { get; } = QueuePath;
		public string TempDirPath { get; } = QueueTempDir;
		public int MaxItems { get; } = 500;
		public int MaxStorageSizeBytes { get; } = 1 << 21; /*2097152 ~ 2MB*/
		public int AllocatedMemory { get; set; } /*Not used yet.*/
		public int TransactionalPageSizeBytes { get; } = 1 << 17; /*DefaultTransactionalPageSize = 1 << 17; => 132071 ~ 128 KB*/
		public int ClusterSize { get; } = 1 << 9; /*512 B*/
		public int ListingClusterCount { get; set; } = 0;/*Set DEFAULT values for them.*/
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