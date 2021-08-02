using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LBJOEE
{
    public interface IDB<T> where T:class,new()
    {
        dynamic Add(T entity);
        bool Modify(T entity);
        bool Delete(T entity);
        bool Add(List<T> entitys);
        bool Modify(List<T> entitys);
        bool Delete(List<T> entitys);
        T Find(int id);
    }
}
