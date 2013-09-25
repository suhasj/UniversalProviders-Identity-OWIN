using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversalProviders_Identity_OWIN
{
    public class Application
    {
        public Application()
        {
            this.PasswordLogins = new HashSet<PasswordLogin>();
            this.Roles = new HashSet<Role>();
            this.Users = new HashSet<User>();
        }

        public string ApplicationName { get; set; }
        public System.Guid ApplicationId { get; set; }
        public string Description { get; set; }

        public virtual ICollection<PasswordLogin> PasswordLogins { get; set; }
        public virtual ICollection<Role> Roles { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
