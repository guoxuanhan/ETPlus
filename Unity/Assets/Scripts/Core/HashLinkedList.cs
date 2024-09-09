using System.Collections.Generic;

namespace ET
{
    public class HashLinkedList<T, K>
    {
        private Dictionary<T, LinkedListNode<K>> dict = new Dictionary<T, LinkedListNode<K>>();

        private LinkedList<K> linkedList = new LinkedList<K>();

        public K this[T t]
        {
            get
            {
                if (this.dict.ContainsKey(t))
                {
                    return this.dict[t].Value;
                }

                return default;
            }
        }

        public int Count
        {
            get
            {
                return this.dict.Count;
            }
        }

        public K First
        {
            get
            {
                var first = this.linkedList.First;
                if (first == null)
                {
                    return default;
                }

                return first.Value;
            }
        }

        public K Last
        {
            get
            {
                var last = this.linkedList.Last;
                if (last == null)
                {
                    return default;
                }

                return last.Value;
            }
        }

        public bool AddFirst(T t, K k)
        {
            if (this.dict.ContainsKey(t))
            {
                return false;
            }

            LinkedListNode<K> node = this.linkedList.AddFirst(k);
            this.dict.Add(t, node);
            return true;
        }

        public bool AddLast(T t, K k)
        {
            if (this.dict.ContainsKey(t))
            {
                return false;
            }

            LinkedListNode<K> node = this.linkedList.AddLast(k);
            this.dict.Add(t, node);
            return true;
        }

        public K Remove(T t)
        {
            if (this.dict.TryGetValue(t, out LinkedListNode<K> node))
            {
                K value = node.Value;
                this.dict.Remove(t);
                this.linkedList.Remove(node);
                return value;
            }

            return default;
        }

        public bool ContainKey(T t)
        {
            return this.dict.ContainsKey(t);
        }

        public LinkedList<K>.Enumerator GetEnumerator()
        {
            return this.linkedList.GetEnumerator();
        }

        public void Clear()
        {
            this.dict.Clear();
            this.linkedList.Clear();
        }
    }
}