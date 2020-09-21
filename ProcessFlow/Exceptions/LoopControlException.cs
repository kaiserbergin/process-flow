using System;

namespace ProcessFlow.Exceptions
{
    public class LoopControlException : Exception
    {
        public LoopControlException(string message) : base(message) { }
    }
}