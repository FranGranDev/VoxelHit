namespace Services
{
    public interface IStock
    {
        public int MaxCount { get; }
        public int Count { get; }

        public event System.Action<IStock> OnChanged;
    }
}
