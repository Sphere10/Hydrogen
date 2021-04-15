using System;
using System.Collections.Generic;

namespace Sphere10.Helium.Retry {
	public class RetryManager : IRetryManager {
		public void RetryCount(int totalCount, TimeSpan durationBetweenCounts) {
			throw new NotImplementedException();
		}

		public void RetryCount(IList<RetryCount> retryCountList) {
			throw new NotImplementedException();
		}
	}
}
