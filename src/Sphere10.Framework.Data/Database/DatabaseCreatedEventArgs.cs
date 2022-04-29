namespace Sphere10.Framework.Data {
    public record DatabaseCreatedEventArgs {
        public string ConnectionString { get; init; }
        public bool CreatedEmptyDatabase { get; init; }
    }
}
