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
            //creates list to hold all active box users from context
            List<BoxUsers> boxUsers = await _context.BoxUsers.Where(
                x => (x.aduser.ADStatus == "Active") && (x.Active == true)).ToListAsync();
            boxUsers.Sort();

            //creates empty list to hold potential updates
            List<BoxADUpdate> potentialUpdates = new List<BoxADUpdate>();
     
            if(boxUsers.Count != 0)
            {
                foreach (var boxUser in boxUsers)
                {
                    if (boxUser.aduser.ADEmail != boxUser.Login)
                    { 
                        potentialUpdates.Add(new BoxADUpdate
                        {
                            BoxUser = boxUser,
                            ADFieldChanged = "AD Email",
                            ADNewData = boxUser.aduser.ADEmail,
                            BoxPreviousData = boxUser.Login,
                            BoxID = boxUser.ID,
                            Status = "Active"
                        });                

                    }
                    else if (boxUser.aduser.ADFullName != boxUser.Name)
                    {
                        potentialUpdates.Add(new BoxADUpdate
                        {
                            BoxUser = boxUser,
                            ADFieldChanged = "AD First Name",
                            ADNewData = boxUser.aduser.ADFullName,
                            BoxPreviousData = boxUser.Name,
                            BoxID = boxUser.ID,
                            Status = "Active"
                        });
                    }
                   
                }

                if (potentialUpdates.Count != 0)
                {
                    foreach (BoxADUpdate potentialUpdate in potentialUpdates)
                    {

                        var result = _context.BoxADUpdates.SingleOrDefault(x => x.BoxID == potentialUpdate.BoxID && x.Status == "Active");
                        if (result != null) 
                        {
                            //update record to match potential update or add additional update
                            result.ADFieldChanged = potentialUpdate.ADFieldChanged;
                            result.ADNewData = potentialUpdate.ADNewData;
                            result.BoxPreviousData = potentialUpdate.BoxPreviousData;
                            result.Status = "Active";
                        }
                        else
                        {
                            //add to context if it does not already exist
                            _context.BoxADUpdates.Add(potentialUpdate);
                        }
                      
                    }
                }
            }
            
            await _context.SaveChangesAsync();
            
            return View(await _context.BoxADUpdates.Where(x => x.Status == "Active").ToListAsync());
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

            foreach (BoxADUpdate boxUpdate in _context.BoxADUpdates.ToList().Where(x => x.Status == "Active"))
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

                    //updates box users context
                    BoxUsers result = new BoxUsers();
                    result = _context.BoxUsers.SingleOrDefault(x => x.ID == updatedUser.Id);
                    result.Login = updatedUser.Login;

                }
                else if (boxUpdate.ADFieldChanged == "AD Full Name")
                {
                    var updates = new BoxUserRequest()
                    {
                        Id = userNeedsUpdates.Id,
                        Name = boxUpdate.ADNewData
                    };
                    BoxUser updatedUser = await boxclient.UsersManager.UpdateUserInformationAsync(updates);

                    //updates box users context
                    BoxUsers result = new BoxUsers();
                    result = _context.BoxUsers.SingleOrDefault(x => x.ID == updatedUser.Id);
                    result.Name = updatedUser.Name;
                }

                //set status to inactive
                boxUpdate.Status = "Inactive";

                //log change
                await LogAction(userNeedsUpdates.Id, "Updated Account");                          
            }

            
            await _context.SaveChangesAsync();
            return View("~/Views/Home/Index.cshtml");
        }

        public async Task LogAction(string userid, string type)
        {
            ApplicationAction act1 = new ApplicationAction
            {
                User = userid,
                Type = type,
                Date = DateTime.Now
            };
            _context.Add(act1);
            await _context.SaveChangesAsync();
        }

    }
}
