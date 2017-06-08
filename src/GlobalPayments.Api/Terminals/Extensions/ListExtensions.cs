using System;
using System.Collections.Generic;

namespace GlobalPayments.Api.Terminals.Extensions {
    internal static class ListExtensions {
        public static void ForEach<T>(this List<T> list, Action<T> action) {
            foreach (T item in list)
                action(item);
        }
    }
}
