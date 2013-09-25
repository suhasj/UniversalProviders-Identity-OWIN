using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace UniversalProviders_Identity_OWIN.IdentityAccount
{
    public partial class CreateRole : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void AddRole_Click(object sender, EventArgs e)
        {
            var currentApplicationId = new CustomUserDBContext().Applications.SingleOrDefault(x => x.ApplicationName == "/").ApplicationId;

            var manager = new RoleManager<Role>(new UniversalProviderRoleStore(new CustomUserDBContext()));

            if (!manager.RoleExists(RoleName.Text))
            {
                var result = manager.Create(new Role() { ApplicationId = currentApplicationId, Name = RoleName.Text });
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                }
            }
            else
            {
                ModelState.AddModelError("", "Role Exists");
            }
        }
    }
}