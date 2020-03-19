using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProcessFlow.Tests.TestUtils
{
    public interface IStepProtectedMembers<T>
    {
        Task Process(T state);
    }
}
