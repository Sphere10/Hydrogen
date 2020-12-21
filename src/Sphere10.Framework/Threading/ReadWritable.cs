namespace Sphere10.Framework {
    public sealed class ReadWritable<T> : ReadWriteSafeObject {
        public ReadWritable(T @object) {
            this.Value = @object;
        }

        public T Value { get; set; }
        
    }
}