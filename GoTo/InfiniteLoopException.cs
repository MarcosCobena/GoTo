using System;

namespace GoTo
{
    public class InfiniteLoopException : Exception
    {
        public InfiniteLoopException(string message) : base(message)
        {
        }
    }
}