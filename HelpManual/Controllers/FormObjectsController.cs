using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HelpManual.Entities;
using HelpManual.Helpers;
using System.Collections;
using Microsoft.AspNetCore.Authorization;

namespace HelpManual.Controllers
{
    [Authorize(Policy = "Admin")]
    public class FormObjectsController : Controller
    {
        private readonly HelpManualDbContext _context;

        public FormObjectsController(HelpManualDbContext context)
        {
            _context = context;
        }

        // GET: FormObjects
        public async Task<IActionResult> Index(string currentFilter, string searchString, int? page)
        {
            var HelpManualDbContext = _context.FormObject
                .Include(f => f.ObjectType)
                .OrderBy(f => f.PageNo)
                .ThenBy(f => f.Order);

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            var formObjects = from s in HelpManualDbContext
                              select s;

            ViewData["CurrentFilter"] = searchString;
            if (!String.IsNullOrEmpty(searchString))
            {
                formObjects = formObjects.Where(s => s.QuestionNo.Contains(searchString)
                                       || s.Order.ToString().Contains(searchString)
                                       || s.ObjectType.Name.Contains(searchString));
            }

            return View(await PaginatedList<FormObject>.CreateAsync(formObjects.AsNoTracking(), page ?? 1));
        }

        // GET: FormObjects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var formObject = await _context.FormObject
                .Include(f => f.ObjectType)
                .SingleOrDefaultAsync(m => m.FormObjectId == id);
            if (formObject == null)
            {
                return NotFound();
            }

            return View(formObject);
        }

        // GET: FormObjects/Create
        public IActionResult Create()
        {
            ViewData["ObjectTypeId"] = new SelectList(_context.ObjectTypes, "ObjectTypeId", "Name");
            ViewData["Orders"] = new SelectList(GetNextOrderNoList());
            ViewData["PageNo"] = new SelectList(GetPages());
            return View();
        }

        // POST: FormObjects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FormObjectId,Order,QuestionNo,PageNo,ObjectTypeId")] FormObject formObject, string save)
        {
            //This will only trigger when the dropdown has been changed
            if (save == null)
            {
                ViewData["Orders"] = new SelectList(GetNextOrderNoList(isEdit: false, pageNo: formObject.PageNo));
                ViewData["PageNo"] = new SelectList(GetPages());
            }
            else
            {
                if (ModelState.IsValid)
                {
                    UpdateOrderNos(formObject);
                    _context.Add(formObject);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            ViewData["ObjectTypeId"] = new SelectList(_context.ObjectTypes, "ObjectTypeId", "Name", formObject.ObjectTypeId);
            return View(formObject);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult GetNewOrders([Bind("FormObjectId,Order,QuestionNo,PageNo,ObjectTypeId")] FormObject formObject)
        //{
        //    GetNextOrderNoList(isEdit: false, pageNo: formObject.PageNo);
        //    ViewData["ObjectTypeId"] = new SelectList(_context.ObjectTypes, "ObjectTypeId", "Name", formObject.ObjectTypeId);
        //    return View(formObject);
        //}

        // GET: FormObjects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var formObject = await _context.FormObject.SingleOrDefaultAsync(m => m.FormObjectId == id);
            if (formObject == null)
            {
                return NotFound();
            }
            ViewData["ObjectTypeId"] = new SelectList(_context.ObjectTypes, "ObjectTypeId", "Name", formObject.ObjectTypeId);
            ViewData["Orders"] = new SelectList(GetNextOrderNoList(isEdit: true));
            ViewData["PageNo"] = new SelectList(GetPages());
            return View(formObject);
        }

        // POST: FormObjects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FormObjectId,Order,QuestionNo,PageNo,ObjectTypeId")] FormObject formObject, string save)
        {
            if (id != formObject.FormObjectId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //This will only trigger when the dropdown has been changed
                    if (save == null)
                    {
                        ViewData["Orders"] = new SelectList(GetNextOrderNoList(isEdit: false, pageNo: formObject.PageNo));
                        ViewData["PageNo"] = new SelectList(GetPages());
                    }
                    else
                    {
                        UpdateOrderNos(formObject, isEdit: true);

                        _context.Update(formObject);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FormObjectExists(formObject.FormObjectId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            ViewData["ObjectTypeId"] = new SelectList(_context.ObjectTypes, "ObjectTypeId", "Name", formObject.ObjectTypeId);
            return View(formObject);
        }

        // GET: FormObjects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var formObject = await _context.FormObject
                .Include(f => f.ObjectType)
                .SingleOrDefaultAsync(m => m.FormObjectId == id);
            if (formObject == null)
            {
                return NotFound();
            }

            return View(formObject);
        }

        // POST: FormObjects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var formObject = await _context.FormObject.SingleOrDefaultAsync(m => m.FormObjectId == id);
            _context.FormObject.Remove(formObject);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FormObjectExists(int id)
        {
            return _context.FormObject.Any(e => e.FormObjectId == id);
        }

        private List<int> GetPages()
        {
            //Gets the total number of pages plus one. 
            //Also checks that the array is not empty and that there are page numbers
            int maxPage = _context.FormObject.DefaultIfEmpty()
                .Max(p => p == null ? 0 : p.PageNo) + 1;

            List<int> pages = new List<int>();
            for (int i = 1; i <= maxPage; i++)
            {
                pages.Add(i);
            }
            return pages;
        }

        private List<int> GetNextOrderNoList(bool isEdit = false, int? pageNo = 0)
        {
            int maxOrder;
            if (pageNo > 0)
            {
                maxOrder = _context.FormObject
                .Where(p => p.PageNo == pageNo).DefaultIfEmpty()
                .Max(p => p == null ? 0 : p.Order) + 1;
            }
            else
            {
                maxOrder = _context.FormObject.DefaultIfEmpty()
                .Max(p => p == null ? 0 : p.Order) + 1;
            }
            //When editing only show the available order numbers
            if (isEdit)
            {
                maxOrder--;
            }
            List<int> orderNos = new List<int>();
            for (int i = 1; i <= maxOrder; i++)
            {
                orderNos.Add(i);
            }
            return orderNos.OrderByDescending(a => a).ToList();
        }

        private void UpdateOrderNos(FormObject formObject, bool isEdit = false)
        {
            FormObject oldFormObject = _context.FormObject.AsNoTracking().Where(x => x.FormObjectId == formObject.FormObjectId).SingleOrDefault();
            bool orderNoExists = _context.FormObject.Any(a => a.Order == formObject.Order && a.PageNo == formObject.PageNo);
            
            if (isEdit && orderNoExists)
            {
                //If neither of these are satisfied then the order number was not changed
                if (oldFormObject.Order < formObject.Order)
                {
                    GetLinqOrdersDecrease(formObject, oldFormObject);
                }
                else if (oldFormObject.Order > formObject.Order)
                {
                    GetLinqOrdersIncrease(formObject, oldFormObject);
                }
            }
            else if (orderNoExists)
            {
                //Always increase controls after this if this is a new object being added
                GetLinqOrdersIncrease(formObject);
            }
        }

        private void GetLinqOrdersIncrease(FormObject formObject)
        {
            //Increments the orders above this by one for the page that this form object is being put onto
            List<FormObject> ordersToUpdate;
            ordersToUpdate = _context.FormObject
                    .Where(a => a.Order >= formObject.Order && a.PageNo == formObject.PageNo)
                    .AsEnumerable()
                    .Select(c => { c.Order = c.Order + 1; return c; })
                    .ToList();
            UpdateOrdersInDb(ordersToUpdate);
        }

        private void GetLinqOrdersIncrease(FormObject formObject, FormObject oldFormObject)
        {
            //Increments the orders above this by one for the page that this form object is being put onto
            List<FormObject> ordersToUpdate;
            ordersToUpdate = _context.FormObject
                    .Where(a => a.Order >= formObject.Order && a.Order < oldFormObject.Order && a.PageNo == formObject.PageNo)
                    .AsEnumerable()
                    .Select(c => { c.Order = c.Order + 1; return c; })
                    .ToList();
            UpdateOrdersInDb(ordersToUpdate);
        }

        private void GetLinqOrdersDecrease(FormObject formObject, FormObject oldFormObject)
        {
            //Decrements the orders below this by one for the page that this form object is being put onto
            List<FormObject> ordersToUpdate;
            ordersToUpdate = _context.FormObject
                    .Where(a => a.Order <= formObject.Order && a.Order > oldFormObject.Order && a.PageNo == formObject.PageNo)
                    .AsEnumerable()
                    .Select(c => { c.Order = c.Order - 1; return c; })
                    .ToList();
            UpdateOrdersInDb(ordersToUpdate);
        }

        private void UpdateOrdersInDb(List<FormObject> formObjects)
        {
            foreach (FormObject formObject in formObjects)
            {
                _context.Update(formObject);
            }
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
