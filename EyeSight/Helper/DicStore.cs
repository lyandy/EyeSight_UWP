//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.Helper
//类名称:       DicStore
//创建时间:     2015/9/21 星期一 15:46:39
//作者：        Andy.Li
//作者站点:     http://www.cnblogs.com/lyandy
//======================================================

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeSight.Helper
{
    public class DicStore
    {
        private static ConcurrentDictionary<string, object> Dictionaries = new ConcurrentDictionary<string, object>();

        public static bool AddOrUpdateValue<T>(string key, T value)
        {
            try
            {
                if (Dictionaries.ContainsKey(key))
                {
                    Dictionaries[key] = value;
                }
                else
                {
                    return Dictionaries.TryAdd(key, value);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
        public static T GetValueOrDefault<T>(string key, T defaultValue)
        {
            if (Dictionaries.ContainsKey(key))
            {
                return (T)Dictionaries[key];
            }
            else
            {
                Dictionaries.TryAdd(key, defaultValue);
                return defaultValue;
            }
        }

        public static void Clear()
        {
            Dictionaries.Clear();
        }

        public static bool RemoveKey(string key)
        {
            if (Dictionaries != null && Dictionaries.ContainsKey(key))
            {
                object value = null;
                return Dictionaries.TryRemove(key, out value);
            }
            else
            {
                return true;
            }
        }
    }
}
