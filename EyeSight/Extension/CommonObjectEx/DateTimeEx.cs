//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.Encrypt.CommonObjectEx
//类名称:       DateTimeEx
//创建时间:     2015/9/21 星期一 15:30:11
//作者：        Andy.Li
//作者站点:     http://www.cnblogs.com/lyandy
//======================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeSight.Extension.CommonObjectEx
{
    public static class DateTimeEx
    {
        public static long ToUnixTime(this DateTime dateTime)
        {
            var timeSpan = (dateTime - new DateTime(1970, 1, 1));
            var timestamp = (long)timeSpan.TotalMilliseconds;

            return timestamp;
        }

        public static DateTime ToDateTime(this long seconds)
        {
            var time = new DateTime(1970, 1, 1);
            time = time.AddMilliseconds(seconds);

            return time;
        }

        public static DateTime ToChinaStandardTime(this DateTime datetime)
        {
            try
            {
                //中国标准时间北京时间的Id为 China Standard Time，这是通过TimeZoneInfo.GetSystemTimeZone()方法看到的
                var timezone = TimeZoneInfo.FindSystemTimeZoneById("China Standard Time");

                DateTime time = TimeZoneInfo.ConvertTime(datetime, timezone);

                return time;
            }
            catch
            {
                return datetime;
            }
        }
    }
}
