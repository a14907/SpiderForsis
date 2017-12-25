using System;
using System.Collections.Generic;
using System.Text;

namespace SpiderForSis001.Data
{
    public class ErroeProcess
    {
        public int Id { get; set; }
        /// <summary>
        /// 资源类型（暂定:1-图片，2-种子，3-详情页）
        /// </summary>
        public int Type { get; set; }
        public string Url { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
