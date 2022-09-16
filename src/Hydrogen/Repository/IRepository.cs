using System;
using System.Collections.Generic;
using System.Text;

namespace Hydrogen;
public interface IRepository {

	event EventHandlerEx<object> Changing;
	event EventHandlerEx<object> Changed;
	event EventHandlerEx<object> Saving;
	event EventHandlerEx<object> Saved;
	event EventHandlerEx<object> Clearing;
	event EventHandlerEx<object> Cleared;

	event EventHandlerEx<object> Adding;
	event EventHandlerEx<object> Added;

	event EventHandlerEx<object> Updating;
	event EventHandlerEx<object> Updated;


}

public interface IRepository<T> : IRepository {
}
