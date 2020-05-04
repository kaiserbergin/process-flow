using System.Threading.Tasks;

namespace ProcessFlow.Interfaces
{
    public interface IProcessor<T> where T : class
    {
        Task<T> Process(T state);
    }
}
