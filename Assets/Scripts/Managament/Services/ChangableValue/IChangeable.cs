namespace Services
{
    public interface IChangeable<T>
    {
        public void Bind(ref System.Action<T> onChanged);
        public void SetValue(T value);
    }
}
