//using PointCloudVR;
//using System;
//using System.Collections.Generic;
//using UnityEngine;

//public class PriorityQueue<T> where T : Chunk
//{
//    private List<T> items;
//    private Dictionary<T, int> itemIndices; // to track the indices of items in the heap
//    //private List<GameObject> uniqueGO = new List<GameObject>();
//    public int Count { get { return items.Count; } }

//    public PriorityQueue()
//    {
//        items = new List<T>();
//        itemIndices = new Dictionary<T, int>();
//    }

//    public void Enqueue(T item)
//    {
//        if (items.Contains(item))
//        {
//            UpdatePriority(item);
//        } else
//        {
//            items.Add(item);
//            itemIndices[item] = Count - 1;
//            //uniqueGO.Add(item.gObj);
//            HeapifyUp(Count - 1);
//            //uniqueGO.Add(item.gObj);
//        }
        
//    }

//    public T Dequeue()
//    {
//        if (Count == 0)
//            throw new InvalidOperationException("Priority queue is empty");

//        T firstItem = items[0];
//        int lastIndex = Count - 1;
//        T lastItem = items[lastIndex];
//        items[0] = lastItem;
//        items.RemoveAt(lastIndex);
//        itemIndices.Remove(firstItem);

//        if (Count > 1)
//        {
//            itemIndices[lastItem] = 0;
//            HeapifyDown(0);
//        }

//        //uniqueGO.Remove(firstItem.gObj);

//        return firstItem;
//    }

//    public void UpdatePriority(T item)
//    {
//        int index = itemIndices[item];
//        HeapifyUp(index);
//        HeapifyDown(index);
//    }

//    private void HeapifyUp(int index)
//    {
//        int parentIndex = (index - 1) / 2;

//        while (index > 0 && Compare(items[index], items[parentIndex]) < 0)
//        {
//            Swap(index, parentIndex);
//            index = parentIndex;
//            parentIndex = (index - 1) / 2;
//        }
//    }

//    public void Clear()
//    {
//        items.Clear();
//        itemIndices.Clear();
//    }


//    private void HeapifyDown(int index)
//    {
//        int leftChildIndex = 2 * index + 1;
//        int rightChildIndex = 2 * index + 2;
//        int smallestIndex = index;

//        if (leftChildIndex < Count && Compare(items[leftChildIndex], items[smallestIndex]) < 0)
//            smallestIndex = leftChildIndex;

//        if (rightChildIndex < Count && Compare(items[rightChildIndex], items[smallestIndex]) < 0)
//            smallestIndex = rightChildIndex;

//        if (smallestIndex != index)
//        {
//            Swap(index, smallestIndex);
//            HeapifyDown(smallestIndex);
//        }
//    }

//    private void Swap(int i, int j)
//    {
//        T temp = items[i];
//        items[i] = items[j];
//        items[j] = temp;

//        itemIndices[items[i]] = i;
//        itemIndices[items[j]] = j;
//    }

//    private int Compare(T item1, T item2)
//    {
//        // Implement your comparison logic here
//        // For example, if T is an integer, you can simply return item1 - item2
//        // For other types, you might need a custom comparison method or comparer
//        // In this implementation, T is assumed to be IComparable<T>

//        return (int) item1.priority*10 - (int) item2.priority*10;
//    }
//}
