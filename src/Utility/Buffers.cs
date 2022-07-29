using System;
using System.Buffers;

namespace ClassicUO.Utility
{
    public static class Buffers
    {
        public static T[] Rent<T>(this ArrayPool<T> pool, int minimumLength, bool clear)
        {
            var array = pool.Rent(minimumLength);

            if (clear)
            {
                Array.Clear(array, 0, minimumLength);
            }

            return array;
        }
    }
}