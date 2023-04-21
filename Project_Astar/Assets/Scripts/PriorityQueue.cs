using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PriorityQueue<TElement>
    where TElement : class
    //where TPriority : IConvertible
{
    private class Node
    {
        public TElement data;
        public float priority;

        public Node(TElement data, float priority)
        {
            this.data = data;
            this.priority = priority;
        }
    }

    public int Count => list.Count;

    private List<Node> list;

    public PriorityQueue()
    {
        list = new List<Node>();
    }

    public PriorityQueue(int capacity)
    {
        list = new List<Node>(capacity);
    }

    public void Clear()
    {
        list.Clear();
    }

    public void Enqueue(TElement element, float priority)
    {
        var newNode = new Node(element, priority);
        list.Add(newNode);

        int pos = list.Count;

        while (pos > 1)
        {
            int parentPos = pos / 2;
            if (list[pos - 1].priority >= list[parentPos - 1].priority)
                break;

            Node temp = list[parentPos - 1];
            list[parentPos - 1] = list[pos - 1];
            list[pos - 1] = temp;

            pos = parentPos;
        }
        //list = list.OrderBy(n => n.priority).ToList();
    }

    public TElement Dequeue()
    {
        TElement resultNode = list[0].data;

        //list.RemoveAt(0);
        list[0] = list[list.Count - 1];
        list.RemoveAt(list.Count - 1);

        int pos = 1;

        while(true)
        {
            int childPos = pos * 2;
            if (childPos >= list.Count)
                break;

            if (list[childPos - 1].priority > list[childPos].priority)
                childPos += 1;

            if (list[pos - 1].priority <= list[childPos - 1].priority)
                break;

            Node temp = list[childPos - 1];
            list[childPos - 1] = list[pos - 1];
            list[pos - 1] = temp;

            pos = childPos;
        }

        return resultNode;
    }

    private void SortAsHeap()
    {
        int pos = list.Count;

        while(pos > 1)
        {
            int parentPos = pos / 2;
            if (list[pos - 1].priority >= list[parentPos - 1].priority)
                break;

            Node temp = list[parentPos - 1];
            list[parentPos - 1] = list[pos - 1];
            list[pos - 1] = temp;

            pos = parentPos;
        }
    }
}
