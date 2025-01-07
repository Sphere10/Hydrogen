using Hydrogen;


public interface IStreamMappedRecylableList : IRecyclableList, IStreamMappedCollection {
	long Count { get; }
}
