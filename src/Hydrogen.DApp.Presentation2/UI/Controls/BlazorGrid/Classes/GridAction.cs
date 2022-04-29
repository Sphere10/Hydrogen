namespace Hydrogen.DApp.Presentation2.UI.Controls.BlazorGrid.Classes {
	public class GridAction<TItem> 
	{
		public delegate TItem PerformAction(TItem item);
		public delegate bool IsActionAvailableDelegate(TItem item);

		public string Name { get; init; }
		public PerformAction ActionWork { get; init; }
		public string IconURL { get; init; }
		public IsActionAvailableDelegate IsActionAvailable { get; init; }

		public GridAction(string name, PerformAction actionWork, string iconURL, IsActionAvailableDelegate isActionAvailable = null) 
		{
			Name = name;
			ActionWork = actionWork;
			IconURL = iconURL;
			IsActionAvailable = isActionAvailable;
		}
	}
}