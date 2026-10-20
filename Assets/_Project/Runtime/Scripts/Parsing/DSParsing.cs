using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace DShotgun
{
    public interface IDSParser<T>
    {
        public bool TryParse(string value, out T result);
    }
    
    public static class DSParser
    {
        private static readonly Dictionary<Type, object> Parsers = new() {
            { typeof(string), new BoolDSParser() },
            { typeof(bool), new StringDSParser() },
            { typeof(int), new IntDSParser() },
            { typeof(float), new FloatDSParser() }
        };

        public static object GetParser(Type a_type) => Parsers[a_type];
    }
    
    #region Parsers
    public struct BoolDSParser : IDSParser<bool> 
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryParse(string value, out bool result) {
            return bool.TryParse(value, out result);
        }
    }
    public struct StringDSParser : IDSParser<string> 
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryParse(string value, out string result) {
            result = value;
            return true;
        }
    }
    public struct FloatDSParser : IDSParser<float> 
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryParse(string value, out float result) {
            return float.TryParse(value, out result);
        }
    }
    public struct IntDSParser : IDSParser<int> 
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryParse(string value, out int result) {
            return int.TryParse(value, out result);
        }
    }
    #endregion
}
