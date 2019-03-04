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
            using (FileStream fs = new FileStream(workingDirectory + "\\678301_uhky0lbr_config.json", FileMode.Open))
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
                newUser.BoxID = user.Id;
                newUser.BoxLogin = user.Login;
                newUser.BoxName = user.Name;
                newUser.BoxDateModified = user.ModifiedAt.Value;
                newUser.BoxDateCreated = user.CreatedAt.Value;
                newUser.BoxSpaceUsed = user.SpaceUsed.Value;
                if (_context.BoxUsers.Contains(newUser))
                    _context.BoxUsers.Update(newUser);
                else
                    _context.BoxUsers.Add(newUser);
                await _context.SaveChangesAsync();
            }
        }

        public BoxUsersController(BoxApplicationContext context)
        {
            _context = context;
            boxclient = BoxConnection();
        }

        public async Task<List<BoxUsers>> GetInactiveUsers()
        {
            List<BoxUsers> users = await _context.BoxUsers.ToListAsync();
            List<BoxUsers> inactiveboxusers = users.Where(item => inactiveusers.Contains(item.BoxLogin)).ToList();
            return (inactiveboxusers);
        }

        public async Task LogRemoval(string userid)
        {
            ApplicationAction act1 = new ApplicationAction();
            act1.ApplicationActionADUser = new ActiveDirectoryUser();
            act1.ApplicationActionADForeignKey = userid;
            act1.ApplicationActionType = "Remove User";
            //act1.ActionObjectModified = "";
            act1.ApplicationActionDate = DateTime.Now;
            _context.Add(act1);
            await _context.SaveChangesAsync();
        }


        // GET: BoxUsers
        public async Task<IActionResult> Index()
        {
            await UpdateDB();
            //_context.Database.ExecuteSqlCommand("TRUNCATE TABLE [BoxUsers]");
            return View(await GetInactiveUsers());
        }

        public async Task<IActionResult> RemoveInactiveAccounts()
        {
            BoxUser currentUser = await boxclient.UsersManager.GetCurrentUserInformationAsync();
            List<BoxUsers> inactiveboxusers = await GetInactiveUsers();
            foreach (BoxUsers user in inactiveboxusers)
            {
                BoxFolder movedFolder = await boxclient.UsersManager.MoveUserFolderAsync(user.BoxID, currentUser.Id);
                await boxclient.UsersManager.DeleteEnterpriseUserAsync(user.BoxID, false, true);
                LogRemoval(user.BoxLogin);
                _context.BoxUsers.Remove(user);
            }
            await _context.SaveChangesAsync();
            return View("Index");
        }
    }
}
