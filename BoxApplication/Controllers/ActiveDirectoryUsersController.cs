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
    public class ActiveDirectoryUsersController : Controller
    {
        private readonly BoxApplicationContext _context;

        public ActiveDirectoryUsersController(BoxApplicationContext context)
        {
            _context = context;
        }

        // GET: ActiveDirectoryUsers
        public async Task<IActionResult> Index()
        {
            return View(await _context.ActiveDirectoryUsers.ToListAsync());
        }

        // GET: ActiveDirectoryUsers/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activeDirectoryUser = await _context.ActiveDirectoryUsers
                .FirstOrDefaultAsync(m => m.ADEmail == id);
            if (activeDirectoryUser == null)
            {
                return NotFound();
            }

            return View(activeDirectoryUser);
        }

        // GET: ActiveDirectoryUsers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ActiveDirectoryUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ADEmail,ADUsername,ADFirstName,ADStatus,ADDateInactive")] ActiveDirectoryUser activeDirectoryUser)
        {
            if (ModelState.IsValid)
            {
                _context.Add(activeDirectoryUser);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(activeDirectoryUser);
        }

        // GET: ActiveDirectoryUsers/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activeDirectoryUser = await _context.ActiveDirectoryUsers.FindAsync(id);
            if (activeDirectoryUser == null)
            {
                return NotFound();
            }
            return View(activeDirectoryUser);
        }

        // POST: ActiveDirectoryUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("ADEmail,ADUsername,ADFirstName,ADStatus,ADDateInactive")] ActiveDirectoryUser activeDirectoryUser)
        {
            if (id != activeDirectoryUser.ADEmail)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(activeDirectoryUser);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ActiveDirectoryUserExists(activeDirectoryUser.ADEmail))
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
            return View(activeDirectoryUser);
        }

        // GET: ActiveDirectoryUsers/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activeDirectoryUser = await _context.ActiveDirectoryUsers
                .FirstOrDefaultAsync(m => m.ADEmail == id);
            if (activeDirectoryUser == null)
            {
                return NotFound();
            }

            return View(activeDirectoryUser);
        }

        // POST: ActiveDirectoryUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var activeDirectoryUser = await _context.ActiveDirectoryUsers.FindAsync(id);
            _context.ActiveDirectoryUsers.Remove(activeDirectoryUser);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ActiveDirectoryUserExists(string id)
        {
            return _context.ActiveDirectoryUsers.Any(e => e.ADEmail == id);
        }
    }
}
