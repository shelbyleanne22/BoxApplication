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
    public class CreateAccountsController : BaseController
    {
        private readonly BoxApplicationContext _context;
        private Box.V2.BoxClient _boxclient;

        public CreateAccountsController(BoxApplicationContext context)
        {
            _context = context;
            _boxclient = BoxConnection();
        }

        public List<ActiveDirectoryUser> GetUsersWithoutBox()
        {
            var usersWithoutBox = new List<ActiveDirectoryUser>();
            foreach(ActiveDirectoryUser adUser in _context.ActiveDirectoryUsers)
            {
                if (adUser.ADStatus == "Active")
                {
                    bool hasBoxAccount = false;
                    foreach (BoxUsers boxUser in _context.BoxUsers)
                    {
                        if (adUser.ADGUID == boxUser.ADGUID)
                        {
                            hasBoxAccount = true;
                        }
                    }
                    if (!hasBoxAccount)
                    {
                        usersWithoutBox.Add(adUser);
                    }
                }
            }
            return usersWithoutBox;
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

        public async Task<IActionResult> Index()
        {
            await UpdateADTable(_context);
            await UpdateBoxTable(_context);
            return View(GetUsersWithoutBox());
        }

        public async Task<IActionResult> CreateAccounts()
        {
            List<ActiveDirectoryUser> usersWithoutBox = GetUsersWithoutBox();
            foreach(ActiveDirectoryUser adUser in usersWithoutBox)
            {
                string id = System.Text.Encoding.UTF8.GetString(adUser.ADGUID);
                BoxUser newUser = await _boxclient.UsersManager.GetUserInformationAsync(userId: id);
                await LogAction(id, "Created Box account for " + adUser.ADUsername);
            }

            return View("Index");

            //BoxUser currentUser = await _boxclient.UsersManager.GetCurrentUserInformationAsync();
            //List<BoxUsers> inactiveboxusers = await GetInactiveUsers();
            //foreach (BoxUsers user in inactiveboxusers)
            //{
            //    //Move root folder to service account and log
            //    BoxFolder movedFolder = await _boxclient.UsersManager.MoveUserFolderAsync(user.ID, currentUser.Id);
            //    await LogAction(user.Login, "Transfer to Service Acount");
            //
            //    //Delete user from Enterprise and log
            //    await _boxclient.UsersManager.DeleteEnterpriseUserAsync(user.ID, false, true);
            //    await LogAction(user.Login, "Removed Account");
            //
            //    user.Active = false;
            //    _context.BoxUsers.Update(user);
            //}
            //await _context.SaveChangesAsync();
            //return View("Index", await GetInactiveUsers());
        }
    }
}