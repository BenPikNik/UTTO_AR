using System;
using System.Collections;
using System.Collections.Generic;

public class CircularBuffer<T> : IEnumerable<T> {
    private T[] elements;
    private int nextIndex = 0;
    private int _size = 0;

    public CircularBuffer(int capacity)
    {
        elements = new T[capacity];
    }

    public int size
    {
        get
        {
            return _size;
        }
    }

    public void add(T elem)
    {
        elements[nextIndex] = elem;
        ++nextIndex;
        if (nextIndex == elements.Length)
        {
            nextIndex = 0;
            _size = elements.Length;
        }
        if (_size != elements.Length)
        {
            ++_size;
        }
    }

    public IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < _size; ++i)
        {
            yield return elements[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void clear()
    {
        nextIndex = 0;
        _size = 0;
    }
}
