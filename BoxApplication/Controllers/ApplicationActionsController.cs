using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BoxApplication.Models;

namespace BoxApplication.Controllers
{
    public class ApplicationActionsController : Controller
    {
        private readonly BoxApplicationContext _context;

        public ApplicationActionsController(BoxApplicationContext context)
        {
            _context = context;
        }

        // GET: ApplicationActions
        public async Task<IActionResult> Index()
        {
            var boxApplicationContext = _context.Action.Include(a => a.ApplicationActionADUser);
            return View(await boxApplicationContext.ToListAsync());
        }

        // GET: ApplicationActions/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationAction = await _context.Action
                .Include(a => a.ApplicationActionADUser)
                .FirstOrDefaultAsync(m => m.ApplicationActionID == id);
            if (applicationAction == null)
            {
                return NotFound();
            }

            return View(applicationAction);
        }

        // GET: ApplicationActions/Create
        public IActionResult Create()
        {
            ViewData["ApplicationActionADForeignKey"] = new SelectList(_context.ActiveDirectoryUser, "ADEmail", "ADEmail");
            return View();
        }

        // POST: ApplicationActions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ApplicationActionID,ApplicationActionADForeignKey,ApplicationActionType,ApplicationActionDescription,ApplicationActionObjectModified,ApplicationActionDate")] ApplicationAction applicationAction)
        {
            if (ModelState.IsValid)
            {
                applicationAction.ApplicationActionID = Guid.NewGuid();
                _context.Add(applicationAction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationActionADForeignKey"] = new SelectList(_context.ActiveDirectoryUser, "ADEmail", "ADEmail", applicationAction.ApplicationActionADForeignKey);
            return View(applicationAction);
        }

        // GET: ApplicationActions/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationAction = await _context.Action.FindAsync(id);
            if (applicationAction == null)
            {
                return NotFound();
            }
            ViewData["ApplicationActionADForeignKey"] = new SelectList(_context.ActiveDirectoryUser, "ADEmail", "ADEmail", applicationAction.ApplicationActionADForeignKey);
            return View(applicationAction);
        }

        // POST: ApplicationActions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("ApplicationActionID,ApplicationActionADForeignKey,ApplicationActionType,ApplicationActionDescription,ApplicationActionObjectModified,ApplicationActionDate")] ApplicationAction applicationAction)
        {
            if (id != applicationAction.ApplicationActionID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(applicationAction);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApplicationActionExists(applicationAction.ApplicationActionID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApplicationActionADForeignKey"] = new SelectList(_context.ActiveDirectoryUser, "ADEmail", "ADEmail", applicationAction.ApplicationActionADForeignKey);
            return View(applicationAction);
        }

        // GET: ApplicationActions/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationAction = await _context.Action
                .Include(a => a.ApplicationActionADUser)
                .FirstOrDefaultAsync(m => m.ApplicationActionID == id);
            if (applicationAction == null)
            {
                return NotFound();
            }

            return View(applicationAction);
        }

        // POST: ApplicationActions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var applicationAction = await _context.Action.FindAsync(id);
            _context.Action.Remove(applicationAction);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ApplicationActionExists(Guid id)
        {
            return _context.Action.Any(e => e.ApplicationActionID == id);
        }







        // Search stuff


        // GET: ApplicationActions/Search
        public IActionResult Search()
        {
            var list = new List<ApplicationAction>
            {
                new ApplicationAction()
            };
            return View(list);
        }

        // POST: ApplicationActions/Search
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Search([Bind("ApplicationActionID,ApplicationActionADForeignKey,ApplicationActionType,ApplicationActionDescription,ApplicationActionObjectModified,ApplicationActionDate")] ApplicationAction actionSearch)
        {
            var searchResults = from action in _context.Action
                                where (actionSearch.ApplicationActionADForeignKey      == null || action.ApplicationActionADForeignKey   == actionSearch.ApplicationActionADForeignKey)
                                      && (actionSearch.ApplicationActionType           == null || action.ApplicationActionType == actionSearch.ApplicationActionType)
                                      && (actionSearch.ApplicationActionDescription    == null || action.ApplicationActionDescription    == actionSearch.ApplicationActionDescription)
                                      && (actionSearch.ApplicationActionObjectModified == null || action.ApplicationActionObjectModified == actionSearch.ApplicationActionObjectModified)
                                select action;
            var searchResultsList = searchResults.ToList();
            searchResultsList.Insert(0, new ApplicationAction());

            return View(searchResultsList);
        }

    }
}
