using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.DirectoryServices;
using System.Net;
using System.DirectoryServices.Protocols;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BoxApplication.Models;
using Box.V2.JWTAuth;
using Box.V2.Config;
using Box.V2.Models;

namespace BoxApplication.Controllers
{
    public class RemoveAccountsController : Controller
    {
        private readonly BoxApplicationContext _context;
        private Box.V2.BoxClient boxclient;
        private readonly string workingDirectory = Environment.CurrentDirectory;

        //Stub for when we can test active directory calls
        private List<string> inactiveusersstub = new List<string> { "testuser1@domain.edu", "testuser2@domain.edu", "ajarnagin1992@gmail.com" };

        // GET: ActiveDirectoryUsers
        public List<string> GetInactiveAD()
        {
            List<string> inactiveADusers = new List<string>();

            string DomainPath = "LDAP://hi-root03.mcghi.mcg.edu";
            //CN = sccs,CN = students, /DC=mcghi,DC=mcg,DC=edu/
            string username = "";
            string password = "";



            //creates directoryentry object that binds the instance to the domain path
            DirectoryEntry searchRoot = new DirectoryEntry(DomainPath, username, password, AuthenticationTypes.Secure);
            //creates a directorysearcher object which searches for all users in the domain
            DirectorySearcher search = new DirectorySearcher(searchRoot);
            //filters the search to only inactive/disabled accounts
            search.Filter = "(&(objectCategory=person)(objectClass=user)(userAccountControl:1.2.840.113556.1.4.803:=2))";
            search.PropertiesToLoad.Add("samaccountname");
            search.PropertiesToLoad.Add("mail");
            search.PropertiesToLoad.Add("displayname");
            search.PropertiesToLoad.Add("whenchanged");

            SearchResult result;
            SearchResultCollection resultCol = search.FindAll();


            //displays and redirects if no inactive accounts are found
            if (resultCol.Count.Equals(0))
            {
                //for production
                ModelState.AddModelError("Error", "No inactive accounts exist.");
            }
            else
            {
                for (int counter = 0; counter < resultCol.Count; counter++)
                {
                    string UserNameEmailString = string.Empty;
                    result = resultCol[counter];
                    if (result.Properties.Contains("samaccountname") &&
                             result.Properties.Contains("mail") &&
                        result.Properties.Contains("displayname"))
                    {
                        inactiveADusers.Add((String)result.Properties["mail"][0]);
                    }
                }

            }

            return inactiveADusers;
        }

        public Box.V2.BoxClient BoxConnection()
        {
            // Read in config file
            IBoxConfig config = null;
            using (FileStream fs = new FileStream(workingDirectory + "\\678301_ygv0a2m9_config.json", FileMode.Open))
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

        public async Task UpdateDB()
        {
            BoxCollection<BoxUser> users = await boxclient.UsersManager.GetEnterpriseUsersAsync();
            foreach(BoxUser user in users.Entries)
            {
                BoxUsers newUser = new BoxUsers();
                newUser.ID = user.Id;
                newUser.Login = user.Login;
                newUser.Name = user.Name;
                newUser.DateModified = user.ModifiedAt.Value;
                newUser.DateCreated = user.CreatedAt.Value;
                newUser.SpaceUsed = user.SpaceUsed.Value;
                if (_context.BoxUsers.Any(x => x == newUser))
                    _context.BoxUsers.Update(newUser);
                else
                    _context.BoxUsers.Add(newUser);
                await _context.SaveChangesAsync();
            }
        }

        public RemoveAccountsController(BoxApplicationContext context)
        {
            _context = context;
            boxclient = BoxConnection();
        }

        public async Task<List<BoxUsers>> GetInactiveUsers()
        {
            List<BoxUsers> users = await _context.BoxUsers.ToListAsync();
            List<BoxUsers> inactiveboxusers = users.Where(item => inactiveusersstub.Contains(item.Login)).ToList();
            return (inactiveboxusers);
        }

        public async Task LogRemoval(string userid, string type)
        {
            ApplicationAction act1 = new ApplicationAction();
            act1.User = userid;
            act1.Type = type;
            act1.Date = DateTime.Now;
            _context.Add(act1);
            await _context.SaveChangesAsync();
        }

        // GET: BoxUsers
        public async Task<IActionResult> Index()
        {
            await UpdateDB();
            return View(await GetInactiveUsers());
        }

        public async Task<IActionResult> RemoveInactiveAccounts()
        {
            BoxUser currentUser = await boxclient.UsersManager.GetCurrentUserInformationAsync();
            List<BoxUsers> inactiveboxusers = await GetInactiveUsers();
            foreach (BoxUsers user in inactiveboxusers)
            {
                //Move root folder to service account and log
                BoxFolder movedFolder = await boxclient.UsersManager.MoveUserFolderAsync(user.ID, currentUser.Id);
                await LogRemoval(user.Login, "Transfer to Service Acount");

                //Delete user from Enterprise and log
                await boxclient.UsersManager.DeleteEnterpriseUserAsync(user.ID, false, true);
                await LogRemoval(user.Login, "Removed Account");
                _context.BoxUsers.Remove(user);
            }
            await _context.SaveChangesAsync();
            return View("Index", await GetInactiveUsers());
        }
    }
}
