using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UdonSharp;
using UnityEngine;

namespace Yamadev.YamachanWebUnit
{
    public static class Utils
    {
        public static void SetVariable<T>(this UdonSharpBehaviour self, string symbolName, T value)
        {
            var type = self.GetType();
            var field = type.GetField(symbolName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            field.SetValue(self, value);
        }
    }
}