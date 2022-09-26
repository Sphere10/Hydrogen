using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hydrogen;

public class DisposableResource : Disposable {
	
	protected Disposables Disposables = new Disposables();
	
	protected override async ValueTask FreeManagedResourcesAsync() {
		Disposables.Dispose();
	}

	protected override void FreeManagedResources() {
		Disposables.Dispose();
	}
}

