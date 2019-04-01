using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BoxApplication.Models;

namespace BoxApplication.Controllers
{
    public class BoxUsersController : Controller
    {
        private readonly BoxApplicationContext _context;

        public BoxUsersController(BoxApplicationContext context)
        {
            _context = context;
        }

        // GET: BoxUsers
        public async Task<IActionResult> Index()
        {
            var boxApplicationContext = _context.BoxUsers.Include(b => b.aduser);
            return View(await boxApplicationContext.ToListAsync());
        }
    }
}
