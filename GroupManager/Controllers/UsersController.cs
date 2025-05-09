using GroupManager.Models;
using GroupManager.Utils;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GroupManager.Controllers
{
    public class UsersController : Controller
    {
        // For simplicity, this sample uses an in-memory data store instead of a db.
        private ConcurrentDictionary<string, List<GroupManager.Models.User>> userList = new ConcurrentDictionary<string, List<GroupManager.Models.User>>();

        [Authorize]
        // GET: Users
        public async Task<ActionResult> Index()
        {
            string tenantId = ClaimsPrincipal.Current.FindFirst(Globals.TenantIdClaimType).Value;

            try
            {
                // Get the Users using Graph API
                var graphUsers = await GraphHelper.GetUsersAsync();

                // Convert from Graph API response to our model
                List<GroupManager.Models.User> users = new List<GroupManager.Models.User>();
                foreach (var graphUser in graphUsers)
                {
                    users.Add(new GroupManager.Models.User
                    {
                        displayName = graphUser.DisplayName,
                        jobTitle = graphUser.JobTitle,
                        mail = graphUser.Mail,
                        id = graphUser.Id,
                    });
                }

                userList[tenantId] = users;
            }
            catch (MsalUiRequiredException ex)
            {
                /*
                    If the tokens have expired or become invalid for any reason, ask the user to sign in again.
                    Another cause of this exception is when you restart the app using InMemory cache.
                    It will get wiped out while the user will be authenticated still because of their cookies, requiring the TokenCache to be initialized again
                    through the sign in flow.
                */
                return new RedirectResult("/Account/SignIn");
            }
            // Handle unexpected errors.
            catch (Exception ex)
            {
                return new RedirectResult("/Error?message=" + ex.Message);
            }

            ViewBag.TenantId = tenantId;
            return View(userList[tenantId]);
        }
    }
}