using System.Threading.Tasks;

namespace ProcessFlow.Interfaces
{
    public interface IProcessor<T>
    {
        Task<T> Process(T state);
    }
}
