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
        public async Task<IActionResult> Index ()
        {
            return View(await _context.BoxADUpdates.OrderBy(x => x.Status).ToListAsync());
        }

        public async Task<IActionResult> PotentialUpdates()
        {
            await UpdateADTable(_context);
            await UpdateBoxTable(_context);

            //find accounts that potential need updating and saves in database
            FindPotentialUpdates();

            return View(await _context.BoxADUpdates.Where(x => x.Status == "Active").ToListAsync());
        }   


        //confirm updates button
        public async Task<IActionResult> UpdateAccounts()
        {
            //establishes box connection
            Box.V2.BoxClient boxclient = BoxConnection();
            BoxCollection<BoxUser> users = await boxclient.UsersManager.GetEnterpriseUsersAsync();
            List<BoxUser> boxUsers = users.Entries;
            List<BoxADUpdate> failedUpdates = new List<BoxADUpdate>();
            
            //for each boxupdate in the context
            foreach (BoxADUpdate boxUpdate in _context.BoxADUpdates.ToList())
            {
                if (boxUpdate.Status == "Active")
                {
                    BoxUser userNeedsUpdates = boxUsers.Where(x => x.Id == boxUpdate.BoxID).FirstOrDefault();

                    if (boxUpdate.ADFieldChanged == "AD Email")
                    {
                        var updates = new BoxUserRequest()
                        {
                            Id = userNeedsUpdates.Id,
                            Login = boxUpdate.ADNewData
                        };
                        try
                        {
                            BoxUser updatedUser = await boxclient.UsersManager.UpdateUserInformationAsync(updates);
                        }
                        catch
                        {
                            failedUpdates.Add(boxUpdate);
                            continue;
                        }
                    }
                    else if (boxUpdate.ADFieldChanged == "AD Full Name")
                    {
                        var updates = new BoxUserRequest()
                        {
                            Id = userNeedsUpdates.Id,
                            Name = boxUpdate.ADNewData
                        };
                        try
                        {
                            BoxUser updatedUser = await boxclient.UsersManager.UpdateUserInformationAsync(updates);
                        }
                        catch
                        {
                            failedUpdates.Add(boxUpdate);
                            continue;
                        }
                    }
                    boxUpdate.Status = "Inactive";
                    //log change
                    await LogAction(userNeedsUpdates.Login, "Updated Box Account");
                }
            }

            _context.SaveChanges();
            await UpdateBoxTable(_context);

            return View("../Home/Index");

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

        public void FindPotentialUpdates()
        {
            //creates list to hold all box users from context
            List<BoxUsers> boxUsers = _context.BoxUsers.ToList();
            //creates empty list to hold potential updates
            List<BoxADUpdate> potentialUpdates = new List<BoxADUpdate>();

            foreach (var boxUser in boxUsers)
            {
                if (boxUser.aduser != null && boxUser.aduser.ADEmail != boxUser.Login && boxUser.aduser.ADStatus == "Active")
                {
                    BoxADUpdate potentialUpdate = new BoxADUpdate
                    {
                        BoxUser = boxUser,
                        ADFieldChanged = "AD Email",
                        ADNewData = boxUser.aduser.ADEmail,
                        BoxPreviousData = boxUser.Login,
                        BoxID = boxUser.ID,
                        Status = "Active"
                    };
                    potentialUpdates.Add(potentialUpdate);
                }
                else if (boxUser.aduser != null && boxUser.aduser.ADFullName != boxUser.Name && boxUser.aduser.ADStatus == "Active")
                {
                    BoxADUpdate potentialUpdate = new BoxADUpdate
                    {
                        BoxUser = boxUser,
                        ADFieldChanged = "AD First Name",
                        ADNewData = boxUser.aduser.ADFullName,
                        BoxPreviousData = boxUser.Name,
                        BoxID = boxUser.ID,
                        Status = "Active"
                    };
                    potentialUpdates.Add(potentialUpdate);
                } 
                //else do nothing

            }

            if (potentialUpdates.Count != 0)
            {
                foreach (BoxADUpdate potentialUpdate in potentialUpdates)
                {
                    //if (_context.BoxADUpdates.Any(x => x != potentialUpdate))
                    if (_context.BoxADUpdates.Any(x => x.BoxID == potentialUpdate.BoxID))
                    {
                        BoxADUpdate tempBoxADUpdate = _context.BoxADUpdates.FirstOrDefault(x => x.BoxID == potentialUpdate.BoxID);
                        string updatedField = tempBoxADUpdate.ADFieldChanged;
                        if (updatedField != potentialUpdate.ADFieldChanged)
                        {
                            //add to context if it does not already exist
                            _context.BoxADUpdates.Add(potentialUpdate);
                        }
                    }
                    else
                    {
                        //add to context if it does not already exist
                        _context.BoxADUpdates.Add(potentialUpdate);
                    }
                }
            }

            _context.SaveChanges();
        }
    }
}
