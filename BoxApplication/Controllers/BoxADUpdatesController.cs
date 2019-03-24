using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BoxApplication.Models;
using Box.V2.Models;

namespace BoxApplication.Controllers
{
    public class BoxADUpdatesController : BaseController
    {
        private readonly BoxApplicationContext _context;

        public BoxADUpdatesController(BoxApplicationContext context)
        {
            _context = context;
        }


        // GET: BoxADUpdates
        public async Task<IActionResult> Index()
        {
            //await UpdateADTable(_context);
            await UpdateBoxTable(_context);
            //creats list to hold all box users from context
            List<BoxUsers> boxUsers = _context.BoxUsers.ToList();
            boxUsers.Sort();

            //creates empty list to hold potential updates
            List<BoxADUpdate> potentialUpdates = new List<BoxADUpdate>();

            foreach (var boxUser in boxUsers)
            {
                if (boxUser.aduser.ADEmail != boxUser.Login)
                {
                    BoxADUpdate potentialUpdate = new BoxADUpdate();
                    potentialUpdate.BoxUser = boxUser;
                    potentialUpdate.ADFieldChanged = "AD Email";
                    potentialUpdate.ADNewData = boxUser.aduser.ADEmail;
                    potentialUpdate.BoxPreviousData = boxUser.Login;
                    potentialUpdate.BoxID = boxUser.ID;

                    potentialUpdates.Add(potentialUpdate);
                }
                else if (boxUser.aduser.ADFirstName != boxUser.Name)
                {
                    BoxADUpdate potentialUpdate = new BoxADUpdate();
                    potentialUpdate.BoxUser = boxUser;
                    potentialUpdate.ADFieldChanged = "AD First Name";
                    potentialUpdate.ADNewData = boxUser.aduser.ADFirstName;
                    potentialUpdate.BoxPreviousData = boxUser.Name;
                    potentialUpdate.BoxID = boxUser.ID;
                    potentialUpdates.Add(potentialUpdate);
                }
                else
                {
                    //display error
                }
            }

            if(potentialUpdates.Count != 0)
            {
                foreach (BoxADUpdate potentialUpdate in potentialUpdates)
                {
                    //if (_context.BoxADUpdates.Any(x => x != potentialUpdate))
                    //{
                    //add to context if it does not already exist
                    _context.BoxADUpdates.Add(potentialUpdate);
                    //}
                }
            }            

            await _context.SaveChangesAsync();

            return View(await _context.BoxADUpdates.ToListAsync());
        }
    
        // GET: BoxADUpdates/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boxADUpdate = await _context.BoxADUpdates
                .FirstOrDefaultAsync(m => m.BoxADUpdateID == id);
            if (boxADUpdate == null)
            {
                return NotFound();
            }

            return View(boxADUpdate);
        }

        // GET: BoxADUpdates/Create
        public IActionResult Create()
        {
            return View();
        }


        public async Task<IActionResult> UpdateAccounts()
        {
            Box.V2.BoxClient boxclient = BoxConnection();
            BoxCollection<BoxUser> users = await boxclient.UsersManager.GetEnterpriseUsersAsync();
            List<BoxUser> boxUsers = users.Entries;    
            
            foreach (BoxADUpdate boxUpdate in _context.BoxADUpdates.ToList())
            {
                BoxUser userNeedsUpdates = boxUsers.Where(x => x.Id == boxUpdate.BoxID).FirstOrDefault();

                if (boxUpdate.ADFieldChanged == "AD Email")
                {
                    var updates = new BoxUserRequest()
                    {
                        Id = userNeedsUpdates.Id,
                        Login = boxUpdate.ADNewData
                    };
                    BoxUser updatedUser = await boxclient.UsersManager.UpdateUserInformationAsync(updates);
                }
                else if (boxUpdate.ADFieldChanged == "AD First Name")
                {
                    var updates = new BoxUserRequest()
                    {
                        Id = userNeedsUpdates.Id,
                        Name = boxUpdate.ADNewData
                    };
                    BoxUser updatedUser = await boxclient.UsersManager.UpdateUserInformationAsync(updates);
                }
                //log change
                await LogAction(userNeedsUpdates.Id, "Updated " + boxUpdate.ADFieldChanged + " from " + boxUpdate.BoxPreviousData + " to " + boxUpdate.ADNewData);
            
            }

            await UpdateBoxTable(_context);

            return View();

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
    }
}
