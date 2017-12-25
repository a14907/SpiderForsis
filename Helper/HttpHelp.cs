using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SpiderForSis001.Helper
{
    public static class HttpHelp
    {
        private static HttpClient client = new HttpClient();
        public static string GetPageString(string url)
        {
            string res = null;
            int t = 0;
            do
            {
                t++;
                try
                {
                    var request = WebRequest.Create(url) as HttpWebRequest;
                    request.Method = "GET";
                    using (var response = request.GetResponse())
                    {
                        using (var resStream = response.GetResponseStream())
                        {
                            using (var sr = new StreamReader(resStream, Encoding.GetEncoding("GBK")))
                            {
                                res = sr.ReadToEnd();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelp.Log("页面获取失败：" + url + "," + ex.Message + "," + ex.StackTrace, true);
                    res = null;
                }
            } while ((res == null || res.Length == 0) && t <= 2);
            return res;
        }

        public static async Task<string> GetPageStringAsync(string url)
        {
            string res = null;
            int t = 0;
            do
            {
                t++;
                try
                {
                    var respMsg = await client.GetAsync(url);
                    using (var resStream = await respMsg.Content.ReadAsStreamAsync())
                    {
                        using (var sr = new StreamReader(resStream, Encoding.GetEncoding("GBK")))
                        {
                            res = sr.ReadToEnd();
                        }
                    }

                }
                catch (Exception ex)
                {
                    LogHelp.Log("页面获取失败：" + url + "," + ex.Message + "," + ex.StackTrace, true);
                    res = null;
                }
            } while ((res == null || res.Length == 0) && t <= 2);
            return res;
        }
        public static bool DownloadFile(string url, string fileName)
        {
            bool res = false;
            int t = 0;
            do
            {
                t++;
                try
                {
                    //如果文件存在，则不需要再接受，直接返回
                    if (File.Exists(fileName) && new FileInfo(fileName).Length != 0)
                    {
                        return true;
                    }
                    var request = WebRequest.Create(url) as HttpWebRequest;
                    request.Method = "GET";
                    request.KeepAlive = true;
                    using (var response = request.GetResponse())
                    {
                        using (var resStream = response.GetResponseStream())
                        {
                            //如果文件存在，则不需要再接受，直接返回
                            if (File.Exists(fileName) && new FileInfo(fileName).Length == response.ContentLength)
                            {
                                return true;
                            }
                            using (var FileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                            {
                                byte[] buf = new byte[1024 * 500];
                                int blen = 0;
                                do
                                {
                                    blen = resStream.Read(buf, 0, buf.Length);
                                    if (blen != 0)
                                        FileStream.Write(buf, 0, blen);
                                } while (blen != 0);
                            }
                            res = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelp.Log("bt文件获取失败：" + url + "," + ex.Message + "," + ex.StackTrace, true);
                    res = false;
                }
            } while (res == false && t <= 2);
            return res;
        }
        public static async Task DownloadFileAsync(string url, string path)
        {
            try
            {
                var respMsg = await client.GetAsync(url);
                using (var resStream = await respMsg.Content.ReadAsStreamAsync())
                {
                    var fileName = Path.Combine(path, GetFileName(respMsg.Content.Headers.GetValues("Content-Disposition").First())) + DateTime.Now.Ticks;
                    using (var FileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                    {
                        byte[] buf = new byte[1024 * 500];
                        Console.WriteLine("buf len:" + buf.Length);
                        int blen = 0;
                        int sum = 0;
                        do
                        {
                            blen = resStream.Read(buf, 0, buf.Length);
                            Console.WriteLine("blen:" + blen);
                            sum += blen;
                            if (blen != 0)
                                FileStream.Write(buf, 0, blen);
                        } while (blen != 0);
                        Console.WriteLine("sum:" + sum);
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        public static Regex regexForFileName = new Regex("filename=\"(.+?)\"", RegexOptions.Compiled);
        private static string GetFileName(string disposition)
        {
            var m = regexForFileName.Match(disposition);
            if (m != null && m.Length > 1)
            {
                return m.Groups[1].Value;
            }
            return null;
        }

        public static bool DownloadImg(string url, string path)
        {
            bool res = false;
            int t = 0;
            do
            {
                t++;
                try
                {
                    var fileName = Path.Combine(path, Path.GetFileName(url));//如果文件存在，则不需要再接受，直接返回
                    if (File.Exists(fileName) && new FileInfo(fileName).Length != 0)
                    {
                        res = true;
                    }
                    var request = WebRequest.Create(url) as HttpWebRequest;
                    request.Method = "GET";
                    using (var response = request.GetResponse())
                    {
                        using (var resStream = response.GetResponseStream())
                        {
                            //如果文件存在，则不需要再接受，直接返回
                            if (File.Exists(fileName) && new FileInfo(fileName).Length == response.ContentLength)
                            {
                                res = true;
                            }
                            using (var FileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                            {
                                byte[] buf = new byte[1024 * 500];
                                int blen = 0;
                                do
                                {
                                    blen = resStream.Read(buf, 0, buf.Length);
                                    if (blen != 0)
                                        FileStream.Write(buf, 0, blen);
                                } while (blen != 0);
                            }
                            res = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelp.Log("图片获取失败：" + url + "," + ex.Message + "," + ex.StackTrace, true);
                    res = false;
                }
            } while (res == false && t <= 2);
            return res;
        }
        public static async Task<bool> DownloadImgAsync(string url, string path)
        {
            bool res = false;
            int t = 0;
            do
            {
                t++;
                try
                {
                    var fileName = Path.Combine(path, Path.GetFileName(url));//如果文件存在，则不需要再接受，直接返回
                    if (File.Exists(fileName) && new FileInfo(fileName).Length != 0)
                    {
                        res = true;
                    }
                    var respMsg = await client.GetAsync(url);
                    using (var resStream = await respMsg.Content.ReadAsStreamAsync())
                    {
                        //如果文件存在，则不需要再接受，直接返回
                        if (File.Exists(fileName) && new FileInfo(fileName).Length == respMsg.Content.Headers.ContentLength)
                        {
                            res = true;
                        }
                        using (var FileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                        {
                            byte[] buf = new byte[1024 * 500];
                            int blen = 0;
                            do
                            {
                                blen = resStream.Read(buf, 0, buf.Length);
                                if (blen != 0)
                                    FileStream.Write(buf, 0, blen);
                            } while (blen != 0);
                        }
                        res = true;
                    }

                }
                catch (Exception ex)
                {
                    LogHelp.Log("图片获取失败：" + url + "," + ex.Message + "," + ex.StackTrace, true);
                    res = false;
                }
            } while (res == false && t <= 2);
            return res;
        }
    }
}
