using System;

namespace TankGame
{
    public interface IHeapItem<T> : IComparable<T>
    {
        int HeapIndex
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Heap algorithm for fast item referencing. Namely for A* pathfinding.
    /// <para>
    /// Referenced from SebLague (https://github.com/SebLague/Pathfinding/tree/master)
    /// </para>
    /// </summary>
    /// <typeparam name="T">Generic type that implements <see cref="IHeapItem{T}"/></typeparam>
    public class Heap<T> where T : IHeapItem<T>
    {
        private T[] items;
        private int currentItemCount;

        public Heap(int maxHeapSize)
        {
            items = new T[maxHeapSize];
        }

        /// <summary>
        /// Add item to heap
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            // Add item to last index
            item.HeapIndex = currentItemCount;
            items[currentItemCount] = item;
            currentItemCount++;

            // Move item upwards
            SortUp(item);
        }

        /// <summary>
        /// Remove and get root item from heap
        /// </summary>
        /// <returns>Root item</returns>
        public T RemoveFirst()
        {
            T firstItem = items[0];
            currentItemCount--;

            // Replace root item with last item
            // And sort root downwards
            items[0] = items[currentItemCount];
            items[0].HeapIndex = 0;
            SortDown(items[0]);

            return firstItem;
        }

        public void UpdateItem(T item)
        {
            SortUp(item);
        }

        public int Count
        {
            get { return currentItemCount; }
        }

        public bool Contains(T item)
        {
            return Equals(items[item.HeapIndex], item);
        }

        /// <summary>
        /// Move item downwards in heap until it is not larger than it's children.
        /// </summary>
        /// <param name="item">Item to be sorted downwards</param>
        private void SortDown(T item)
        {
            while (true)
            {
                // Get array index of items
                int childIndexLeft = (item.HeapIndex * 2) + 1;
                int childIndexRight = (item.HeapIndex * 2) + 2;

                int swapIndex = 0;

                // If index exists
                if (childIndexLeft < currentItemCount)
                {
                    swapIndex = childIndexLeft;

                    if (childIndexRight < currentItemCount)
                    {
                        // Swap with the larger child
                        if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0)
                        {
                            swapIndex = childIndexRight;
                        }
                    }

                    // Make sure child is smaller
                    if (item.CompareTo(items[swapIndex]) < 0)
                    {
                        Swap(item, items[swapIndex]);
                    }
                    else
                    {
                        return;
                    }
                }
                else
                    return;
            }
        }

        /// <summary>
        /// Move item upwards in heap until it is not smaller than it's parent
        /// </summary>
        /// <param name="item"></param>
        private void SortUp(T item)
        {
            int parentIndex = (item.HeapIndex - 1) / 2;

            while (true)
            {
                T parentItem = items[parentIndex];

                // If item is 
                if (item.CompareTo(parentItem) > 0)
                    Swap(item, parentItem);
                else
                    break;

                parentIndex = (item.HeapIndex - 1) / 2;
            }
        }

        /// <summary>
        /// Swap index positions and heap position of both items
        /// </summary>
        private void Swap(T itemA, T itemB)
        {
            items[itemA.HeapIndex] = itemB;
            items[itemB.HeapIndex] = itemA;

            int itemAIndex = itemA.HeapIndex;
            itemA.HeapIndex = itemB.HeapIndex;
            itemB.HeapIndex = itemAIndex;
        }
    }
}
