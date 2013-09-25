using System;
using System.Linq;
using Microsoft.AspNet.Identity;

namespace UniversalProviders_Identity_OWIN.IdentityAcccount
{
    public partial class Register : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void Submit_Click(object sender, EventArgs e)
        {
            var currentApplicationId = new CustomUserDBContext().Applications.SingleOrDefault(x => x.ApplicationName == "/").ApplicationId;

            var manager = new UserManager();
            var user = new User() { UserName = Username.Text,ApplicationId=currentApplicationId };

            user.CreatePasswordLogin();
            user.PasswordLogin.IsApproved = true;

            // Copy the PasswordSalt and Passwrod format
            var passwordHasher = manager.PasswordHasher as UserPasswordHasher;
            user.PasswordLogin.PasswordFormat = passwordHasher.PasswordFormat;
            user.PasswordLogin.PasswordSalt = passwordHasher.PasswordSalt;

            var result = manager.Create(user, Password.Text);

            if (result.Succeeded)
            {
                IdentityHelper.SignIn(manager, user, isPersistent: false);
                IdentityHelper.RedirectToReturnUrl(Request.QueryString["ReturnUrl"], Response);
            }
            else
            {
                ModelState.AddModelError("", result.Errors.FirstOrDefault());
            }
        }

    }
}