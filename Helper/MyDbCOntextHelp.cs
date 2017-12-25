using SpiderForSis001.Data;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;

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
