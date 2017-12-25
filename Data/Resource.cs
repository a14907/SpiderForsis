using System;
using System.Collections.Generic;
using System.Text;

namespace SpiderForSis001.Data
{
    public class Resource
    {
        public int Id { get; set; }
        public int PicturePageId { get; set; } = 1;
        /// <summary>
        /// 是否下载
        /// </summary>
        public bool IsHandler { get; set; }
        /// <summary>
        /// 资源类型（暂定:1-图片，2-种子）
        /// </summary>
        public int Type { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
        public virtual MoviePage PicturePage { get; set; }
    }
}
