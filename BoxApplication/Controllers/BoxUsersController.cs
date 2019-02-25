using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BoxApplication.Models;
using Box.V2.JWTAuth;
using Box.V2.Config;
using Box.V2.Models;

namespace BoxApplication.Controllers
{
    public class BoxUsersController : Controller
    {
        private readonly BoxApplicationContext _context;
        private Box.V2.BoxClient boxclient;
        private readonly string workingDirectory = Environment.CurrentDirectory;

        //Stub for when we can test active directory calls
        private List<string> inactiveusers = new List<string> { "testuser1@domain.edu", "testuser2@domain.edu" };

        public Box.V2.BoxClient BoxConnection()
        {
            // Read in config file
            IBoxConfig config = null;
            using (FileStream fs = new FileStream(workingDirectory + "\\678301_96d24sif_config.json", FileMode.Open))
            {
                config = BoxConfig.CreateFromJsonFile(fs);
            }

            // Create JWT auth using config file
            var boxJWT = new BoxJWTAuth(config);

            // Create admin client
            var adminToken = boxJWT.AdminToken();
            var client = boxJWT.AdminClient(adminToken);

            return client;
        }

        public BoxUsersController(BoxApplicationContext context)
        {
            _context = context;
            boxclient = BoxConnection();
        }

        // GET: BoxUsers
        public async Task<IActionResult> Index()
        {
            //var boxApplicationContext = _context.BoxUser.Include(b => b.BoxEmail);
            //return View(await boxApplicationContext.ToListAsync());
            BoxCollection<Box.V2.Models.BoxUser> users = await boxclient.UsersManager.GetEnterpriseUsersAsync();
            IEnumerable<Box.V2.Models.BoxUser> usersdisplay = users.Entries.Where(item => inactiveusers.Contains(item.Login));
            return View(usersdisplay);
        }

        public async Task<IActionResult> RemoveInactiveAccounts()
        {
            var boxApplicationContext = _context.BoxUser.Include(b => b.BoxEmail);
            return View(await boxApplicationContext.ToListAsync());
        }

        // GET: BoxUsers/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boxUser = await _context.BoxUser
                .Include(b => b.BoxEmail)
                .FirstOrDefaultAsync(m => m.BoxID == id);
            if (boxUser == null)
            {
                return NotFound();
            }

            return View(boxUser);
        }

        // GET: BoxUsers/Create
        public IActionResult Create()
        {
            ViewData["BoxADForeignKey"] = new SelectList(_context.ActiveDirectoryUser, "ADEmail", "ADEmail");
            return View();
        }

        // POST: BoxUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BoxID,BoxADForeignKey,BoxName,BoxLogin,BoxSpaceUsed,BoxStatus,BoxDateCreated,BoxDateModified")] Models.BoxUser boxUser)
        {
            if (ModelState.IsValid)
            {
                _context.Add(boxUser);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BoxADForeignKey"] = new SelectList(_context.ActiveDirectoryUser, "ADEmail", "ADEmail", boxUser.BoxADForeignKey);
            return View(boxUser);
        }

        // GET: BoxUsers/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boxUser = await _context.BoxUser.FindAsync(id);
            if (boxUser == null)
            {
                return NotFound();
            }
            ViewData["BoxADForeignKey"] = new SelectList(_context.ActiveDirectoryUser, "ADEmail", "ADEmail", boxUser.BoxADForeignKey);
            return View(boxUser);
        }

        // POST: BoxUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("BoxID,BoxADForeignKey,BoxName,BoxLogin,BoxSpaceUsed,BoxStatus,BoxDateCreated,BoxDateModified")] Models.BoxUser boxUser)
        {
            if (id != boxUser.BoxID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(boxUser);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BoxUserExists(boxUser.BoxID))
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
            ViewData["BoxADForeignKey"] = new SelectList(_context.ActiveDirectoryUser, "ADEmail", "ADEmail", boxUser.BoxADForeignKey);
            return View(boxUser);
        }

        // GET: BoxUsers/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boxUser = await _context.BoxUser
                .Include(b => b.BoxEmail)
                .FirstOrDefaultAsync(m => m.BoxID == id);
            if (boxUser == null)
            {
                return NotFound();
            }

            return View(boxUser);
        }

        // POST: BoxUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var boxUser = await _context.BoxUser.FindAsync(id);
            _context.BoxUser.Remove(boxUser);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BoxUserExists(string id)
        {
            return _context.BoxUser.Any(e => e.BoxID == id);
        }
    }
}
