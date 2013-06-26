using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace SpencerHakim.Extensions
{
    /// <summary>
    /// Methods for IEnumerables, ILists, and IBindingLists
    /// </summary>
    public static class EnumerableMethods
    {
        /// <summary>
        /// Performs the specified action on each member of the enumerable. Best for short Actions.
        /// </summary>
        /// <typeparam name="T">Type of items in the enumerable</typeparam>
        /// <param name="enumerable">The enumerable on which to act</param>
        /// <param name="action">The action to perform on each member of the enumerable</param>
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            if( enumerable == null )
                throw new ArgumentNullException("enumerable");

            if( action == null )
                throw new ArgumentNullException("action");

            foreach( T item in enumerable )
                action(item);
        }

        /// <summary>
        /// Copies the contents of an enumerable to a list. Does not clear the list beforehand.
        /// </summary>
        /// <typeparam name="T">Type of items in the enumerable and list</typeparam>
        /// <param name="enumerable">The enumerable to copy from</param>
        /// <param name="list">The list to copy to</param>
        public static void CopyTo<T>(this IEnumerable<T> enumerable, IList<T> list)
        {
            enumerable.ForEach(item => list.Add(item));
        }

        /// <summary>
        /// Finds the index of the specified object in the enumerable
        /// </summary>
        /// <typeparam name="T">Type of items in the enumerable</typeparam>
        /// <param name="enumerable">The enumerable to search</param>
        /// <param name="obj">The object to find the index of</param>
        /// <returns>The index of the object if it is found in the enumerable, otherwise -1</returns>
        public static int IndexOf<T>(this IEnumerable<T> enumerable, T obj)
        {
            var result = enumerable.Select((o, i) => new { Object=o, Index=i }).Where(anon => Object.ReferenceEquals(anon.Object, obj));

            if( result.Any() )
                return result.First().Index;

            else
            {
                result = enumerable.Select((o, i) => new { Object=o, Index=i }).Where(anon => anon.Object.Equals(obj));

                if( result.Any() )
                    return result.First().Index;
                else
                    return -1;
            }
        }

        /// <summary>
        /// Inserts an object of type T before the other specified object in the list
        /// </summary>
        /// <typeparam name="T">Type of items in the list</typeparam>
        /// <param name="list">The list to insert into</param>
        /// <param name="obj">The object that will serve as the marker for the new object</param>
        /// <param name="item">The object to be inserted</param>
        public static void InsertBefore<T>(this IList<T> list, T obj, T item)
        {
            if( list == null )
                throw new ArgumentNullException("list");

            int i = list.IndexOf(obj);
            if( i == -1 )
                i = 0; //-1 throws an exception, so change it to 0

            list.Insert(i, item);
        }

        /// <summary>
        /// Inserts an object of type T after the other specified object in the list
        /// </summary>
        /// <typeparam name="T">Type of items in the list</typeparam>
        /// <param name="list">The list to insert into</param>
        /// <param name="obj">The object that will serve as the marker for the new object</param>
        /// <param name="item">The object to be inserted</param>
        public static void InsertAfter<T>(this IList<T> list, T obj, T item)
        {
            if( list == null )
                throw new ArgumentNullException("list");

            int i = list.IndexOf(obj);
            if( i == -1 )
                i = list.Count-1; //-1 throws an exception, so change it to the last index

            list.Insert(i+1, item); //+1 the index to move to to after that element
        }

        /// <summary>
        /// Determines if an enumerable is empty
        /// </summary>
        /// <typeparam name="T">The type of items in the enumerable</typeparam>
        /// <param name="enumerable">The enumerable on which to act</param>
        /// <returns>True if empty, false otherwise</returns>
        public static bool IsEmpty<T>(this IEnumerable<T> enumerable)
        {
            if( enumerable == null )
                throw new ArgumentNullException("enumerable");

            var genericCollection = enumerable as ICollection<T>;
            if( genericCollection != null )
                return genericCollection.Count == 0;

            var nonGenericCollection = enumerable as ICollection;
            if( nonGenericCollection != null )
                return nonGenericCollection.Count == 0;

            return !enumerable.Any();
        }

        /// <summary>
        /// Swaps the positions of the items at the provided indices in a BindingList
        /// </summary>
        /// <typeparam name="T">Type of items in the BindingList</typeparam>
        /// <param name="list">The BindingList on which to act</param>
        /// <param name="index0">The index of the first item in the swap</param>
        /// <param name="index1">The index of the second item in the swap</param>
        /// <returns>True if the rows were swapped, false is they weren't</returns>
        public static bool Swap<T>(this BindingList<T> list, int index0, int index1)
        {
            if( list == null )
                throw new ArgumentNullException("list");

            //can't swap with self
            if( index0 == index1 )
                return false;

            //ignore attempts to swap above top
            if( index0 < 0 || index1 < 0 )
                return false;

            //ignore attempts to swap below bottom
            if( index0 >= list.Count || index1 >= list.Count )
                return false;

            bool raiseEvents = list.RaiseListChangedEvents;

            try
            {
                list.RaiseListChangedEvents = false;

                //this should be more transactional; what if setting list[index1] fails for whatever reason?
                T tmp = list[index0];
                list[index0] = list[index1];
                list[index1] = tmp;
            }
            finally
            {
                list.RaiseListChangedEvents = raiseEvents;
                list.ResetItem(index0);
                list.ResetItem(index1);
            }

            return true;
        }

        /// <summary>
        /// Used for EnumerableMethods.Distinct
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private class TComparer<T> : IEqualityComparer<T>
        {
            private Func<T, T, bool> _pred;

            public TComparer(Func<T, T, bool> predicate)
            {
                _pred = predicate;
            }

            public bool Equals(T a, T b)
            {
                return _pred(a, b);
            }

            public int GetHashCode(T obj)
            {
                return 0; //force usage of Equals (not exactly the most efficient thing to do, but whatever)
            }
        }

        /// <summary>
        /// Allows the usage of Distinct(IEqualityComparer) without the need to implement an IEqualityComparer
        /// </summary>
        /// <typeparam name="T">Type of items in the enumerable</typeparam>
        /// <param name="enumerable">The enumerable to find distinct objects in</param>
        /// <param name="predicate">The comparison function that will be used</param>
        /// <returns>An enumerable of distinct obejcts, based on the provided predicate function</returns>
        public static IEnumerable<T> Distinct<T>(this IEnumerable<T> enumerable, Func<T, T, bool> predicate)
        {
            return enumerable.Distinct( new TComparer<T>(predicate) );
        }

        /// <summary>
        /// Allows the usage of Distinct(IEqualityComparer) without the need to implement an IEqualityComparer. Uses the class's own equality method
        /// </summary>
        /// <typeparam name="T">Type of items in the enumerable</typeparam>
        /// <param name="enumerable">The enumerable to find distinct objects in</param>
        /// <returns>An enumerable of distinct obejcts, based on the provided predicate function</returns>
        public static IEnumerable<T> Distinct<T>(this IEnumerable<T> enumerable) where T : class
        {
            Func<T, T, bool> pred = (a,b) => a == b;
            return enumerable.Distinct( new TComparer<T>(pred) );
        }
    }
}
