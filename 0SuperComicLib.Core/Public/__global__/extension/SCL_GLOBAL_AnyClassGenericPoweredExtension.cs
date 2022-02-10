using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperComicLib
{
    public static class SCL_GLOBAL_AnyClassGenericPoweredExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item1"></param>
        /// <param name="item2"></param>
        /// <returns></returns>
        public static ref T Swap<T>(this T item1, ref T item2)
        {

            return ref item2;
        }
    }
}
