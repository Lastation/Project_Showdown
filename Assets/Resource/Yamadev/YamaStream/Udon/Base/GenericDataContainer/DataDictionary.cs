using UnityEngine;
using VRC.SDK3.Data;
using UdonSharp;

namespace Yamadev.YamaStream.GenericDataContainer
{
    // Copy from Koyashiro.GenericDataContainer (MIT license)
    [AddComponentMenu("")]
    public class DataDictionary<TKey, TValue> : UdonSharpBehaviour
    {
        public static DataDictionary<TKey, TValue> New()
        {
            return (DataDictionary<TKey, TValue>)(object)new DataDictionary();
        }
    }
}
