using System.ComponentModel;

namespace Hydrogen.Generators {
	
	public interface IObjectSpaceObject : INotifyPropertyChanging, INotifyPropertyChanged {
		bool Dirty { get; set; }
	}

	public class Class1 : INotifyPropertyChanging, INotifyPropertyChanged {

		public event PropertyChangingEventHandler? PropertyChanging;
		public event PropertyChangedEventHandler? PropertyChanged;
	}
}
