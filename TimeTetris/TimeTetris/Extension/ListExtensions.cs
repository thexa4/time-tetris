using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeTetris.Extension
{
    public static class ListExtensions
    {

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T Pop<T>(this List<T> list)
        {
            var last = list.Last();
            list.Remove(last);
            return last;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T UnShift<T>(this List<T> list)
        {
            var first = list.First();
            list.Remove(first);
            return first;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="item"></param>
        public static void Shift<T>(this List<T> list, T item)
        {
            list.Insert(0, item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="item"></param>
        public static void Push<T>(this List<T> list, T item)
        {
            list.Add(item);
        }
    }
}
