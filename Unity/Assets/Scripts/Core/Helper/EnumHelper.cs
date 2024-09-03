using System;

namespace ET
{
    public static class EnumHelper
    {
        public static int EnumIndex<T>(int value)
        {
            int i = 0;
            foreach (object v in Enum.GetValues(typeof (T)))
            {
                if ((int)v == value)
                {
                    return i;
                }

                ++i;
            }

            return -1;
        }

        public static T FromString<T>(string str)
        {
            if (!Enum.IsDefined(typeof (T), str))
            {
                return default (T);
            }

            return (T)Enum.Parse(typeof (T), str);
        }

        public static bool IsDefine<T>(int key)
        {
            return Enum.IsDefined(typeof (T), key);
        }

        public static int ToInt(this Enum e)
        {
            return e.GetHashCode();
        }
    }
}