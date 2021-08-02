using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DapperExtensions;
namespace LBJOEE
{
    public class DBImp<T> : OracleBaseFixture, IDB<T> where T : class, new()
    {
        public DBImp()
        {

        }
        public DBImp(string constr) : base(constr)
        {

        }
        public virtual dynamic Add(T entity)
        {
            return Db.Insert<T>(entity);
        }

        public virtual bool Add(List<T> entitys)
        {
            List<dynamic> retlist = new List<dynamic>();
            foreach (var item in entitys)
            {
                var ret = Db.Insert(item);
                retlist.Add(ret);
            }
            return entitys.Count == retlist.Count ? true : false;
        }

        public virtual bool Delete(T entity)
        {
            return Db.Delete<T>(entity);
        }

        public virtual bool Delete(List<T> entitys)
        {
            List<bool> retlist = new List<bool>();
            foreach (var item in entitys)
            {
                var ret = Db.Delete<T>(item);
                retlist.Add(ret);
            }
            return retlist.Where(t => t).Count() == entitys.Count ? true : false;
        }

        public virtual T Find(int id)
        {
            return Db.Get<T>(id);
        }

        public virtual bool Modify(T entity)
        {
            return Db.Update<T>(entity);
        }

        public virtual bool Modify(List<T> entitys)
        {
            List<bool> retlist = new List<bool>();
            foreach (var item in entitys)
            {
                var ret = Db.Update<T>(item);
                retlist.Add(ret);
            }
            return retlist.Where(t => t).Count() == entitys.Count ? true : false;
        }
    }
}
