using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace UniversalProviders_Identity_OWIN.Account
{
    public partial class RoleManagement : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void AddRole_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(RoleName.Text))
            {
                Roles.CreateRole(RoleName.Text);
            }
        }

        protected void AddUserRole_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(Username.Text) && !String.IsNullOrEmpty(AddRoleName.Text))
            {
                Roles.AddUserToRole(Username.Text, AddRoleName.Text);
            }
        }
    }
}