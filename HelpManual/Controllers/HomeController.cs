using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HelpManual.Models;
using HelpManual.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Diagnostics;
using HelpManual.Helpers;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace HelpManual.Controllers
{
    public class HomeController : Controller
    {
        private HelpManualDbContext _context;

        public HomeController(HelpManualDbContext ctx)
        {
            _context = ctx;
        }

        public async Task<IActionResult> Index(int? page)
        {
            int pageNo = page ?? 1;
            var HelpManualDbContext = _context.FormObject
                .Include(f => f.ObjectType.ControlType)
                .OrderBy(x => x.Order);

            List<int> pages = new List<int>();
            foreach (var fo in HelpManualDbContext)
            {
                pages.Add(fo.PageNo);
            }

            int? maxPage = HttpContext.Session.GetInt32("MaxPage");

            //Remembers the max page using a session variable so that if the user goes back to another page
            //the page that they left will still be enabled
            if (maxPage == null)
            {
                HttpContext.Session.SetInt32("MaxPage", pageNo);
            }
            else if (maxPage < pageNo)
            {
                HttpContext.Session.SetInt32("MaxPage", pageNo);
            }

            ViewBag.MaxPage = HttpContext.Session.GetInt32("MaxPage");

            string userName = HttpContext.User.Identity.Name.ToLowerInvariant();
            _context.Add(new UserAccess(userName, pageNo));
            await _context.SaveChangesAsync();

            return View(await PaginatedList<FormObject>.CreateAsyncHomePage(HelpManualDbContext.AsNoTracking(), 
                pageNo, pages.Distinct().Count()));
        }

        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> GetUsage(string currentFilter, string searchString, int? page)
        {
            var HelpManualDbContext = _context.UserAccess;
            var userAccess = from s in HelpManualDbContext
                              select s;

            CheckSearchString(currentFilter, ref searchString, ref page);

            if (!String.IsNullOrEmpty(searchString))
            {
                userAccess = userAccess.Where(s => s.FullName.Contains(searchString)
                                    || s.PageNo.Equals(searchString));
            }

            return View(await PaginatedList<UserAccess>.CreateAsync(userAccess.AsNoTracking(), page ?? 1));
        }

        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> GetUsagePage(string currentFilter, string searchString, int? page)
        {
            var usagePages = _context.UserAccess
                            .GroupBy(x => x.PageNo)
                            .Select(group => new GetPagesTotalViewModel
                            {
                                PageNo = group.Key,
                                Total = group.Count()
                            }).ToList(); 

            var pages = from s in usagePages
                             select s;

            CheckSearchString(currentFilter, ref searchString, ref page);

            if (!String.IsNullOrEmpty(searchString))
            {
                pages = pages.Where(s => s.PageNo.ToString().Equals(searchString));
            }

            return View(await PaginatedList<GetPagesTotalViewModel>.Create(pages, page ?? 1));
        }

        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> GetUsagePageUser(string currentFilter, string searchString, int? page)
        {
            var usagePages = _context.UserAccess
                            .GroupBy(x => new { x.PageNo, x.FullName })
                            .Select(group => new GetPagesUserTotalViewModel
                            {
                                FullName = group.Key.FullName,
                                PageNo = group.Key.PageNo,
                                Total = group.Count()
                            }).ToList();

            var pages = from s in usagePages
                        select s;

            CheckSearchString(currentFilter, ref searchString, ref page);

            if (!String.IsNullOrEmpty(searchString))
            {
                pages = pages.Where(s => s.FullName.Contains(searchString)
                                    || s.PageNo.Equals(searchString));
            }

            return View(await PaginatedList<GetPagesUserTotalViewModel>.Create(pages, page ?? 1));
        }

        public bool IsPrime(double number)
        {
            if (number <= 1) return false;
            if (number == 2) return true;
            if (number % 2 == 0) return false;

            var boundary = (int)Math.Floor(Math.Sqrt(number));

            for (int i = 3; i <= boundary; i += 2)
                if (number % i == 0)
                    return false;

            return true;

            //if (prime == 1)
            //{
            //    return false;
            //}
            //return true;
        }

        private void CheckSearchString(string currentFilter, ref string searchString, ref int? page)
        {
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;
        }

        public IActionResult Error()
        {
            var exception = HttpContext.Features.Get<IExceptionHandlerFeature>();

            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, ErrorMessage = exception.Error.Message });
        }

        protected override void Dispose(bool disposing)
        {
            if (_context != null)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
