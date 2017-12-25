using System;
using System.Collections.Generic;
using System.Text;

namespace SpiderForSis001.Data
{
    public class MoviePage
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsHandler { get; set; }
        /// <summary>
        /// 用户输入（暂定:有码，无码）
        /// </summary>
        public string Type { get; set; }
        public string Url { get; set; }
        public int DownloadCount { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
        public virtual List<Resource> Resources { get; set; }
    }
}
