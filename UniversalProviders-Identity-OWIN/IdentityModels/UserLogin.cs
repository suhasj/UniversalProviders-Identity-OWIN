using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversalProviders_Identity_OWIN
{
    public class UserLogin 
    {

        public string ApplicationName { get; set; }
        public string ProviderName { get; set; }
        public string ProviderUserId { get; set; }
        public string ProviderUserName { get; set; }
       
        public string MembershipUserName { get; set; }
        public Nullable<System.DateTime> LastUsedUtc { get; set; }

        public UserLoginData UserLoginData { get; set; }
    }

    public class UserLoginData
    {
        public UserLoginData()
        {
            this.UsersOpenAuthLogins = new HashSet<UserLogin>();
        }

        public string ApplicationName { get; set; }
        public string MembershipUserName { get; set; }

        public bool HasLocalPassword { get; set; }

        public virtual ICollection<UserLogin> UsersOpenAuthLogins{ get; set; }
    }
}
