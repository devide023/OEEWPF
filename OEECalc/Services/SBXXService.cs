using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DapperExtensions;
using Dapper;
using DapperExtensions.Predicate;
using OEECalc.Model;
namespace OEECalc.Services
{
    public class SBXXService:OracleBaseFixture
    {
        public IEnumerable<base_sbxx> Get_SBXX_List()
        {
            try
            {
                PredicateGroup pg = new PredicateGroup()
                {
                    Operator = GroupOperator.And,
                    Predicates = new List<IPredicate>()
                };
                pg.Predicates.Add(Predicates.Field<base_sbxx>(t => t.scbz, Operator.Eq, "N"));
                return Db.GetList<base_sbxx>(pg);
            }
            catch (Exception)
            {
                return new List<base_sbxx>();
            }
        }
    }
}
