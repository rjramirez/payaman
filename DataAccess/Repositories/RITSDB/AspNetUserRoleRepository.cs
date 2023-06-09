﻿using DataAccess.DBContexts.RITSDB;
using DataAccess.DBContexts.RITSDB.Models;
using DataAccess.Repositories.Base;
using DataAccess.Repositories.RITSDB.Interfaces;

namespace DataAccess.Repositories.RITSDB
{
    public class AspNetUserRoleRepository : BaseRepository<AspNetUserRole>, IAspNetUserRoleRepository
    {
        public AspNetUserRoleRepository(RITSDBContext context) : base(context)
        {

        }
    }
}
