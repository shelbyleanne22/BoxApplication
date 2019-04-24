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
    public class ActiveDirectoryUsersController : BaseController
    {
        private readonly BoxApplicationContext _context;

        public ActiveDirectoryUsersController(BoxApplicationContext context)
        {
            _context = context;
        }

        // GET: ActiveDirectoryUsers
        public async Task<IActionResult> Index()
        {
            await UpdateADTable(_context);
            return View(await _context.ActiveDirectoryUsers.ToListAsync()); 
        }
    }
}
