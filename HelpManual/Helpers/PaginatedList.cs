using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HelpManual.Entities;

namespace HelpManual.Helpers
{
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            this.AddRange(items);
        }

        public PaginatedList(List<T> items, int pageIndex, int totalPages)
        {
            PageIndex = pageIndex;
            TotalPages = totalPages;

            this.AddRange(items);
        }

        public bool HasPreviousPage
        {
            get
            {
                return (PageIndex > 1);
            }
        }

        public bool HasNextPage
        {
            get
            {
                return (PageIndex < TotalPages);
            }
        }

        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize = 5)
        {
            var count = await source.CountAsync();
            //Only gets the items for the current page
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }

        public static async Task<PaginatedList<T>> CreateAsyncHomePage(IQueryable<FormObject> source, int pageIndex, int totalPages)
        {
            var count = await source.CountAsync();
            //Only gets the items for the current page
            //Needs to be cast to a generic list for the AddRange method in the constructor
            List<T> items = source.Where(p => p.PageNo == pageIndex).Cast<T>().ToList();
            return new PaginatedList<T>(items, pageIndex, totalPages);
        }

        public static async Task<PaginatedList<T>> Create(IEnumerable<T> source, int pageIndex, int pageSize = 10)
        {
            var count = source.Count();
            //Only gets the items for the current page
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
}
