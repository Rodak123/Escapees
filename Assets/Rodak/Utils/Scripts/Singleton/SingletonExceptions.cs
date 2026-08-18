using System;

namespace Rodak.Utils.Singleton
{
    [Serializable]
    public class SingletonException : Exception
    {
        public SingletonException() { }
        public SingletonException(string message) : base(message) { }
    }

    [Serializable]
    public class SingletonUninitializedException : Exception
    {
        public SingletonUninitializedException() { }
        public SingletonUninitializedException(string message) : base(message) { }
    }
}