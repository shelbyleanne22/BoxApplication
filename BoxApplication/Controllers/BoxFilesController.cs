using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BoxApplication.Models;
using Box.V2.Config;
using System.IO;
using Box.V2.JWTAuth;
using Box.V2.Models;
using Box.V2;

namespace BoxApplication.Controllers
{
    public class BoxFilesController : Controller
    {
        private readonly BoxApplicationContext _context;
        private Box.V2.BoxClient boxclient;
        private readonly string workingDirectory = Environment.CurrentDirectory;

        public Box.V2.BoxClient BoxConnection()
        {
            // Read in config file
            IBoxConfig config = null;
            using (FileStream fs = new FileStream(workingDirectory + "\\678301_s116imjm_config.json", FileMode.Open))
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

        public BoxFilesController(BoxApplicationContext context)
        {
            _context = context;
            boxclient = BoxConnection();
        }



        // GET: BoxFiles
        public async Task<IActionResult> Index()
        {
            //list for our database
            List<Models.BoxFile> adminObjects = new List<Models.BoxFile>();
            List<string> serviceAccounts = new List<string>();
            serviceAccounts.Add("242840337");

            BoxCollection<BoxItem> results = await boxclient.SearchManager.SearchAsync("", ownerUserIds: serviceAccounts);

            List<BoxItem> boxFolderItemsList = results.Entries;

            foreach (BoxItem item in boxFolderItemsList)
            {
                Models.BoxFile boxObject = new Models.BoxFile();
                boxObject.BoxFileID = item.Id;
                boxObject.BoxFileName = item.Name;
                boxObject.BoxFileType = item.Type;
                adminObjects.Add(boxObject);
            }

            return View(adminObjects);
        }

        // GET: BoxFiles/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id.Equals(null))
            {
                return NotFound();
            }

            var boxFile = await _context.BoxFile
                .FirstOrDefaultAsync(m => m.BoxFileID.Equals(id));
            if (boxFile == null)
            {
                return NotFound();
            }

            return View(boxFile);
        }

        // GET: BoxFiles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BoxFiles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BoxFileID,BoxFileType,BoxFileName")] Models.BoxFile boxFile)
        {
            if (ModelState.IsValid)
            {
                _context.Add(boxFile);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(boxFile);
        }

        // GET: BoxFiles/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id.Equals(null))
            {
                return NotFound();
            }

            var boxFile = await _context.BoxFile.FindAsync(id);
            if (boxFile == null)
            {
                return NotFound();
            }
            return View(boxFile);
        }

        // POST: BoxFiles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("BoxFileID,BoxFileType,BoxFileName")] Models.BoxFile boxFile)
        {
            if (!id.Equals(boxFile.BoxFileID))
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(boxFile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BoxFileExists(boxFile.BoxFileID))
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
            return View(boxFile);
        }

        // GET: BoxFiles/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boxFile = await _context.BoxFile
                .FirstOrDefaultAsync(m => m.BoxFileID.Equals(id));
            if (boxFile == null)
            {
                return NotFound();
            }

            return View(boxFile);
        }

        // POST: BoxFiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var boxFile = await _context.BoxFile.FindAsync(id);
            _context.BoxFile.Remove(boxFile);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BoxFileExists(string id)
        {
            return _context.BoxFile.Any(e => e.BoxFileID.Equals(id));
        }
    }
}
