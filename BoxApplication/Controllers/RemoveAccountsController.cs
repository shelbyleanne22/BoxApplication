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
    public class RemoveAccountsController : BaseController
    {
        private readonly BoxApplicationContext _context;
        private Box.V2.BoxClient _boxclient;

        public RemoveAccountsController(BoxApplicationContext context)
        {
            _context = context;
            _boxclient = BoxConnection();
        }

        public async Task<List<BoxUsers>> GetInactiveUsers()
        {
            List<BoxUsers> inactiveadusers = await _context.BoxUsers.Where(
                x => (x.aduser.ADStatus == "Inactive") && (x.Active == true)).ToListAsync();
            return (inactiveadusers);
        }

        public async Task LogAction(string userid, string type)
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
            await UpdateADTable(_context);
            await UpdateBoxTable(_context);
            return View(await GetInactiveUsers());
        }

        public async Task<IActionResult> RemoveInactiveAccounts()
        {
            BoxUser currentUser = await _boxclient.UsersManager.GetCurrentUserInformationAsync();
            List<BoxUsers> inactiveboxusers = await GetInactiveUsers();
            foreach (BoxUsers user in inactiveboxusers)
            {
                //Move root folder to service account and log
                BoxFolder movedFolder = await _boxclient.UsersManager.MoveUserFolderAsync(user.ID, currentUser.Id);
                await LogAction(user.Login, "Transfer to Service Acount");

                //Delete user from Enterprise and log
                await _boxclient.UsersManager.DeleteEnterpriseUserAsync(user.ID, false, true);
                await LogAction(user.Login, "Removed Account");

                user.Active = false;
                _context.BoxUsers.Update(user);
            }
            await _context.SaveChangesAsync();
            return View("Index", await GetInactiveUsers());
        }
    }
}
