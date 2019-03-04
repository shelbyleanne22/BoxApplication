using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BoxApplication.Models;
using System.DirectoryServices;
using System.Net;
using System.DirectoryServices.Protocols;

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
            List<ActiveDirectoryUser> inactiveADusers = new List<ActiveDirectoryUser>();

            string DomainPath = "LDAP://hi-root03.mcghi.mcg.edu:389/OU=students/sccs,DC=mcg,DC=edu/";
            string username = "";
            string password = "";
    
            var credentials = new NetworkCredential(username, password);
            var serverId = new LdapDirectoryIdentifier("hi - root03.mcghi.mcg.edu:389");

            var conn = new LdapConnection(serverId, credentials);
            try
            {
                conn.Bind();
            }
            catch (Exception)
            {
                
            }


            //creates directoryentry object that binds the instance to the domain path
            DirectoryEntry searchRoot = new DirectoryEntry(DomainPath);
            //creates a directorysearcher object which searches for all users in the domain
            DirectorySearcher search = new DirectorySearcher(searchRoot);
            //filters the search to only inactive/disabled accounts
            search.Filter = "(&(objectCategory=person)(objectClass=user)(userAccountControl:1.2.840.113556.1.4.803:=2))";
            search.PropertiesToLoad.Add("samaccountname");
            search.PropertiesToLoad.Add("mail");
            search.PropertiesToLoad.Add("displayname");
            SearchResult result;
            SearchResultCollection resultCol = search.FindAll();

            for(int counter = 0; counter < resultCol.Count; counter++)
            {
                string UserNameEmailString = string.Empty;
                result = resultCol[counter];
                if (result.Properties.Contains("samaccountname") &&
                         result.Properties.Contains("mail") &&
                    result.Properties.Contains("displayname"))
                {
                    ActiveDirectoryUser activeDirectoryUser = new ActiveDirectoryUser();
                    activeDirectoryUser.ADEmail = (String)result.Properties["mail"][0];
                    activeDirectoryUser.ADFirstName = (String)result.Properties["displayname"][0];
                    activeDirectoryUser.ADUsername = (String)result.Properties["samaccountname"][0];
                    activeDirectoryUser.ADStatus = "INACTIVE";
                    inactiveADusers.Add(activeDirectoryUser);
                }
            }

            conn.Dispose();

            return View(inactiveADusers);
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
