using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace UniversalProviders_Identity_OWIN.IdentityAccount
{
    public partial class UserRoleManagement : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void AddUserRole_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(Username.Text) && !String.IsNullOrEmpty(AddRoleName.Text))
            {
                var manager = new UserManager();
                var user = manager.FindByName(Username.Text);

                if (user == null)
                {
                    ModelState.AddModelError("", "User not found");
                    return;
                }

                var result = manager.AddToRole(user.Id, AddRoleName.Text);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                }
            }
        }
    }
}