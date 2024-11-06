using System;

namespace UnityEngine.XR.Interaction.Toolkit.Utilities.Collections
{
    /// <summary>
    /// A simple circular buffer implementation for efficient data storage and access.
    /// </summary>
    /// <typeparam name="T">The type of elements stored in the buffer.</typeparam>
    internal class CircularBuffer<T>
    {
        readonly T[] m_Buffer;
        int m_Start;
        int m_Count;

        /// <summary>
        /// Initializes a new instance of the <see cref="CircularBuffer{T}"/> class.
        /// </summary>
        /// <param name="capacity">The maximum number of elements the buffer can hold.</param>
        public CircularBuffer(int capacity)
        {
            m_Buffer = new T[capacity];
            m_Start = 0;
            m_Count = 0;
        }

        /// <summary>
        /// Gets the number of elements currently in the buffer.
        /// </summary>
        public int count => m_Count;

        /// <summary>
        /// Gets the maximum number of elements the buffer can hold.
        /// </summary>
        public int capacity => m_Buffer.Length;

        /// <summary>
        /// Adds an item to the buffer, overwriting the oldest item if the buffer is full.
        /// </summary>
        /// <param name="item">The item to add to the buffer.</param>
        public void Add(T item)
        {
            int index = (m_Start + m_Count) % m_Buffer.Length;
            m_Buffer[index] = item;

            if (m_Count < m_Buffer.Length)
                m_Count++;
            else
                m_Start = (m_Start + 1) % m_Buffer.Length;
        }

        /// <summary>
        /// Gets the item at the specified index in the buffer.
        /// </summary>
        /// <param name="index">The index of the item to retrieve.</param>
        /// <returns>The item at the specified index.</returns>
        /// <exception cref="IndexOutOfRangeException">Throws if <paramref name="index"/> is less than 0 or equal to or greater than <see cref="count"/>.</exception>
        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= m_Count)
                    throw new IndexOutOfRangeException();

                return m_Buffer[(m_Start + index) % m_Buffer.Length];
            }
        }

        /// <summary>
        /// Clears all items from the buffer.
        /// </summary>
        public void Clear()
        {
            m_Start = 0;
            m_Count = 0;
        }
    }
}
