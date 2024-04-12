using UnityEngine;
using VRC.SDK3.Data;
using UdonSharp;
using Yamadev.YamaStream.GenericDataContainer.Internal;

namespace Yamadev.YamaStream.GenericDataContainer
{
    // Copy from Koyashiro.GenericDataContainer (MIT license)
    [AddComponentMenu("")]
    public class DataList<T> : UdonSharpBehaviour
    {
        public static DataList<T> New()
        {
            return (DataList<T>)(object)new DataList();
        }

        public static DataList<T> New(params T[] array)
        {
            var tokens = DataTokenUtil.NewDataTokens(array);
            return (DataList<T>)(object)new DataList(tokens);
        }
    }
}
