using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace SharpBlock.Server
{
    public class SocketAsyncEventArgsPool
    {
        private readonly Stack<SocketAsyncEventArgs> _pool;

        public SocketAsyncEventArgsPool(int capacity)
        {
            _pool = new Stack<SocketAsyncEventArgs>(capacity);
        }

        public void Push(SocketAsyncEventArgs item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            lock (_pool)
            {
                _pool.Push(item);
            }
        }

        public SocketAsyncEventArgs? Pop()
        {
            lock (_pool)
            {
                return _pool.Count > 0 ? _pool.Pop() : null;
            }
        }

        public int Count
        {
            get { lock (_pool) { return _pool.Count; } }
        }
    }
}