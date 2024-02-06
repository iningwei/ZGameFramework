using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AYellowpaper.SerializedCollections
{
    public interface IKeyable
    {
        void RecalculateOccurences();
        IReadOnlyList<int> GetOccurences(object key);
        IEnumerable Keys { get; }

        void AddKey(object key);
        void RemoveKey(object key);
        void RemoveAt(int index);
        object GetKeyAt(int index);
        int GetCount();
        void RemoveDuplicates();
    }
}