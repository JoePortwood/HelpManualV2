using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace HelpManual.Entities
{
    public class UserAccess
    {
        public int UserAccessId { get; set; }
        public string UserId { get; set; }
        public DateTime DateAccessed { get; set; }
        public int PageNo { get; set; }
        public string FullName { get; set; }

        public UserAccess(string userName, int pageNo)
        {
            UserId = userName;
            DateAccessed = DateTime.Now;
            PageNo = pageNo;
            FullName = userName;
        }

        public UserAccess GetUserId(string userName, int pageNo)
        {
            //return SqlQuery("spGetUser {0}, {1}", userName, pageNo);
            using (var context = new HelpManualDbContext())
            {
                var result = context.UserAccess
                .FromSql("spGetUser {0}, {1}", userName, pageNo)
                .SingleOrDefault();

                return result;
            }
        }

        //public IQueryable<UserAccess> GetUserPageTotal()
        //{
        //    return SqlQuery("spGetUserTotal");
        //}

        //public IQueryable<UserAccess> GetPageTotal()
        //{
        //    return SqlQuery("spGetPageTotal");
        //}

        //private IQueryable<UserAccess> SqlQuery(string queryName, params object[] list)
        //{
        //    using (var context = new HelpManualDbContext())
        //    {
        //        var result = context.UserAccess
        //        .FromSql(queryName, list);
        //        //.ToList();

        //        return result;
        //    }
        //}
    }
}