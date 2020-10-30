using System;

namespace Puzzle.Actions {
    /**
     * Stack backed by a ring buffer. If the buffer is full, new elements will overwrite old elements.
     */
    [Serializable]
    public struct RingStack<T> {
        private readonly T[] items;
        private int first;
        private int next;
        
        public int Size { get; private set; }
        public bool Empty => Size == 0;

        public bool AtEnd => (next + items.Length - first) % items.Length == Size;

        public RingStack(int capacity) {
            items = new T[capacity];
            first = 0;
            next = 0;
            Size = 0;
        }

        public void Push(T item) {
            items[next] = item;
            if (next == first && !Empty) {
                first++;
            } else {
                Size++;
            }
            next = (next + 1) % items.Length;
        }

        public T Pop() {
            if (Size == 0) {
                throw new InvalidOperationException("empty");
            }
            
            next = (next + items.Length - 1) % items.Length;
            Size--;
            return items[next];
        }

        public T Peek() {
            return items[next-1];
        }

        public T Advance() {
            next = (next + 1) % items.Length;
            Size++;
            return Peek();
        }

        public void Clear() {
            first = 0;
            next = 0;
            Size = 0;
        }
    }
}