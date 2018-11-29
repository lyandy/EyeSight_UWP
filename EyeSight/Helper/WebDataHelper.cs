//=====================================================
//Copyright (C) 2013-2015 All rights reserved
//CLR版本:      4.0.30319.42000
//命名空间:     EyeSight.Helper
//类名称:       WebDataHelper
//创建时间:     2015/9/21 星期一 15:49:17
//作者：        Andy.Li
//作者站点:     http://www.cnblogs.com/lyandy
//======================================================

using EyeSight.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EyeSight.Helper
{
    public class WebDataHelper : ClassBase<WebDataHelper>
    {
        public WebDataHelper() : base() { }

        public async Task<string> GetFromUrlWithAuthReturnString(string url, string formData, int timeout)
        {
            var uri = string.Empty;

            if (string.IsNullOrEmpty(formData))
            {
                uri = url + "&r=" + DateTime.Now.Ticks;
            }
            else if (formData.Contains("="))
            {
                uri = url + "?" + formData + "r=" + DateTime.Now.Ticks;
            }
            else
            {
                uri = url + formData;
            }

            try
            {
                Debug.WriteLine(uri);

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.ExpectContinue = false;
                    //client.Timeout = TimeSpan.FromSeconds(timeout);
                    using (var message = await client.GetAsync(uri))
                    {
                        message.EnsureSuccessStatusCode();
                        string result = await message.Content.ReadAsStringAsync();
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public async Task<string> PostToUrlWithAuthAndJsonReturnString(string url, string jsonData, string userName = "", string password = "")
        {
            string json = string.Empty;
            HttpWebResponse response = null;
            try
            {

                HttpWebRequest request = HttpWebRequest.CreateHttp(new Uri(url));

                if (userName == "")
                {
                    userName = "";
                }

                if (password == "")
                {
                    password = "";
                }

                request.Method = "POST";
                request.Credentials = new NetworkCredential(userName, password);
                //request.ContentType = "application/x-www-form-urlencoded";

                //返回应答请求异步操作的状态

                using (Stream stream = await Task.Factory.FromAsync<Stream>(request.BeginGetRequestStream, request.EndGetRequestStream, TaskCreationOptions.None))
                {
                    byte[] bs = Encoding.UTF8.GetBytes(jsonData);
                    await stream.WriteAsync(bs, 0, bs.Length);
                }

                response = (HttpWebResponse)await Task.Factory.FromAsync<WebResponse>(request.BeginGetResponse, request.EndGetResponse, TaskCreationOptions.None);
                using (Stream s = response.GetResponseStream())
                {
                    using (var sr = new StreamReader(s))
                    {
                        json = await sr.ReadToEndAsync();
                    }
                }
            }
            catch
            {
            }
            if (response != null)
            {
                response.Dispose();
            }
            return json;
        }

    }
}
