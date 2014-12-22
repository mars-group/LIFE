using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace CustomCollections.Collections {
    /// <summary>
    ///     A ThreadSafe List
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ThreadSafeList<T> : IList<T> {
        protected readonly List<T> InternalList;

        protected static ReaderWriterLockSlim Lock;

        public ThreadSafeList() {
            Lock = new ReaderWriterLockSlim();
            InternalList = new List<T>();
        }

        public IEnumerator<T> GetEnumerator() {
            return Clone().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return Clone().GetEnumerator();
        }


        private List<T> Clone() {
            var newList = new List<T>();

            Lock.EnterReadLock();
            try {
                InternalList.ForEach(x => newList.Add(x));
            }
            finally {
                Lock.ExitReadLock();
            }

            return newList;
        }

        public void Add(T item) {
            InternalList.Add(item);
        }

        public void Clear() {
            Lock.EnterWriteLock();
            try {
                InternalList.Clear();
            }
            finally {
                Lock.ExitWriteLock();
            }
        }

        public bool Contains(T item) {
            Lock.EnterReadLock();
            try {
                return InternalList.Contains(item);
            }
            finally {
                Lock.ExitReadLock();
            }
        }

        public void CopyTo(T[] array, int arrayIndex) {
            Lock.EnterReadLock();
            try {
                InternalList.CopyTo(array, arrayIndex);
            }
            finally {
                Lock.ExitReadLock();
            }
        }

        public bool Remove(T item) {
            Lock.EnterWriteLock();
            try {
                return InternalList.Remove(item);
            }
            finally {
                Lock.ExitWriteLock();
            }
        }

        public int Count {
            get {
                Lock.EnterReadLock();
                try {
                    return InternalList.Count;
                }
                finally {
                    Lock.ExitReadLock();
                }
            }
            private set { }
        }

        public bool IsReadOnly { get; private set; }

        public int IndexOf(T item) {
            Lock.EnterReadLock();
            try {
                return InternalList.IndexOf(item);
            }
            finally {
                Lock.ExitReadLock();
            }
        }

        public void Insert(int index, T item) {
            Lock.EnterWriteLock();
            try {
                InternalList.Insert(index, item);
            }
            finally {
                Lock.ExitWriteLock();
            }
        }

        public void RemoveAt(int index) {
            Lock.EnterWriteLock();
            try {
                InternalList.RemoveAt(index);
            }
            finally {
                Lock.ExitWriteLock();
            }
        }

        public T this[int index] {
            get {
                Lock.EnterReadLock();
                try {
                    return InternalList[index];
                }
                finally {
                    Lock.ExitReadLock();
                }
            }
            set {
                Lock.EnterWriteLock();
                try {
                    InternalList[index] = value;
                }
                finally {
                    Lock.ExitWriteLock();
                }
            }
        }
    }
}