using SpiderForSis001.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace SpiderForSis001.Helper
{
    public class Spider
    {
        private readonly int _threadCount;
        private readonly string _baseDir;
        private readonly string _url;
        private readonly Uri _uri;
        private readonly string _typeName;
        private readonly int? _frontCount;
        public readonly Semaphore _semaphore;
        private int _totalPage;
        private string _firstPageStr;
        private Regex _regTotalPage = new Regex("class=\"last\">[.]{3} ([0-9]+?)</a>", RegexOptions.Compiled);
        private Regex _regA = new Regex("<a href=\"(.+?)\".*?>(.+?)</a>", RegexOptions.Compiled);
        private Regex _regBt = new Regex("<a href=\"(.+?)\" target=\"_blank\">(.+?)</a>", RegexOptions.Compiled);
        private Regex _regDownloadCount = new Regex("下载次数: ([0-9]*)", RegexOptions.Compiled);
        private Regex _regImgArea = new Regex("<div id=\"postmessage_[0-9]+\" class=\"t_msgfont\">[\\s\\S]*?[】][\\s\\S]*?</div>", RegexOptions.Compiled);
        private Regex _regImg = new Regex("<img src=\"(.*?)\"", RegexOptions.Compiled);
        private string _startPart;
        private string _endPart = "</table>";
        private List<string> _pageList;
        private bool _isFinish = false;
        private Random _random = new Random();
        private string[] _unvalidFileParts = new string[] { "\\", "/", ":", "*", "?", "<", ">", "|", "\"" };
        public bool IsFinish
        {
            get
            {
                return _isFinish;
            }
        }

        public Spider(string url, string typeName, int? frontCount, int threadCount = 40)
        {
            _url = url;
            _uri = new Uri(url);
            _typeName = typeName;
            _frontCount = frontCount;
            _threadCount = threadCount;
            _baseDir = Path.Combine(Directory.GetCurrentDirectory(), typeName);
            _semaphore = new Semaphore(threadCount, threadCount);

            var reg = new Regex("forum-([0-9]+)-[0-9]+[.]html$");
            var partstr = reg.Match(_url).Groups[1].Value;
            _startPart = string.Format("<table summary=\"forum_{0}\" id=\"forum_{0}\" cellspacing=\"0\" cellpadding=\"0\">", partstr);
        }

        internal async Task RunAsync()
        {
            //准备环境，创建目录
            Init();
            //获取总页数
            if (await ProcessTotalPageCountAsync() == false)
            {
                LogHelp.Log("=================================操作失败。。。", true);
                return;
            }

            //循环处理，
            for (int i = 0; i < _totalPage; i++)
            {
                LogHelp.Log("正在处理第{0}/{1}页。。。", i + 1, _totalPage, true);
                //处理当前分页数据
                await ProcessPageAsync(i);
            }
            for (int i = 0; i < _threadCount; i++)
            {
                _semaphore.WaitOne();
            }
            LogHelp.Log("处理完毕！。。。。", true);
            Program.isFinish = true;
            _isFinish = true;
        }

        private async Task ProcessPageAsync(int index)
        {
            //第一页数据缓存过不需要处理
            if (index == 0)
            {
                ProcessAsync(_firstPageStr ?? await HttpHelp.GetPageStringAsync(_pageList[index]));
            }
            else
            {
                ProcessAsync(await HttpHelp.GetPageStringAsync(_pageList[index]));
            }
        }
        private int[] waitopt = new int[] { 100, 200, 300, 400 };
        private void ProcessAsync(string pageStr)
        {
            try
            {
                if (pageStr == null || pageStr.Length == 0 || pageStr.Contains("您无权进行当前操作，这可能因以下原因之一造成"))
                {
                    return;
                }
                var startPos = pageStr.LastIndexOf(_startPart) + _startPart.Length;
                var endPos = pageStr.IndexOf(_endPart, startPos);
                var dataArea = pageStr.Substring(startPos, endPos - startPos);

                var ms = _regA.Matches(dataArea);
                LogHelp.Log("本页一共{0}个链接需要判断。。。", ms.Count, true);
                for (int i = 0; i < ms.Count; i++)
                {
                    var item = ms[i];
                    var u = item.Groups[1].Value;
                    var name = item.Groups[2].Value;
                    bool isnum = false; int num = 0;
                    isnum = int.TryParse(name, out num);
                    if (isnum || u.Contains(".php") || name.Contains("<"))
                    {
                        //LogHelp.Log("进度：{0}/{1},不符合要求。。。", i, ms.Count);
                        continue;
                    }
                    LogHelp.Log("进度：{0}/{1}，符合要求", i, ms.Count);
                    //获取详情页的信息
                    _semaphore.WaitOne();
                    Thread.Sleep(waitopt[_random.Next(1000) % 4]);
                    ThreadPool.QueueUserWorkItem(ProcessDetailAsync, item);
                    //ProcessDetailAsync(item);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private async void ProcessDetailAsync(object stat)
        {
            try
            {
                var item = stat as Match;
                MoviePage pageModel = null;
                string movieName = ValidFileName(item.Groups[2].Value);
                var movieUrl = _uri.Scheme + "://" + _uri.Authority + "/bbs/" + item.Groups[1].Value;
                string moviedir = null;

                if (MyDbCOntextHelp.ExistMovie(movieUrl))
                {
                    pageModel = MyDbCOntextHelp.QueryMovie(m => m.Url == movieUrl);
                    moviedir = Path.Combine(_baseDir, pageModel.Name);
                }
                else
                {
                    pageModel = new MoviePage
                    {
                        CreateTime = DateTime.Now,
                        UpdateTime = DateTime.Now,
                        Name = movieName,
                        Url = movieUrl,
                        IsHandler = false,
                        Type = _typeName,
                    };

                    LogHelp.Log("影片：" + pageModel.Name);
                    moviedir = Path.Combine(_baseDir, pageModel.Name);
                }
                if (!Directory.Exists(moviedir))
                {
                    Directory.CreateDirectory(moviedir);
                }
                if (pageModel.Id != 0 || MyDbCOntextHelp.AddPicturePage(pageModel))
                {
                    bool res = false;
                    var detailPageString = await HttpHelp.GetPageStringAsync(pageModel.Url);
                    if (detailPageString == null || detailPageString.Length == 0 || detailPageString.Contains("您无权进行当前操作，这可能因以下原因之一造成"))
                    {
                        return;
                    }
                    //下载次数
                    var m = _regDownloadCount.Match(detailPageString);
                    if (m.Groups.Count == 2)
                    {
                        pageModel.DownloadCount = int.Parse(m.Groups[1].Value);
                    }
                    LogHelp.Log("影片下载次数：" + pageModel.DownloadCount);
                    //图片
                    var imgAreaStr = _regImgArea.Match(detailPageString).Value;

                    var mimgs = _regImg.Matches(imgAreaStr);
                    var resList = new List<Resource>(mimgs.Count + 1);
                    for (int j = 0; j < mimgs.Count; j++)
                    {
                        var imgitem = mimgs[j];
                        var r = new Resource
                        {
                            CreateTime = DateTime.Now,
                            UpdateTime = DateTime.Now,
                            PicturePageId = pageModel.Id,
                            IsHandler = false,
                            Type = 1,
                            Url = imgitem.Groups[1].Value
                        };
                        if (!r.Url.StartsWith("http"))
                        {
                            r.Url = _uri.Scheme + "://" + _uri.Authority + "/bbs/" + r.Url;
                        }
                        resList.Add(r);
                    }
                    LogHelp.Log("截图{0}张.....bt文件一个", mimgs.Count);
                    //bt 检查重复
                    var btRes = new Resource
                    {
                        CreateTime = DateTime.Now,
                        UpdateTime = DateTime.Now,
                        PicturePageId = pageModel.Id,
                        Type = 2,
                        IsHandler = false,
                    };
                    var p1 = detailPageString.IndexOf("检查重复</a>");
                    if (p1 == -1)
                    {
                        return;
                    }
                    var startbt = detailPageString.IndexOf("<a href=\"", p1);
                    var endbt = detailPageString.IndexOf("</a>", startbt);
                    var bta = detailPageString.Substring(startbt, endbt - startbt + 4);
                    var mbt = _regBt.Match(bta);
                    btRes.Name = mbt.Groups[2].Value;
                    btRes.Url = _uri.Scheme + "://" + _uri.Authority + "/bbs/" + mbt.Groups[1].Value;
                    resList.Add(btRes);

                    MyDbCOntextHelp.AddResourceList(resList);
                    for (int i = 0; i < resList.Count - 1; i++)
                    {
                        res = await HttpHelp.DownloadImgAsync(resList[i].Url, moviedir);
                    }
                    res = await HttpHelp.DownloadFileAsync(btRes.Url, Path.Combine(moviedir, btRes.Name));
                }
                LogHelp.Log("处理完毕：" + movieName);
                return;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private void Init()
        {
            if (!File.Exists(_baseDir))
            {
                Directory.CreateDirectory(_baseDir);
                LogHelp.Log("目录创建完成：" + _baseDir);
            }
            LogHelp.Log("目录存在：" + _baseDir);
        }

        private async Task<bool> ProcessTotalPageCountAsync()
        {
            if (_frontCount != null)
            {
                _totalPage = _frontCount.Value;
                LogHelp.Log("总分页数为：" + _totalPage);
                GeneratePageList();
                return true;
            }
            _firstPageStr = await HttpHelp.GetPageStringAsync(_url);
            if (_firstPageStr == null || _firstPageStr.Length == 0)
            {
                return false;
            }
            var m = _regTotalPage.Match(_firstPageStr);
            if (m.Groups.Count != 2)
            {
                return false;
            }
            _totalPage = int.Parse(m.Groups[1].Value);
            LogHelp.Log("总分页数为：" + _totalPage);
            GeneratePageList();
            return true;
        }

        private void GeneratePageList()
        {
            Regex reg = new Regex("-([0-9]+)[.]html");
            var gs = reg.Match(_url).Groups;
            _pageList = new List<string>(_totalPage);

            var template = _url.Replace(gs[0].Value, "-{0}.html");
            int startIndex = int.Parse(gs[1].Value);

            for (int i = 0; i < _totalPage; i++)
            {
                _pageList.Add(string.Format(template, startIndex + i));
            }
        }

        public string ValidFileName(string fileName)
        {
            string newfilename = fileName;
            for (int i = 0; i < _unvalidFileParts.Length; i++)
            {
                if (fileName.Contains(_unvalidFileParts[i]))
                {
                    newfilename = fileName.Replace(_unvalidFileParts[i], "");
                }
            }

            return newfilename;
        }
    }
}
