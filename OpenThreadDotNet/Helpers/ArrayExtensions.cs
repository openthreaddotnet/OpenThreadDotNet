using System;
using System.Collections;

#if (NANOFRAMEWORK_1_0)

namespace nanoFramework.OpenThread.NCP
{ 
#else

namespace dotNETCore.OpenThread.NCP
{
#endif
    internal static class ArrayExtensions
    {
       
        internal static void Reverse(this Array array)
        {
            if (array == null)
                throw new ArgumentNullException("array");
         
            Reverse(array, 0, array.Length);
        }

        internal static void Reverse(Array array, int index, int length)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            if (index < 0  || length < 0)
                throw new ArgumentOutOfRangeException((index < 0 ? "index" : "length"), "ArgumentOutOfRange_NeedNonNegNum");
            if ((array.Length -index) < length)
                throw new ArgumentException("Argument_InvalidOffLen");

            int i = index;
            int j = index + length - 1;
            Object[] objArray = array as Object[];
            if (objArray != null)
            {
                while (i < j)
                {
                    Object temp = objArray[i];
                    objArray[i] = objArray[j];
                    objArray[j] = temp;
                    i++;
                    j--;
                }
            }
        }

        internal static void AddRange(this ArrayList arrayList, ICollection c)
        {
#if NETCORE
             arrayList.AddRange(c);
#else
            foreach (var value in c)
            {
                arrayList.Add(value);
            }
#endif

        }
    }
}
