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
            IBoxConfig config = null;
            using (FileStream fs = new FileStream(Environment.CurrentDirectory + "\\678301_s116imjm_config.json", FileMode.Open))
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
            Box.V2.BoxClient boxclient = BoxConnection();
            BoxCollection<BoxUser> users = await boxclient.UsersManager.GetEnterpriseUsersAsync();
            foreach (BoxUser user in users.Entries)
            {
                BoxUsers newUser = new BoxUsers();
                newUser.ID = user.Id;
                newUser.Login = user.Login;
                newUser.Name = user.Name;
                newUser.DateModified = user.ModifiedAt.Value;
                newUser.DateCreated = user.CreatedAt.Value;
                newUser.SpaceUsed = user.SpaceUsed.Value;
                newUser.Active = true;

                //Get ADGUID of new box users
                List<ActiveDirectoryUser> associatedaccts = _context.ActiveDirectoryUsers.Where(x => x.ADEmail == user.Login).ToList();
                if (associatedaccts.Count == 1)
                {
                    newUser.ADGUID = associatedaccts[0].ADGUID;
                }
                else
                    continue;

                if (!_context.BoxUsers.Any(x => x.ID == newUser.ID))
                    _context.BoxUsers.Add(newUser);
                _context.SaveChanges();
            }
        }
        public async Task UpdateADTable(BoxApplicationContext _context)
        {
            List<ActiveDirectoryUser> inactiveADusers = new List<ActiveDirectoryUser>();

            string DomainPath = "LDAP://hi-root03.mcghi.mcg.edu";
            //CN = sccs,CN = students, /DC=mcghi,DC=mcg,DC=edu/
            string username = "";
            string password = "";



            //creates directoryentry object that binds the instance to the domain path
            DirectoryEntry searchRoot = new DirectoryEntry(DomainPath, username, password, AuthenticationTypes.Secure);
            //creates a directorysearcher object which searches for all users in the domain
            DirectorySearcher searchactive = new DirectorySearcher(searchRoot);
            DirectorySearcher searchinactive = new DirectorySearcher(searchRoot);

            //filters the search to only active/enabled accounts
            searchactive.Filter = "(&(objectCategory=person)(objectClass=user)(!userAccountControl:1.2.840.113556.1.4.803:=2))";
            searchactive.PropertiesToLoad.Add("objectguid");
            searchactive.PropertiesToLoad.Add("samaccountname");
            searchactive.PropertiesToLoad.Add("mail");
            searchactive.PropertiesToLoad.Add("displayname");
            searchactive.PropertiesToLoad.Add("whenchanged");

            //filters the search to only inactive/disabled accounts
            searchinactive.Filter = "(&(objectCategory=person)(objectClass=user)(userAccountControl:1.2.840.113556.1.4.803:=2))";
            searchinactive.PropertiesToLoad.Add("objectguid");
            searchinactive.PropertiesToLoad.Add("samaccountname");
            searchinactive.PropertiesToLoad.Add("mail");
            searchinactive.PropertiesToLoad.Add("displayname");
            searchinactive.PropertiesToLoad.Add("whenchanged");

            SearchResult result;
            SearchResultCollection resultCol = searchactive.FindAll();

            for (int counter = 0; counter < resultCol.Count; counter++)
            {
                string UserNameEmailString = string.Empty;
                result = resultCol[counter];
                if (result.Properties.Contains("samaccountname") &&
                            result.Properties.Contains("mail") &&
                    result.Properties.Contains("displayname"))
                {
                    ActiveDirectoryUser activeDirectoryUser = new ActiveDirectoryUser();
                    activeDirectoryUser.ADGUID = (Byte[])result.Properties["objectguid"][0];
                    activeDirectoryUser.ADEmail = (String)result.Properties["mail"][0];
                    activeDirectoryUser.ADFirstName = (String)result.Properties["displayname"][0];
                    activeDirectoryUser.ADUsername = (String)result.Properties["samaccountname"][0];
                    activeDirectoryUser.ADDateModified = (DateTime)result.Properties["whenchanged"][0];
                    activeDirectoryUser.ADStatus = "Active";

                    if (_context.ActiveDirectoryUsers.Any(o => o.ADGUID == activeDirectoryUser.ADGUID))
                    {
                        if (!_context.ActiveDirectoryUsers.Contains(activeDirectoryUser))
                            _context.ActiveDirectoryUsers.Update(activeDirectoryUser);
                    }
                    else
                        _context.ActiveDirectoryUsers.Add(activeDirectoryUser);
                    await _context.SaveChangesAsync();
                }
            }

            resultCol = searchinactive.FindAll();

            for (int counter = 0; counter < resultCol.Count; counter++)
            {
                string UserNameEmailString = string.Empty;
                result = resultCol[counter];
                if (result.Properties.Contains("samaccountname") &&
                            result.Properties.Contains("mail") &&
                    result.Properties.Contains("displayname"))
                {
                    ActiveDirectoryUser activeDirectoryUser = new ActiveDirectoryUser();
                    activeDirectoryUser.ADGUID = (Byte[])result.Properties["objectguid"][0];
                    activeDirectoryUser.ADEmail = (String)result.Properties["mail"][0];
                    activeDirectoryUser.ADFirstName = (String)result.Properties["displayname"][0];
                    activeDirectoryUser.ADUsername = (String)result.Properties["samaccountname"][0];
                    activeDirectoryUser.ADDateModified = (DateTime)result.Properties["whenchanged"][0];
                    activeDirectoryUser.ADStatus = "Inactive";

                    if (_context.ActiveDirectoryUsers.Any(o => o.ADGUID == activeDirectoryUser.ADGUID))
                    {
                        if (!_context.ActiveDirectoryUsers.Contains(activeDirectoryUser))
                            _context.ActiveDirectoryUsers.Update(activeDirectoryUser);
                    }
                    else
                        _context.ActiveDirectoryUsers.Add(activeDirectoryUser);
                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}
