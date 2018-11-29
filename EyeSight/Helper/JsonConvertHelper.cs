//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.Helper
//类名称:       JsonConvertHelper
//创建时间:     2015/9/21 星期一 15:47:40
//作者：        Andy.Li
//作者站点:     http://www.cnblogs.com/lyandy
//======================================================

using EyeSight.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace EyeSight.Helper
{
    public class JsonConvertHelper : ClassBase<JsonConvertHelper>
    {
        public JsonConvertHelper() : base() { }

        public T DeserializeObject<T>(Stream stream)
        {
            try
            {
                if (stream != null)
                {
                    DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(T));
                    return (T)js.ReadObject(stream);
                }
            }
            catch (Exception ex) { }
            return default(T);
        }

        public T DeserializeObject<T>(string json)
        {
            try
            {
                if (!string.IsNullOrEmpty(json))
                {
                    using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                    {
                        DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(T));
                        return (T)js.ReadObject(ms);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
            return default(T);
        }
        public string SerializeObject<T>(T obj)
        {
            try
            {
                DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(T));

                using (var stream = new MemoryStream())
                {
                    json.WriteObject(stream, obj);
                    stream.Seek(0, SeekOrigin.Begin);
                    byte[] b = new byte[stream.Length];
                    stream.Read(b, 0, b.Length);
                    return Encoding.UTF8.GetString(b, 0, b.Length);
                }
            }
            catch { }
            return null;
        }
    }
}
