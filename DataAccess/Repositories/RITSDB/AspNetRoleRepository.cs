﻿using DataAccess.DBContexts.RITSDB;
using DataAccess.DBContexts.RITSDB.Models;
using DataAccess.Repositories.Base;
using DataAccess.Repositories.RITSDB.Interfaces;

namespace DataAccess.Repositories.RITSDB
{
    public class AspNetRoleRepository : BaseRepository<AspNetRole>, IAspNetRoleRepository
    {
        public AspNetRoleRepository(RITSDBContext context) : base(context)
        {

        }
    }
}
