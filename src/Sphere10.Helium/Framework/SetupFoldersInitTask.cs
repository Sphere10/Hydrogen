using System.IO;
using Sphere10.Framework.Application;
using Sphere10.Helium.Queue;

namespace Sphere10.Helium.Framework {
	public class SetupFoldersInitTask : BaseApplicationInitializeTask{

		public override void Initialize() {

			var localQueueSettings = GlobalSettings.Get<LocalQueueSettings>();
			
			if (!Directory.Exists(localQueueSettings.TempDirPath))
				Directory.CreateDirectory(localQueueSettings.TempDirPath);

			if (File.Exists(localQueueSettings.Path))
				File.Delete(localQueueSettings.Path);

			var privateQueueSettings = GlobalSettings.Get<PrivateQueueSettings>();

			if (!Directory.Exists(privateQueueSettings.TempDirPath))
				Directory.CreateDirectory(privateQueueSettings.TempDirPath);

			if (File.Exists(privateQueueSettings.Path))
				File.Delete(privateQueueSettings.Path);

			var routerQueueSettings = GlobalSettings.Get<PrivateQueueSettings>();

			if (!Directory.Exists(routerQueueSettings.TempDirPath))
				Directory.CreateDirectory(routerQueueSettings.TempDirPath);

			if (File.Exists(routerQueueSettings.Path))
				File.Delete(routerQueueSettings.Path);
		}
	}
}