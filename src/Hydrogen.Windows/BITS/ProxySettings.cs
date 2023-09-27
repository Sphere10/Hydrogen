// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Windows.BITS;

public class ProxySettings {
	private BG_JOB_PROXY_USAGE proxyUsage;
	private string proxyList;
	private string proxyBypassList;
	private IBackgroundCopyJob job;

	internal ProxySettings(IBackgroundCopyJob job) {
		this.job = job;
		job.GetProxySettings(out proxyUsage, out proxyList, out proxyBypassList);
	}

	public ProxyUsage ProxyUsage {
		get { return (ProxyUsage)this.proxyUsage; }
		set { this.proxyUsage = (BG_JOB_PROXY_USAGE)value; }
	}

	public string ProxyList {
		get { return this.proxyList; }
		set { this.proxyList = value; }
	}

	public string ProxyBypassList {
		get { return this.proxyBypassList; }
		set { this.proxyBypassList = value; }
	}

	public void Update() {
		this.job.SetProxySettings(this.proxyUsage, this.proxyList, this.proxyBypassList);
	}
}
