using System;
using System.Web;
using Microsoft.AspNet.Identity;

namespace UniversalProviders_Identity_OWIN.IdentityAcccount
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RegisterHyperLink.NavigateUrl = "Register";
            OpenAuthLogin.ReturnUrl = Request.QueryString["ReturnUrl"];
            var returnUrl = HttpUtility.UrlEncode(Request.QueryString["ReturnUrl"]);
            if (!String.IsNullOrEmpty(returnUrl))
            {
                RegisterHyperLink.NavigateUrl += "?ReturnUrl=" + returnUrl;
            }
        }

        protected void UserLogin_Click(object sender, EventArgs e)
        {
            if (IsValid)
            {
                // Validate the user password
                var manager = new UserManager();

                // Find user from database and copy password salt and format
                var user = manager.FindByName(Username.Text); 

                if (user == null)
                {
                    ModelState.AddModelError("", "User not found");
                }

                var passwordHasher = manager.PasswordHasher as UserPasswordHasher;
                passwordHasher.PasswordFormat = user.PasswordLogin.PasswordFormat;
                passwordHasher.PasswordSalt = user.PasswordLogin.PasswordSalt;

                user = manager.Find(Username.Text, Password.Text);

                if (user != null)
                {
                    IdentityHelper.SignIn(manager, user, RememberMe.Checked);
                    IdentityHelper.RedirectToReturnUrl(Request.QueryString["ReturnUrl"], Response);
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password");
                }
            }
        }
    }
}