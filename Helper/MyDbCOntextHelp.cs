using SpiderForSis001.Data;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace SpiderForSis001.Helper
{
    public class MyDbCOntextHelp
    {

        public static bool AddPicturePage(MoviePage model)
        {
            using (var db = new MyDbContext())
            {
                if (db.MoviePages.Any(m=>m.Url==model.Url))
                {
                    return true;
                }
                db.MoviePages.Add(model);
                return db.SaveChanges() >= 1;
            }
        }

        public static bool SetHandled(MoviePage page,List<Resource> rs)
        {
            using (var db = new MyDbContext())
            {
                if (page!=null)
                {
                    page.IsHandler=true;
                    var p=db.Entry<MoviePage>(page);
                    p.State=EntityState.Detached;
                    p.Property(m=>m.IsHandler).IsModified=true;
                }
               
                if (rs!=null && rs.Count!=0)
                {
                    for(int i=0;i<rs.Count;i++)
                    {
                        var item=rs[i];
                        item.IsHandler=true;
                        var obj=db.Entry(item);
                        obj.State=EntityState.Detached;
                        obj.Property(m=>m.IsHandler).IsModified=true;
                    }
                }
                int res=db.SaveChanges();
                 
                return  res>= ( (page==null?0:1) + (rs==null || rs.Count==0?0: rs.Count));
            }
        }

        public static bool AddResource(Resource model)
        {
            using (var db = new MyDbContext())
            {
                if (db.Resources.Any(m => m.Url == model.Url))
                {
                    return true;
                }
                db.Resources.Add(model);
                return db.SaveChanges() >= 1;
            }
        }
        public static bool AddErroeProcess(ErroeProcess model)
        {
            using (var db = new MyDbContext())
            {
                if (db.ErroeProcesses.Any(m => m.Url == model.Url))
                {
                    return true;
                }
                db.ErroeProcesses.Add(model);
                return db.SaveChanges() >= 1;
            }
        }

        public static bool AddResourceList(List<Resource> model)
        {
            using (var db = new MyDbContext())
            {
                for (int i = 0; i < model.Count; i++)
                {
                    var item = model[i];
                    if (db.Resources.Any(m => m.Url == item.Url))
                    {
                        model.Remove(item);
                    }
                }
                db.Resources.AddRange(model);
                return db.SaveChanges() >= model.Count;
            }
        }
        public static bool ExistMovie(string movieUrl)
        {
            using (var db = new MyDbContext())
            {
                if (db.MoviePages.Any(m => m.Url == movieUrl))
                {
                    return true;
                }
                return db.MoviePages.Any(m=>m.Name==movieUrl);
            }
        }

        public static MoviePage QueryMovie(Expression<Func<MoviePage, bool>> func)
        {
            using (var db = new MyDbContext())
            {                
                return db.MoviePages.FirstOrDefault(func);
            }
        }
    }
}
