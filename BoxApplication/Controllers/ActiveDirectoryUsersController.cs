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
            await UpdateADTable();
            return View(await _context.ActiveDirectoryUsers.ToListAsync());
        }

        public async Task UpdateADTable()
        {
            List<ActiveDirectoryUser> inactiveADusers = new List<ActiveDirectoryUser>();

            string DomainPath = "LDAP://hi-root03.mcghi.mcg.edu";
            //CN = sccs,CN = students, /DC=mcghi,DC=mcg,DC=edu/
            string username = "";
            string password = "";



            //creates directoryentry object that binds the instance to the domain path
            DirectoryEntry searchRoot = new DirectoryEntry(DomainPath, username, password, AuthenticationTypes.Secure);
            //creates a directorysearcher object which searches for all users in the domain
            DirectorySearcher search = new DirectorySearcher(searchRoot);
            //filters the search to only inactive/disabled accounts
            search.Filter = "(&(objectCategory=person)(objectClass=user))";
            search.PropertiesToLoad.Add("objectguid");
            search.PropertiesToLoad.Add("samaccountname");
            search.PropertiesToLoad.Add("mail");
            search.PropertiesToLoad.Add("displayname");
            search.PropertiesToLoad.Add("whenchanged");
            search.PropertiesToLoad.Add("useraccountcontrol");

            SearchResult result;
            SearchResultCollection resultCol = search.FindAll();

            for (int counter = 0; counter < resultCol.Count; counter++)
            {
                string UserNameEmailString = string.Empty;
                result = resultCol[counter];
                if (result.Properties.Contains("samaccountname") &&
                            result.Properties.Contains("mail") &&
                    result.Properties.Contains("displayname"))
                {
                    ActiveDirectoryUser activeDirectoryUser = new ActiveDirectoryUser();
                    activeDirectoryUser.ADGUID = (String)result.Properties["objectguid"][0];
                    activeDirectoryUser.ADEmail = (String)result.Properties["mail"][0];
                    activeDirectoryUser.ADFirstName = (String)result.Properties["displayname"][0];
                    activeDirectoryUser.ADUsername = (String)result.Properties["samaccountname"][0];
                    activeDirectoryUser.ADDateModified = (DateTime)result.Properties["whenchanged"][0];
                    if ((int)result.Properties["useraccountcontrol"][0] == 2)
                        activeDirectoryUser.ADStatus = "Active";
                    else
                        activeDirectoryUser.ADStatus = "Inactive";

                    if (_context.ActiveDirectoryUsers.Any(o => o.ADGUID == activeDirectoryUser.ADGUID))
                    {
                        if (!_context.ActiveDirectoryUsers.Contains(activeDirectoryUser))
                            _context.ActiveDirectoryUsers.Update(activeDirectoryUser);
                    }
                    else
                        _context.ActiveDirectoryUsers.Add(activeDirectoryUser);
                    await _context.SaveChangesAsync();
                }
            }
        }

    }
}