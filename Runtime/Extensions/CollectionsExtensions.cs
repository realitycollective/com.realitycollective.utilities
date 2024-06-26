﻿// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace RealityCollective.Utilities.Extensions
{
    /// <summary>
    /// Extension methods for .Net Collection objects, e.g. Lists, Dictionaries, Arrays
    /// </summary>
    public static class CollectionsExtensions
    {
        /// <summary>
        /// Creates a read-only wrapper around an existing collection.
        /// </summary>
        /// <typeparam name="TElement">The type of element in the collection.</typeparam>
        /// <param name="elements">The collection to be wrapped.</param>
        /// <returns>The new, read-only wrapper around <paramref name="elements"/>.</returns>
        public static ReadOnlyCollection<TElement> AsReadOnly<TElement>(this IList<TElement> elements)
        {
            return new ReadOnlyCollection<TElement>(elements);
        }

        /// <summary>
        /// Creates a read-only copy of an existing collection.
        /// </summary>
        /// <typeparam name="TElement">The type of element in the collection.</typeparam>
        /// <param name="elements">The collection to be copied.</param>
        /// <returns>The new, read-only copy of <paramref name="elements"/>.</returns>
        public static ReadOnlyCollection<TElement> ToReadOnlyCollection<TElement>(this IEnumerable<TElement> elements)
        {
            return elements.ToArray().AsReadOnly();
        }

        /// <summary>
        /// Inserts an item in its sorted position into an already sorted collection. This is useful if you need to consume the
        /// collection in between insertions and need it to stay correctly sorted the whole time. If you just need to insert a
        /// bunch of items and then consume the sorted collection at the end, it's faster to add all the elements and then use
        /// <see cref="List{T}.Sort()"/> at the end.
        /// </summary>
        /// <typeparam name="TElement">The type of element in the collection.</typeparam>
        /// <param name="elements">The collection of sorted elements to be inserted into.</param>
        /// <param name="toInsert">The element to insert.</param>
        /// <param name="comparer">The comparer to use when sorting or null to use <see cref="Comparer{T}.Default"/>.</param>
        /// <returns></returns>
        public static int SortedInsert<TElement>(this List<TElement> elements, TElement toInsert, IComparer<TElement> comparer = null)
        {
            var effectiveComparer = comparer ?? Comparer<TElement>.Default;

            if (Application.isEditor)
            {
                for (int iElement = 0; iElement < elements.Count - 1; iElement++)
                {
                    var element = elements[iElement];
                    var nextElement = elements[iElement + 1];

                    if (effectiveComparer.Compare(element, nextElement) > 0)
                    {
                        Debug.LogWarning("Elements must already be sorted to call this method.");
                        break;
                    }
                }
            }

            int searchResult = elements.BinarySearch(toInsert, effectiveComparer);

            int insertionIndex = searchResult >= 0
                ? searchResult
                : ~searchResult;

            elements.Insert(insertionIndex, toInsert);

            return insertionIndex;
        }

        /// <summary>
        /// Disposes of all non-null elements in a collection.
        /// </summary>
        /// <typeparam name="TElement">The type of element in the collection.</typeparam>
        /// <param name="elements">The collection of elements to be disposed.</param>
        public static void DisposeElements<TElement>(this IEnumerable<TElement> elements)
            where TElement : IDisposable
        {
            foreach (var element in elements)
            {
                if (element != null)
                {
                    element.Dispose();
                }
            }
        }

        /// <summary>
        /// Disposes of all non-null elements in a collection.
        /// </summary>
        /// <typeparam name="TElement">The type of element in the collection.</typeparam>
        /// <param name="elements">The collection of elements to be disposed.</param>
        public static void DisposeElements<TElement>(this IList<TElement> elements)
            where TElement : IDisposable
        {
            for (int iElement = 0; iElement < elements.Count; iElement++)
            {
                var element = elements[iElement];

                if (element != null)
                {
                    element.Dispose();
                }
            }
        }

        /// <summary>
        /// Exports the values of a uint indexed Dictionary as an Array.
        /// </summary>
        /// <typeparam name="T">Type of data stored in the values of the Dictionary.</typeparam>
        /// <param name="input">Dictionary to be exported.</param>
        /// <returns>array in the type of data stored in the Dictionary.</returns>
        public static T[] ExportDictionaryValuesAsArray<T>(this Dictionary<uint, T> input)
        {
            T[] output = new T[input.Count];
            input.Values.CopyTo(output, 0);
            return output;
        }

        /// <summary>
        /// Validate if a list contains an item and add it if not found.
        /// </summary>
        /// <typeparam name="T">Data type used in the List.</typeparam>
        /// <param name="list">The instance of the List to validate.</param>
        /// <param name="item">The item of Type T to add to the list if not found</param>
        /// <returns>True if a new item was added to the collection</returns>
        public static bool EnsureListItem<T>(this IList<T> list, T item)
        {
            if (!list.Contains(item))
            {
                list.Add(item);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Validate if a list contains items from another collection and adds items if not found, essentially merging the lists.
        /// </summary>
        /// <typeparam name="T">Data type used in the List.</typeparam>
        /// <param name="list">The instance of the List to validate.</param>
        /// <param name="items">The array of items of Type T to add to the list if not found</param>
        public static void EnsureListItems<T>(this IList<T> list, T[] items)
        {
            foreach (var item in items)
            {
                list.EnsureListItem(item);
            }
        }

        /// <summary>
        /// Validate if a list contains items from another collection and adds items if not found, essentially merging the lists.
        /// </summary>
        /// <typeparam name="T">Data type used in the List.</typeparam>
        /// <param name="list">The instance of the List to validate.</param>
        /// <param name="items">The List of items of Type T to add to the list if not found</param>
        public static void EnsureListItems<T>(this IList<T> list, IList<T> items)
        {
            foreach (var item in items)
            {
                list.EnsureListItem(item);
            }
        }

        /// <summary>
        /// Safely removes an item from a <see cref="List{T}"/> if it is found.
        /// </summary>
        /// <typeparam name="T">Data type used in the List.</typeparam>
        /// <param name="list">The instance of the List to validate.</param>
        /// <param name="item">The item of Type T to remove to the list if not found</param>
        /// <returns>True if the item was removed from the collection</returns>
        public static void SafeRemoveListItem<T>(this IList<T> list, T item)
        {
            if (list != null && item != null && list.Contains(item))
            {
                list.Remove(item);
            }
        }

        /// <summary>
        /// Safely removes an item from a <see cref="List{T}"/> if it is found.
        /// </summary>
        /// <typeparam name="T">Data type used in the List.</typeparam>
        /// <param name="list">The instance of the List to validate.</param>
        /// <param name="item">The item of Type T to remove to the list if not found</param>
        /// <returns>True if the item was removed from the collection</returns>
        public static bool TrySafeRemoveListItem<T>(this IList<T> list, T item)
        {
            if (list != null && item != null && list.Contains(item))
            {
                list.SafeRemoveListItem(item);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Validate if a <see cref="Dictionary{TKey, TValue}"/> contains an item and add it if not found."/>
        /// </summary>
        /// <remarks>
        /// Will update the existing value when found unless overridden using the 'update' parameter
        /// </remarks>
        /// <typeparam name="TKey">Data type used in the Dictionary Key.</typeparam>
        /// <typeparam name="TValue">Data type used in the Dictionary Value.</typeparam>
        /// <param name="dictionary">The instance of the <see cref="Dictionary{TKey, TValue}"/> to validate.</param>
        /// <param name="key">The Key of a <see cref="KeyValuePair{TKey, TValue}"/> to validate against the dictionary with.</param>
        /// <param name="value">The Value of a <see cref="KeyValuePair{TKey, TValue}"/> to set the dictionary item with if required.</param>
        /// <param name="update">By default, the Ensure function will override the existing dictionary value if found, if this is not required it can be overridden with this bool.  Setting this to <see cref="false"/> will leave the dictionary item untouched if found.</param>
        /// <returns>True if a new item was added to the collection</returns>
        public static bool EnsureDictionaryItem<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value, bool update = true)
        {
            if (!dictionary.TryGetValue(key, out _))
            {
                dictionary.Add(key, value);
                return true;
            }

            if (update)
            {
                dictionary[key] = value;
            }
            return false;
        }

        /// <summary>
        /// Safely removes an item from a <see cref="Dictionary{TKey, TValue}"/> if it is found.
        /// </summary>
        /// <typeparam name="TKey">Data type used in the Dictionary Key.</typeparam>
        /// <typeparam name="TValue">Data type used in the Dictionary Value.</typeparam>
        /// <param name="dictionary">The instance of the <see cref="Dictionary{TKey, TValue}"/> to validate.</param>
        /// <param name="key">The Key of a <see cref="KeyValuePair{TKey, TValue}"/> to validate against the dictionary with.</param>
        public static void SafeRemoveDictionaryItem<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
        {
            if (dictionary != null && key != null && dictionary.TryGetValue(key, out _))
            {
                dictionary.Remove(key);
            }
        }

        /// <summary>
        /// Safely removes an item from a <see cref="Dictionary{TKey, TValue}"/> if it is found.
        /// </summary>
        /// <typeparam name="TKey">Data type used in the Dictionary Key.</typeparam>
        /// <typeparam name="TValue">Data type used in the Dictionary Value.</typeparam>
        /// <param name="dictionary">The instance of the <see cref="Dictionary{TKey, TValue}"/> to validate.</param>
        /// <param name="key">The Key of a <see cref="KeyValuePair{TKey, TValue}"/> to validate against the dictionary with.</param>
        /// <returns>True if an item was removed from the collection</returns>
        public static bool TrySafeRemoveDictionaryItem<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
        {
            if (dictionary != null && key != null && dictionary.TryGetValue(key, out _))
            {
                dictionary.SafeRemoveDictionaryItem(key);
                return true;
            }

            return false;
        }
    }
}
