namespace Hydrogen.Data {
    public record DatabaseCreatedEventArgs {
        public string ConnectionString { get; init; }
        public bool CreatedEmptyDatabase { get; init; }
    }
}
