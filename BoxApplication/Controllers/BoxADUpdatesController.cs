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
    public class BoxADUpdatesController : Controller
    {
        private readonly BoxApplicationContext _context;

        public BoxADUpdatesController(BoxApplicationContext context)
        {
            _context = context;
        }


        // GET: BoxADUpdates
        public async Task<IActionResult> Index()
        {
            //creates list to hold all ad users from context
            List<ActiveDirectoryUser> activeDirectoryUsers = new List<ActiveDirectoryUser>();
            //adds all activedirectoryusers from context to empty list and sorts
            activeDirectoryUsers = _context.ActiveDirectoryUsers.ToList();
            activeDirectoryUsers.Sort();

            //creats list to hold all box users from context
            List<BoxUsers> boxUsers = new List<BoxUsers>();
            //adds all boxusers from context to empty list and sorts
            boxUsers = _context.BoxUsers.ToList();
            boxUsers.Sort();

            //creates empty list to hold potential updates
            List<BoxADUpdate> potentialUpdates = new List<BoxADUpdate>();

            foreach(var adUser in activeDirectoryUsers)
            {
                foreach(var boxUser in boxUsers)
                {
                    if(adUser.NeedsUpdate(adUser, boxUser) == "ADEmail")
                    {
                        BoxADUpdate potentialUpdate = new BoxADUpdate();
                        potentialUpdate.ADUser = adUser;
                        potentialUpdate.ADFieldChanged = "AD Email";
                        potentialUpdate.ADNewData = adUser.ADEmail;
                        potentialUpdate.BoxPreviousData = boxUser.Login;
                        potentialUpdate.UpdateBoxOption = false;
                        //potentialUpdate.UserID = adUser.ADUserPrimaryKey;
                        
                        potentialUpdates.Add(potentialUpdate);
                    }
                }
            }

            foreach(BoxADUpdate potentialUpdate in potentialUpdates)
            {
                if (_context.BoxADUpdates.Any(x => x == potentialUpdate))
                {
                    //potentialUpdate.Status = "Ignore?";
                    _context.BoxADUpdates.Update(potentialUpdate);
                }
                else
                {
                    _context.BoxADUpdates.Add(potentialUpdate);
                }
            }     

            return View(await _context.BoxADUpdates.ToListAsync());
        }

        // GET: BoxADUpdates/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boxADUpdate = await _context.BoxADUpdates
                .FirstOrDefaultAsync(m => m.BoxADUpdateID == id);
            if (boxADUpdate == null)
            {
                return NotFound();
            }

            return View(boxADUpdate);
        }

        // GET: BoxADUpdates/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BoxADUpdates/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BoxADUpdateID,UserID,UpdateBoxOption,ADFieldChanged,BoxPreviousData,ADNewData")] BoxADUpdate boxADUpdate)
        {
            if (ModelState.IsValid)
            {
                boxADUpdate.BoxADUpdateID = Guid.NewGuid();
                _context.Add(boxADUpdate);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(boxADUpdate);
        }

        // GET: BoxADUpdates/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boxADUpdate = await _context.BoxADUpdates.FindAsync(id);
            if (boxADUpdate == null)
            {
                return NotFound();
            }
            return View(boxADUpdate);
        }

        // POST: BoxADUpdates/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("BoxADUpdateID,UserID,UpdateBoxOption,ADFieldChanged,BoxPreviousData,ADNewData")] BoxADUpdate boxADUpdate)
        {
            if (id != boxADUpdate.BoxADUpdateID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(boxADUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BoxADUpdateExists(boxADUpdate.BoxADUpdateID))
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
            return View(boxADUpdate);
        }

        // GET: BoxADUpdates/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boxADUpdate = await _context.BoxADUpdates
                .FirstOrDefaultAsync(m => m.BoxADUpdateID == id);
            if (boxADUpdate == null)
            {
                return NotFound();
            }

            return View(boxADUpdate);
        }

        // POST: BoxADUpdates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var boxADUpdate = await _context.BoxADUpdates.FindAsync(id);
            _context.BoxADUpdates.Remove(boxADUpdate);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BoxADUpdateExists(Guid id)
        {
            return _context.BoxADUpdates.Any(e => e.BoxADUpdateID == id);
        }
    }
}
