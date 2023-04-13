using DataAccess.DBContexts.RITSDB;
using DataAccess.DBContexts.RITSDB.Models;
using DataAccess.Repositories.Base;
using DataAccess.Repositories.RITSDB.Interfaces;

namespace DataAccess.Repositories.RITSDB
{
    public class OrderRepository : BaseRepository<Order>, IOrderRepository
    {
        public OrderRepository(RITSDBContext context) : base(context)
        {

        }
    }
}
