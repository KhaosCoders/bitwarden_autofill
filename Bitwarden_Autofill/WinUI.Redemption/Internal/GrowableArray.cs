using System;

namespace WinUI.Redemption.Internal;

internal class GrowableArray<T>
{
    private T?[] _array;

    public GrowableArray(int initialCount = 10)
    {
        _array = new T[initialCount];
    }

    public int Count => _array.Length;

    public T? this[int index]
    {
        get
        {
            if (index > _array.Length - 1)
                return default;
            return _array[index];
        }
        set
        {
            EnsureOrGrow(index);
            _array[index] = value;
        }
    }

    private void EnsureOrGrow(int index)
    {
        if (index > _array.Length - 1)
        {
            var newArray = new T[Math.Max(_array.Length * 2, index + 1)];
            Array.Copy(_array, 0, newArray, 0, _array.Length);
            _array = newArray;
        }
    }
}