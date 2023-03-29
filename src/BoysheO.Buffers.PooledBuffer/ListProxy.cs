﻿using Collections.Pooled;

namespace BoysheO.Buffers
{
    internal sealed class ListProxy<T>
    {
        internal int Version = 1;
        internal readonly PooledList<T> List = new PooledList<T>();
    }
}