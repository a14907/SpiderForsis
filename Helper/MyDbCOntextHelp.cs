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
                db.MoviePages.Add(model);
                return db.SaveChanges() >= 1;
            }
        }

        public static bool AddResource(Resource model)
        {
            using (var db = new MyDbContext())
            {
                db.Resources.Add(model);
                return db.SaveChanges() >= 1;
            }
        }
        public static bool AddErroeProcess(ErroeProcess model)
        {
            using (var db = new MyDbContext())
            {
                db.ErroeProcesses.Add(model);
                return db.SaveChanges() >= 1;
            }
        }

        public static bool AddResourceList(List<Resource> model)
        {
            using (var db = new MyDbContext())
            {
                db.Resources.AddRange(model);
                return db.SaveChanges() >= model.Count;
            }
        }
        public static bool ExistMovie(string name)
        {
            using (var db = new MyDbContext())
            {
                return db.MoviePages.Any(m=>m.Name==name);
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
