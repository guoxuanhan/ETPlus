using System;
using System.Collections.Generic;

namespace ET
{
    public class DictionaryComponent<T, K>: Dictionary<T, K>, IDisposable
    {
        public static DictionaryComponent<T, K> Create()
        {
            return ObjectPool.Instance.Fetch(typeof (DictionaryComponent<T, K>)) as DictionaryComponent<T, K>;
        }

        public void Dispose()
        {
            this.Clear();
            ObjectPool.Instance.Recycle(this);
        }
    }
}