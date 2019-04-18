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
                var userParams = new BoxUserRequest()
                {
                    Name = adUser.ADFullName,
                    Login = adUser.ADEmail
                };
                string id = adUser.ADEmail;
                BoxUser newUser = await _boxclient.UsersManager.CreateEnterpriseUserAsync(userParams);
                await LogAction(id, "Created Box Account");
                _context.ActiveDirectoryUsers.Update(adUser);
            }

            return View("Index");
        }
    }
}