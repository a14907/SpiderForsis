using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SpiderForSis001.Data;
using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using SpiderForSis001.Helper;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading.Tasks;

namespace SpiderForSis001
{

    class Program
    {
        static Program()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using (var db = new MyDbContext())
            {
                db.Database.EnsureCreated();
                db.Database.Migrate();
                if (!db.MoviePages.Any(m=>m.Id==1))
                {
                    db.MoviePages.Add(new MoviePage
                    {
                        Name = "默认"
                    });
                    db.SaveChanges();
                }
            }
        }
        /// <summary>
        /// 参数：0-url，1-文件夹名称,2-爬取页数（默认全部）,3-线程数
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            int argLen = args.Length;
            if (argLen==1 && args[0]=="/?")
            {
                Console.WriteLine("参数：0-url，1-文件夹名称,2-爬取页数（默认全部）,3-线程数");
                return;
            }
            if (argLen < 2)
            {
                Console.WriteLine("参数错误");
                return;
            }
            var spider = new Spider(args[0],
                args[1],
                argLen >= 3 ? (int?)int.Parse(args[2]) : null,
                argLen == 4 ? int.Parse(args[3]) : 20);
            Console.WriteLine("Begin");
            new TaskFactory().StartNew(()=> spider.RunAsync()).Unwrap().GetAwaiter().GetResult();
            


            Console.WriteLine("End");
        }
    }
}
