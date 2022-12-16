namespace Hydrogen.Data;

public record DatabaseSchemasCreatedEventArgs {
	public string ConnectionString { get; init; }
}
