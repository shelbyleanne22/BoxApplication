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
    public class BaseController : Controller
    {
        public Box.V2.BoxClient BoxConnection()
        {
            // Read in config file
            string filepath = Environment.CurrentDirectory + "\\" + Startup.MyAppData.Configuration["BoxJWTAuth"];
            IBoxConfig config = null;
            

            using (FileStream fs = new FileStream(filepath, FileMode.Open))
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

        public async Task UpdateBoxTable(BoxApplicationContext _context)
        {
            Box.V2.BoxClient boxclient;
            try
            {
               boxclient = BoxConnection();
            }
            catch
            {
                return;
            }
            BoxCollection<BoxUser> users = await boxclient.UsersManager.GetEnterpriseUsersAsync();
            foreach (BoxUser user in users.Entries)
            {
                BoxUsers newUser = new BoxUsers
                {
                    ID = user.Id,
                    Login = user.Login,
                    Name = user.Name,
                    DateModified = user.ModifiedAt.Value,
                    DateCreated = user.CreatedAt.Value,
                    SpaceUsed = user.SpaceUsed.Value,
                    Active = true
                };

                //Get ADGUID of new box users
                List<ActiveDirectoryUser> associatedaccts = _context.ActiveDirectoryUsers.Where(x => x.ADEmail.ToLower() == user.Login.ToLower()).ToList();
                if (associatedaccts.Count == 1)
                {
                    newUser.aduser = associatedaccts[0];
                    newUser.ADGUID = associatedaccts[0].ADGUID;
                }
                else
                    continue;

                if (!_context.BoxUsers.Any(x => x.ID == newUser.ID))
                    _context.BoxUsers.Add(newUser);
                else
                {
                    var result = _context.BoxUsers.SingleOrDefault(x => x.ID == newUser.ID);
                    if(result != null)
                    {
                        result.DateCreated = newUser.DateCreated;
                        result.DateModified = newUser.DateModified;
                        result.Login = newUser.Login;
                        result.Name = newUser.Name;
                        result.SpaceUsed = newUser.SpaceUsed;
                    }
                }
            }

            //Remove accounts from database that have been deleted from admin console, might be able to remove this after deployment
            foreach(BoxUsers boxUser in _context.BoxUsers)
            {
                if (boxUser.Active == true && !users.Entries.Any(x => x.Id == boxUser.ID))
                    _context.Remove(boxUser);
            }

            _context.SaveChanges();

        }


        public async Task UpdateADTable(BoxApplicationContext _context)
        {
            List<ActiveDirectoryUser> inactiveADusers = new List<ActiveDirectoryUser>();

            SearchResultCollection resultCol;
            string DomainPath = Startup.MyAppData.Configuration["ADAuth:DomainPath"];
            //CN = sccs,CN = students, /DC=mcghi,DC=mcg,DC=edu/
            string username = Startup.MyAppData.Configuration["ADAuth:Username"];
            string password = Startup.MyAppData.Configuration["ADAuth:Password"];



            //creates directoryentry object that binds the instance to the domain path
            DirectoryEntry searchRoot = new DirectoryEntry(DomainPath, username, password, AuthenticationTypes.Secure);
            //creates a directorysearcher object which searches for all users in the domain
            DirectorySearcher searchactive = new DirectorySearcher(searchRoot);
            DirectorySearcher searchinactive = new DirectorySearcher(searchRoot);

            //filters the search to only active/enabled accounts
            searchactive.Filter = "(&(objectCategory=person)(objectClass=user)(!userAccountControl:1.2.840.113556.1.4.803:=2))";
            searchactive.PropertiesToLoad.Add("objectguid");
            searchactive.PropertiesToLoad.Add("mail");
            searchactive.PropertiesToLoad.Add("displayName");
            searchactive.PropertiesToLoad.Add("whenchanged");

            //filters the search to only inactive/disabled accounts
            searchinactive.Filter = "(&(objectCategory=person)(objectClass=user)(userAccountControl:1.2.840.113556.1.4.803:=2))";
            searchinactive.PropertiesToLoad.Add("objectguid");
            searchinactive.PropertiesToLoad.Add("mail");
            searchinactive.PropertiesToLoad.Add("displayName");
            searchinactive.PropertiesToLoad.Add("whenchanged");

            SearchResult result;

            try
            {
                resultCol = searchactive.FindAll();
            }
            catch
            {
                return;
            }

            for (int counter = 0; counter < resultCol.Count; counter++)
            {
                string UserNameEmailString = string.Empty;
                result = resultCol[counter];
                if (result.Properties.Contains("mail") &&
                    result.Properties.Contains("displayname"))
                {
                    ActiveDirectoryUser activeDirectoryUser = new ActiveDirectoryUser();
                    activeDirectoryUser.ADGUID = (Byte[])result.Properties["objectguid"][0];
                    activeDirectoryUser.ADEmail = (String)result.Properties["mail"][0];
                    activeDirectoryUser.ADEmail = activeDirectoryUser.ADEmail.ToLower();
                    activeDirectoryUser.ADFullName = (String)result.Properties["displayName"][0];
                    activeDirectoryUser.ADDateModified = (DateTime)result.Properties["whenchanged"][0];
                    activeDirectoryUser.ADStatus = "Active";

                    var tempuser = _context.ActiveDirectoryUsers.Find(activeDirectoryUser.ADGUID);
                    if (tempuser != null)
                    {
                        if (!tempuser.Equals(activeDirectoryUser))
                        {
                            _context.Entry(tempuser).CurrentValues.SetValues(activeDirectoryUser);
                        }
                    }
                    else
                        _context.ActiveDirectoryUsers.Add(activeDirectoryUser);
                    await _context.SaveChangesAsync();
                }
            }

            try
            {
                resultCol = searchinactive.FindAll();
            }
            catch
            {
                return;
            }
            for (int counter = 0; counter < resultCol.Count; counter++)
            {
                string UserNameEmailString = string.Empty;
                result = resultCol[counter];
                if (result.Properties.Contains("mail") &&
                    result.Properties.Contains("displayName"))
                {
                    ActiveDirectoryUser activeDirectoryUser = new ActiveDirectoryUser();
                    activeDirectoryUser.ADGUID = (Byte[])result.Properties["objectguid"][0];
                    activeDirectoryUser.ADEmail = (String)result.Properties["mail"][0];
                    activeDirectoryUser.ADEmail = activeDirectoryUser.ADEmail.ToLower();
                    activeDirectoryUser.ADFullName = (String)result.Properties["displayName"][0];
                    activeDirectoryUser.ADDateModified = (DateTime)result.Properties["whenchanged"][0];
                    activeDirectoryUser.ADStatus = "Inactive";

                    var tempuser = _context.ActiveDirectoryUsers.Find(activeDirectoryUser.ADGUID);
                    if (tempuser != null) {
                        if (!tempuser.Equals(activeDirectoryUser))
                        {
                            _context.Entry(tempuser).CurrentValues.SetValues(activeDirectoryUser);
                        }
                    }
                    else
                        _context.ActiveDirectoryUsers.Add(activeDirectoryUser);
                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}
