using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversalProviders_Identity_OWIN
{
    public class Role : IRole
    {
        public Role()
        {
        }
        public Role(string name)
        {
            RoleId = Guid.NewGuid();
            Name = name;
        }
        public System.Guid ApplicationId { get; set; }

        public System.Guid RoleId { get; set; }
        public string Description { get; set; }

        public string Id
        {
            get { return RoleId.ToString(); }
        }

        public string Name
        {
            get;
            set;
        }

        public virtual ICollection<User> Users { get; set; }
    }
}
