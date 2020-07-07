using System;
using System.Collections.Generic;
using System.Linq;

namespace DddEfteling.Common.Controls
{
    public static class EnumExtensions
    {
        public static IEnumerable<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
    }
}
