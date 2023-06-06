
namespace Factory
{
    public interface IFactory<T, A>
    {
        public void Create(T parameter, A other);
    }
    public interface IFactory<T>
    {
        public void Create(T parameter);
        public T Created { get; }
    }
    public interface IFactory
    {
        public void Create();
    }
}