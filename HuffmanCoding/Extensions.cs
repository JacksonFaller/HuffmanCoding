using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace HuffmanCoding
{
    public static class ListExtensions
    {
        public static void AddSorted<T>(this List<T> @this, T item) where T : IComparable<T>
        {
            if (@this.Count == 0)
            {
                @this.Add(item);
                return;
            }
            if (@this[@this.Count - 1].CompareTo(item) <= 0)
            {
                @this.Add(item);
                return;
            }
            if (@this[0].CompareTo(item) >= 0)
            {
                @this.Insert(0, item);
                return;
            }
            int index = @this.BinarySearch(item);
            if (index < 0)
                index = ~index;
            @this.Insert(index, item);
        }

        public static IEnumerable<T> ReverseImmutable<T>(this IList<T> @this)
        {
            for (int i = @this.Count - 1; i >= 0; i--)
            {
                yield return @this[i];
            }
        }
    }

    public static class CharExtensions
    {
        public static bool ToBool(this char @this)
        {
            switch (@this)
            {
                case '1': return true;
                case '0': return false;
                default: throw new ArgumentException("Character neither 1 nor 0, can't convert", nameof(@this));
            }
        }
    }

    public static class BoolExtensions
    {
        public static char ToChar(this bool @this) => @this ? '1' : '0';
    }


    public class ListComparer<T> : IEqualityComparer<List<T>>
    {
        private readonly IEqualityComparer<T> _comparer;
        private readonly Func<T, T, bool> _equalityFunc;

        public ListComparer(IEqualityComparer<T> comparer = null)
        {
            _comparer = comparer;
            if (comparer != null)
                _equalityFunc = _comparer.Equals;
            else
                _equalityFunc = (T x, T y) => x.Equals(y);
        }

        public bool Equals([AllowNull] List<T> x, [AllowNull] List<T> y)
        {
            if (ReferenceEquals(x, y))
                return true;

            if (x.Count != y.Count)
                return false;

            for (int i = 0; i < x.Count; i++)
            {
                if (!_equalityFunc(x[i], y[i]))
                    return false;
            }
            return true;
        }

        public int GetHashCode([DisallowNull] List<T> elements)
        {
            unchecked
            {
                int hash = 19;
                foreach (var element in elements)
                {
                    hash = hash * 31 + element.GetHashCode();
                }
                return hash;
            }
        }
    }
}
