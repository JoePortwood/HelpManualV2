using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HelpManual.Entities;
using HelpManual.Helpers;
using Microsoft.AspNetCore.Http;
using System.IO;
using HelpManual.Models;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;

namespace HelpManual.Controllers
{
    [Authorize(Policy = "Admin")]
    public class ObjectTypesController : Controller
    {
        private readonly HelpManualDbContext _context;

        public ObjectTypesController(HelpManualDbContext context)
        {
            _context = context;
        }

        // GET: ObjectTypes
        public async Task<IActionResult> Index(string currentFilter, string searchString, int? page)
        {
            var helpManualDbContext = _context.ObjectTypes.Include(o => o.ControlType);

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            var objectTypes = from s in helpManualDbContext
                              select s;

            ViewData["CurrentFilter"] = searchString;
            if (!String.IsNullOrEmpty(searchString))
            {
                objectTypes = objectTypes.Where(s => s.Name.Contains(searchString)
                                       || s.Data.Contains(searchString));
            }

            return View(await PaginatedList<ObjectType>.CreateAsync(objectTypes, page ?? 1));
        }

        // GET: ObjectTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var objectType = await _context.ObjectTypes
                .Include(o => o.ControlType)
                .SingleOrDefaultAsync(m => m.ObjectTypeId == id);
            if (objectType == null)
            {
                return NotFound();
            }

            return View(objectType);
        }

        // GET: ObjectTypes/Create
        public IActionResult Create()
        {
            ViewData["ControlTypeId"] = new SelectList(_context.ControlTypes, "ControlTypeId", "Name");
            return View();
        }

        // POST: ObjectTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ObjectTypeId,Name,Data,Image,Options,StartEnd,ControlTypeId")] ObjectTypeViewModel objectTypeViewModel)
        {
            ObjectType objectType = new ObjectType { };
            if (ModelState.IsValid)
            {
                //Only save if the image is valid or not required
                if (CheckImage(objectTypeViewModel))
                {
                    objectType = await AssignObjectType(objectTypeViewModel);
                    _context.Add(objectType);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            ViewData["ControlTypeId"] = new SelectList(_context.ControlTypes, "ControlTypeId", "Name", objectType.ControlTypeId);
            return View(objectTypeViewModel);
        }

        // GET: ObjectTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ObjectType objectType = await _context.ObjectTypes.Include(f => f.ControlType).SingleOrDefaultAsync(m => m.ObjectTypeId == id);
            ObjectTypeViewModel objectTypeViewModel = AssignViewModel(objectType);
            if (objectTypeViewModel == null)
            {
                return NotFound();
            }
            ViewData["ControlTypeId"] = new SelectList(_context.ControlTypes, "ControlTypeId", "Name", objectType.ControlTypeId);
            return View(objectTypeViewModel);
        }

        // POST: ObjectTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ObjectTypeId,Name,Data,Image,Options,StartEnd,ControlTypeId")] ObjectTypeViewModel objectTypeViewModel)
        {
            ObjectType objectType = new ObjectType { };
            if (id != objectTypeViewModel.ObjectTypeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //Only update if the image is valid or not required
                    if (CheckImage(objectTypeViewModel))
                    {
                        objectType = await AssignObjectType(objectTypeViewModel);
                        _context.Update(objectType);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ObjectTypeExists(objectType.ObjectTypeId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            ViewData["ControlTypeId"] = new SelectList(_context.ControlTypes, "ControlTypeId", "Name", objectType.ControlTypeId);
            return View(objectTypeViewModel);
        }

        // GET: ObjectTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var objectType = await _context.ObjectTypes
                .Include(o => o.ControlType)
                .SingleOrDefaultAsync(m => m.ObjectTypeId == id);
            if (objectType == null)
            {
                return NotFound();
            }

            return View(objectType);
        }

        // POST: ObjectTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var objectType = await _context.ObjectTypes
                .Include(o => o.ControlType)
                .SingleOrDefaultAsync(m => m.ObjectTypeId == id);
            try
            {
                _context.ObjectTypes.Remove(objectType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException e)
            {
                if (e.GetBaseException() is SqlException sqlException)
                {
                    if (sqlException.Errors.Count > 0)
                    {
                        switch (sqlException.Errors[0].Number)
                        {
                            case 547: // Foreign Key violation
                                ModelState.AddModelError("CodeInUse", "Object Type could not be deleted because a Form Object is referencing it");
                                return View(objectType);
                            default:
                                throw;
                        }
                    }
                }
                else
                {
                    throw;
                }
            }
            return View(objectType);
        }

        private bool ObjectTypeExists(int id)
        {
            return _context.ObjectTypes.Any(e => e.ObjectTypeId == id);
        }

        private bool CheckImage(ObjectTypeViewModel objectTypeViewModel)
        {
            objectTypeViewModel.ControlType = _context.ControlTypes.Where(x => x.ControlTypeId == objectTypeViewModel.ControlTypeId).SingleOrDefault();

            //If this is meant to be an image but another file type is used
            if (objectTypeViewModel.ControlType.Name == "Image" &&
                HttpPostedFileBaseExtensions.IsImage(objectTypeViewModel.Image) == false)
            {
                ModelState.AddModelError("", "Please select an image file");
                return false;
            }
            else
            {
                return true;
            }
        }

        private async Task<ObjectType> AssignObjectType(ObjectTypeViewModel objectTypeViewModel)
        {
            ObjectType objectType = new ObjectType
            {
                ControlType = objectTypeViewModel.ControlType,
                ControlTypeId = objectTypeViewModel.ControlTypeId,
                Data = objectTypeViewModel.Data,
                FormObject = objectTypeViewModel.FormObject,
                Name = objectTypeViewModel.Name,
                ObjectTypeId = objectTypeViewModel.ObjectTypeId,
                Options = objectTypeViewModel.Options,
                StartEnd = objectTypeViewModel.StartEnd,
            };

            //Only read from the stream if this is an image
            if (objectTypeViewModel.ControlType.Name == "Image")
            {
                using (var memoryStream = new MemoryStream())
                {
                    await objectTypeViewModel.Image.CopyToAsync(memoryStream);
                    objectType.Image = memoryStream.ToArray();
                }
            }
            return objectType;
        }

        private ObjectTypeViewModel AssignViewModel(ObjectType objectType)
        {
            ObjectTypeViewModel objectTypeViewModel = new ObjectTypeViewModel
            {
                ControlType = objectType.ControlType,
                ControlTypeId = objectType.ControlTypeId,
                Data = objectType.Data,
                FormObject = objectType.FormObject,
                ImageBytes = objectType.Image,
                Name = objectType.Name,
                ObjectTypeId = objectType.ObjectTypeId,
                Options = objectType.Options,
                StartEnd = objectType.StartEnd,
            };

            return objectTypeViewModel;
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
