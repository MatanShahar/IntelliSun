﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace IntelliSun.Collections
{
    public static class EnumerableExtensions
    {
        public static bool Any<T>(this IEnumerable<T> enumerable, T item)
        {
            var array = enumerable as T[] ?? enumerable.ToArray();
            return !array.IsEmpty() && array.Any(x => x.Equals(item));
        }

        public static bool Any<T>(this IEnumerable<T> enumerable, T item, EqualityComparer<T> comparer)
        {
            var array = enumerable as T[] ?? enumerable.ToArray();
            return !array.IsEmpty() && array.Any(x => comparer.Equals(x, item));
        }

        public static void Foreach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var item in enumerable)
                action(item);
        }

        public static void Foreach<T>(this IEnumerable<T> enumerable, Action<T, int> action)
        {
            var idx = 0;
            foreach (var item in enumerable)
                action(item, idx++);
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            return (enumerable == null || enumerable.IsEmpty());
        }

        public static bool IsEmpty<T>(this IEnumerable<T> enumerable)
        {
            var collection = enumerable as ICollection<T>;
            if (collection != null)
                return collection.Count == 0;

            return !enumerable.Any();
        }

        public static IEnumerable<T> RemoveNulls<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.Where(item => !ReferenceEquals(item, null));
        }

        public static IEnumerable RemoveNulls(this IEnumerable enumerable)
        {
            return enumerable.Cast<object>().Where(item => item != null);
        }

        public static IEnumerable<T> Append<T>(this IEnumerable<T> enumerable, T item)
        {
            foreach (var i in enumerable)
                yield return i;

            yield return item;
        }

        public static IEnumerable<T> Append<T>(this IEnumerable<T> enumerable, params T[] items)
        {
            foreach (var i in enumerable)
                yield return i;

            foreach (var item in items)
                yield return item;
        }

        public static IEnumerable<T> AppendAt<T>(this IEnumerable<T> enumerable, int index, T item)
        {
            var idx = 0;
            foreach (var i in enumerable)
            {
                if (index == idx++)
                    yield return item;

                yield return i;
            }
        }

        public static T FirstOrValue<T>(this IEnumerable<T> enumerable, T value)
        {
            var array = enumerable as T[] ?? enumerable.ToArray();
            return (array.IsEmpty() ? value : array.First());
        }

        public static T SingleOrValue<T>(this IEnumerable<T> enumerable, T value, Func<T, bool> predicate)
        {
            var single = value;

            var array = enumerable as T[] ?? enumerable.ToArray();
            if (array.IsEmpty())
                return single;

            single = array.Single(predicate);

            return single;
        }

        public static IEnumerable<T> RemoveAny<T>(IEnumerable<T> enumerable, Predicate<T> predicate)
        {
            return enumerable.Where(item => !predicate(item));
        }

        public static bool ContainsAny<T>(this IEnumerable<T> enumerable, IEnumerable<T> items)
        {
            return items.Any(enumerable.Contains);
        }

        [SuppressMessage("ReSharper", "CanBeReplacedWithTryCastAndCheckForNull", 
            Justification = "TResult is not constraint to class")]
        public static IEnumerable<TResult> CastOrSkip<TResult>(this IEnumerable enumerable)
        {
            var enumerator = enumerable.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var item = enumerator.Current;
                if (item is TResult)
                    yield return (TResult)item;
            }
        }

        public static bool IsMatch<T1, T2>(this IEnumerable<T1> enumerable, IEnumerable<T2> other)
        {
            if (ReferenceEquals(enumerable, other))
                return true;

            if (enumerable == null || other == null)
                return false;

            var ls1 = enumerable.ToList();
            var ls2 = other.ToList();

            if (ls1.Count != ls2.Count)
                return false;

            return !ls1.Where((t, i) => !t.Equals(ls2[i])).Any();
        }

        public static bool IsMatch<T1, T2>(this IEnumerable<T1> enumerable, IEnumerable<T2> other,
            Func<T1, T2, bool> predicate)
        {
            if (ReferenceEquals(enumerable, other))
                return true;

            if (enumerable == null || other == null)
                return false;

            var ls1 = enumerable.ToList();
            var ls2 = other.ToList();

            if (ls1.Count != ls2.Count)
                return false;

            return !ls1.Where((t, i) => !predicate(t, ls2[i])).Any();
        }

        public static IEnumerable<T> But<T>(this IEnumerable<T> enumerable, T item)
        {
            return enumerable.Where(current => !current.Equals(item));
        }

        public static IEnumerable<T> With<T>(this IEnumerable<T> enumerable, T item)
        {
            foreach (var current in enumerable)
                yield return current;

            yield return item;
        }

        public static void Enumerate<T>(this IEnumerable<T> enumerable)
        {
            var enumerator = enumerable.GetEnumerator();
            while (enumerator.MoveNext())
            {
            }
        }

        public static IEnumerable<T> Resolve<T>(this IEnumerable<T> enumerable)
        {
            var enumerator = enumerable.GetEnumerator();
            while (enumerator.MoveNext())
                yield return enumerator.Current;
        }

        [Obsolete("Use EnumerableExtensions.Enumerate instead")]
        public static void DoEnumerate<T>(this IEnumerable<T> enumerable)
        {
            var enumerator = enumerable.GetEnumerator();
            while (enumerator.MoveNext())
            {
            }
        }

        public static IEnumerable<T> NotNull<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.Where(arg => !ReferenceEquals(arg, null));
        }

        public static void Invoke<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var item in enumerable)
                action(item);
        }

        public static void InvokeWhile<T>(this IEnumerable<T> enumerable, Func<T, bool> action)
        {
            var enumerator = enumerable.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (!action(enumerator.Current))
                    break;
            }
        }

        public static void InvokeWhile<T>(this IEnumerable<T> enumerable, Action<T> action, Func<T, bool> predicate)
        {
            var enumerator = enumerable.GetEnumerator();
            while (enumerator.MoveNext() && predicate(enumerator.Current))
                action(enumerator.Current);
        }

        public static TResult FirstValueOrDefault<T, TResult>(this IEnumerable<T> enumerable, Func<T, TResult> selector)
            where TResult : class
        {
            return enumerable.Select(selector).FirstOrDefault(value => value != null);
        }

        public static void Each<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var item in enumerable)
                action(item);
        }

        public static IEnumerable<TResult> SelectWhere<T, TResult>(this IEnumerable<T> enumerable,
            Func<T, EnumerableSelection<TResult>> selector)
        {
            return from item in enumerable
                   select selector(item)
                   into result
                   where !result.IsEmpty
                   select result.Value;
        }

        public static TResult FirstValueOrDefault<T, TResult>(this IEnumerable<T> enumerable, Func<T, TResult> selector,
            Func<TResult, bool> predicate)
        {
            foreach (var value in enumerable.Select(selector).Where(predicate))
                return value;

            return default(TResult);
        }

        public static IEnumerable Close(this IEnumerable enumerable)
        {
            return new CloseEnumerable(enumerable);
        }

        public static IEnumerable<T> Close<T>(this IEnumerable<T> enumerable)
        {
            return new CloseEnumerable<T>(enumerable);
        }

        public static IEnumerable<TItem> DistinctBy<TItem, TArg>(
            this IEnumerable<TItem> enumerable, Func<TItem, TArg> selector)
        {
            return DistinctBy(enumerable, selector, null);
        }

        public static IEnumerable<TItem> DistinctBy<TItem, TArg>(
            this IEnumerable<TItem> enumerable, Func<TItem, TArg> selector,
            IEqualityComparer<TArg> comparer)
        {
            if (enumerable == null)
                throw new ArgumentNullException("enumerable");

            if (selector == null)
                throw new ArgumentNullException("selector");

            var innerComparer = comparer ?? new OperatorEqualityComparer<TArg>();
            var selectionComparer = new DistinctByComparer<TItem, TArg>(selector, innerComparer);

            return enumerable.Distinct(selectionComparer);
        }

        public static bool AllTrue(this IEnumerable<bool> enumerable)
        {
            return enumerable.All(item => item);
        }

        public static bool AllTrue(this IEnumerable<Func<bool>> enumerable)
        {
            return enumerable.All(item => item());
        }

        public static bool AllTrue<TIn>(this IEnumerable<Func<TIn, bool>> enumerable, TIn state)
        {
            return enumerable.All(item => item(state));
        }

        public static bool AllTrue<TIn>(this IEnumerable<Predicate<TIn>> enumerable, TIn state)
        {
            return enumerable.All(item => item(state));
        }

        public static IOrderedEnumerable<TElement> Order<TElement>(this IEnumerable<TElement> enumerable,
            IComparer<TElement> comparer)
        {
            return Order(enumerable, comparer, false);
        }

        public static IOrderedEnumerable<TElement> OrderDesending<TElement>(this IEnumerable<TElement> enumerable,
            IComparer<TElement> comparer)
        {
            return Order(enumerable, comparer, true);
        }

        private static IOrderedEnumerable<TElement> Order<TElement>(IEnumerable<TElement> enumerable,
            IComparer<TElement> comparer, bool desending)
        {
            if (enumerable == null)
                throw new ArgumentNullException("enumerable");

            if (comparer == null)
                throw new ArgumentNullException("comparer");

            return new ComparerOrderedEnumerable<TElement>(enumerable, comparer, desending);
        }

        private class DistinctByComparer<T, TArg> : IEqualityComparer<T>
        {
            private readonly Func<T, TArg> selector;
            private readonly IEqualityComparer<TArg> innerComparer;

            public DistinctByComparer(Func<T, TArg> selector, IEqualityComparer<TArg> innerComparer)
            {
                if (selector == null)
                    throw new ArgumentNullException("selector");

                if (innerComparer == null)
                    throw new ArgumentNullException("innerComparer");

                this.selector = selector;
                this.innerComparer = innerComparer;
            }

            public bool Equals(T x, T y)
            {
                if (ReferenceEquals(x, y))
                    return true;

                var xArg = this.selector(x);
                var yArg = this.selector(y);

                return this.innerComparer.Equals(xArg, yArg);
            }

            public int GetHashCode(T obj)
            {
                return this.innerComparer.GetHashCode(this.selector(obj));
            }
        }

        private class OperatorEqualityComparer<T> : IEqualityComparer<T>
        {
            public bool Equals(T x, T y)
            {
                return ReferenceEquals(x, y) || x.Equals(y);
            }

            public int GetHashCode(T obj)
            {
                return obj.GetHashCode();
            }
        }

    }

    //internal class ValidationEnumerable<T> : IEnumerable<T> 
    //{
    //    public ValidationEnumerable()
    //    {
    //        
    //    }
    //}
}