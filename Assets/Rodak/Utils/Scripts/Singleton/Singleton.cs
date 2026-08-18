using System;

namespace Rodak.Utils.Singleton
{
    public abstract class Singleton<T> where T : Singleton<T>
    {
        private static readonly Lazy<T> instance = new(() =>
        {
            return Activator.CreateInstance(typeof(T), true) as T;
        });

        public static T Instance => instance.Value;
    }
}