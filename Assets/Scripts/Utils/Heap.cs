using UnityEngine;
using System;
using System.Collections.Generic;

public class Heap<T>
{
    private List<T> items;
    private readonly IComparer<T> comparer;

    public Heap(IComparer<T> comparer)
    {
        this.items = new List<T>();
        this.comparer = comparer;
    }

    public int Count => items.Count;

    public void Enqueue(T item)
    {
        items.Add(item);
        SiftUp(items.Count - 1);
    }

    public T Dequeue() 
    {
        if (items.Count == 0) throw new InvalidOperationException("Heap Empty");
        T root = items[0];
        int lastIndex = items.Count - 1;
        items[0] = items[lastIndex];
        items.RemoveAt(lastIndex);
        if (items.Count > 0) SiftDown(0);
        return root;
    }
    
    public T Peek()
    {
        if (items.Count == 0) throw new InvalidOperationException("Heap Empty");
        return items[0];
    }

    private void Swap(int i, int j)
    {
        T temp = items[i];
        items[i] = items[j];
        items[j] = temp;
    }

    private void SiftUp(int idx)
    {
        int parIdx = (idx - 1) / 2;
        while (idx > 0 && comparer.Compare(items[idx], items[parIdx]) < 0)
        {
            Swap(idx, parIdx);
            idx = parIdx;
            parIdx = (idx - 1) / 2;
        }
    }
    
    private void SiftDown(int idx)
    {
        int size  = items.Count;
        while (idx < size)
        {
            int lhsIdx = 2 * idx + 1;
            int rhsIdx = 2 * idx + 2;
            int prioElement = idx;

            if (lhsIdx < size && comparer.Compare(items[lhsIdx], items[prioElement]) < 0)
            {
                prioElement = lhsIdx;
            }
            if (rhsIdx < size && comparer.Compare(items[rhsIdx], items[prioElement]) < 0)
            {
                prioElement = rhsIdx;
            }
            if (prioElement != idx)
            {
                Swap(idx, prioElement);
                idx = prioElement;
            }
            else 
            {
                break;
            }
        }
    }
}
