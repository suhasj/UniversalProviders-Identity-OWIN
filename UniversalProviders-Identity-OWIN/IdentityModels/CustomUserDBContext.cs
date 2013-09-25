using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversalProviders_Identity_OWIN
{
    public class CustomUserDBContext : DbContext
    {
        public CustomUserDBContext()
            : base("DefaultConnection")
        {
            this.Database.Log = Logger;
        }

        private void Logger(string log)
        {
            Debug.WriteLine(log);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Map Roles
            var role = modelBuilder.Entity<Role>();

            role.Map(map =>
                {
                    map.Properties(t => new { t.ApplicationId, t.Description, t.RoleId, t.Name });
                    map.ToTable("Roles");

                    map.Property(t => t.RoleId).HasColumnName("RoleId");
                    map.Property(t => t.Name).HasColumnName("RoleName");

                    map.Property(t => t.Description).HasColumnName("Description");
                    map.Property(t => t.ApplicationId).HasColumnName("ApplicationId");

                });

            role.Ignore(t => t.Id);

            // Map user class
            var user = modelBuilder.Entity<User>();

            user.HasMany(u => u.Roles).WithMany(r => r.Users).Map((config) =>
            {
                config
                    .ToTable("UsersInRoles")
                    .MapLeftKey("UserId")
                    .MapRightKey("RoleId");
            });

            user.Map(map =>
            {
                map.Properties(t => new { t.ApplicationId, t.UserId, t.UserName, t.IsAnonymous, t.LastActivityDate });
                map.ToTable("Users");

                map.Property(u => u.UserId).HasColumnName("UserId");
            });

            user.Ignore(t => t.Id);
            user.Ignore(t => t.SecurityStamp);
            user.Ignore(t => t.PasswordHash);
            user.Ignore(t => t.ExternalLogins);

            // Map PasswordLogin 
            var passwordLogin = modelBuilder.Entity<PasswordLogin>();

            passwordLogin.HasKey(p => p.UserId).ToTable("Memberships");

            passwordLogin.HasRequired(p => p.User).WithOptional(u => u.PasswordLogin);

            passwordLogin.HasRequired(p => p.Application).WithMany(u => u.PasswordLogins);

            // Map user login
            var userLogin = modelBuilder.Entity<UserLogin>();

            userLogin.HasKey(t => new { t.ApplicationName, t.ProviderName, t.ProviderUserId }).ToTable("UsersOpenAuthAccounts");

            // Map UserAuth Data

            var userLoginData = modelBuilder.Entity<UserLoginData>();

            userLoginData.HasKey(t => new { t.ApplicationName, t.MembershipUserName }).ToTable("UsersOpenAuthData");

            userLogin.HasOptional(t => t.UserLoginData).WithMany(u => u.UsersOpenAuthLogins);

            // Map user Claims

            var userClaim = modelBuilder.Entity<UserClaim>();

            userClaim.HasKey(t => new { t.UserId }).ToTable("Profiles");

            userClaim.HasRequired(u => u.User).WithOptional(u => u.Claims);

            // Map application class
            var application = modelBuilder.Entity<Application>();

            application.HasKey(t => new { t.ApplicationId }).ToTable("Applications");
        }

        public virtual IDbSet<Role> Roles { get; set; }
        public virtual IDbSet<User> Users { get; set; }
        public virtual IDbSet<UserLogin> UserLogins { get; set; }
        public virtual IDbSet<PasswordLogin> PasswordLogins { get; set; }
        public virtual IDbSet<UserLoginData> UsersLoginData { get; set; }
        public virtual IDbSet<UserClaim> UserClaims { get; set; }
        public virtual IDbSet<Application> Applications { get; set; }
    }

    // User store
    public class UniversalProviderUserStore : IUserStore<User>, IUserLoginStore<User>, IUserPasswordStore<User>,IUserRoleStore<User>
    {
        private readonly CustomUserDBContext _context;

        public UniversalProviderUserStore(CustomUserDBContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            _context = context;
        }

        public async Task CreateAsync(User user)
        {
            if (user.PasswordLogin == null)
            {
                user.CreatePasswordLogin();
            }

            _context.Users.Add(user);

            foreach (var item in user.ExternalLogins)
            {
                _context.UserLogins.Add(item);
            }

            await _context.SaveChangesAsync();
        }

        public Task<User> FindByIdAsync(string userId)
        {
            if (String.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException("User Id");
            }

            var GuidId = Guid.Parse(userId);

            return _context.Users.SingleOrDefaultAsync<User>(x => x.UserId.Equals(GuidId));
        }

        public Task<User> FindByNameAsync(string userName)
        {
            if (String.IsNullOrEmpty(userName))
            {
                throw new ArgumentNullException("User Name");
            }

            return _context.Users.SingleOrDefaultAsync<User>(x => x.UserName.Equals(userName));
        }

        public Task UpdateAsync(User user)
        {
            return _context.SaveChangesAsync();
        }

        public void Dispose()
        {

        }

        public Task AddLoginAsync(User user, UserLoginInfo loginInfo)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (user.ExternalLogins == null)
            {
                throw new ArgumentException("user");
            }

            if (loginInfo == null)
            {
                throw new ArgumentNullException("login");
            }

            var loginData = new UserLoginData()
            {
                ApplicationName = "/",
                HasLocalPassword = false,
                MembershipUserName = user.UserName
            };

            var userLogin = new UserLogin()
            {
                ProviderName = loginInfo.LoginProvider,
                ProviderUserId = loginInfo.ProviderKey,
                MembershipUserName = user.UserName,
                ProviderUserName = user.UserName,
                UserLoginData = loginData
            };

            user.ExternalLogins.Add(userLogin);

            _context.UserLogins.Add(userLogin);
            _context.UsersLoginData.Add(loginData);

            return _context.SaveChangesAsync();
        }

        public Task<User> FindAsync(UserLoginInfo login)
        {
            if (login == null)
            {
                return Task.FromResult<User>(null);
            }

            var existingLogin = _context.UserLogins.SingleOrDefault(x => x.ProviderName == login.LoginProvider && x.ProviderUserId == login.ProviderKey);

            if (existingLogin == null)
            {
                return Task.FromResult<User>(null);
            }

            var user = _context.Users.SingleOrDefaultAsync<User>(u => u.UserName == existingLogin.MembershipUserName).Result;

            user.ExternalLogins.Add(existingLogin);

            return Task.FromResult<User>(user);
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            IList<UserLoginInfo> logins = new List<UserLoginInfo>();

            foreach (UserLogin login in _context.UserLogins.Where(x => x.MembershipUserName == user.UserName))
            {
                logins.Add(new UserLoginInfo(login.ProviderName, login.ProviderUserId));
            }

            return Task.FromResult(logins);
        }

        public Task RemoveLoginAsync(User user, UserLoginInfo login)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (user.ExternalLogins == null)
            {
                throw new ArgumentException("user");
            }

            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

            var toDeleteLoginData = _context.UsersLoginData.Single(x => x.MembershipUserName == user.UserName);

            _context.UsersLoginData.Remove(toDeleteLoginData);

            _context.Entry(toDeleteLoginData).State = EntityState.Deleted;

            var toDeleteLogin = _context.UserLogins.SingleOrDefault(
                l => l.ProviderName == login.LoginProvider && l.ProviderUserId == login.ProviderKey && l.MembershipUserName == user.UserName);

            _context.UserLogins.Remove(toDeleteLogin);

            _context.Entry(toDeleteLogin).State = EntityState.Deleted;

            return Task.FromResult<object>(null);
        }


        public Task DeleteAsync(User user)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetPasswordHashAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (user.PasswordLogin == null)
            {
                return Task.FromResult<string>(null);
            }

            return Task.FromResult(user.PasswordLogin.Password);
        }

        public Task<bool> HasPasswordAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult(user.PasswordLogin != null);
        }

        public Task SetPasswordHashAsync(User user, string passwordHash)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            user.PasswordLogin.Password = passwordHash;

            return Task.FromResult<object>(null);
        }

        public async Task AddToRoleAsync(User user, string role)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (user.Roles == null)
            {
                throw new ArgumentException("user");
            }

            if (role == null)
            {
                throw new ArgumentNullException("role");
            }

            Role toAdd = await _context.Roles.SingleAsync(r => r.Name == role);

            user.Roles.Add(toAdd);
        }

        public Task<IList<string>> GetRolesAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (user.Roles == null)
            {
                throw new ArgumentException("user");
            }

            IList<string> roles = new List<string>();

            foreach (Role role in user.Roles)
            {
                roles.Add(role.Name);
            }

            return Task.FromResult(roles);
        }

        public Task<bool> IsInRoleAsync(User user, string role)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (user.Roles == null)
            {
                throw new ArgumentException("user");
            }

            if (role == null)
            {
                throw new ArgumentNullException("role");
            }

            bool inRole = user.Roles.Any(r => r.Name == role);
            return Task.FromResult(inRole);
        }

        public Task RemoveFromRoleAsync(User user, string role)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (user.Roles == null)
            {
                throw new ArgumentException("user");
            }

            if (role == null)
            {
                throw new ArgumentNullException("role");
            }

            Role toRemove = user.Roles.Single(r => r.Name == role);
            user.Roles.Remove(toRemove);

            return Task.FromResult<object>(null);
        }
    }

    // Role Store
    public class UniversalProviderRoleStore : IRoleStore<Role>
    {
        private CustomUserDBContext _context;

        public UniversalProviderRoleStore(CustomUserDBContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            _context = context;
        }
        public Task CreateAsync(Role role)
        {
            _context.Roles.Add(role);
            return _context.SaveChangesAsync();
        }

        public Task DeleteAsync(Role role)
        {
            throw new NotImplementedException();
        }

        public Task<Role> FindByIdAsync(string roleId)
        {
            var GuidId = Guid.Parse(roleId);

            return _context.Roles.SingleOrDefaultAsync(r => r.RoleId == GuidId);
        }

        public Task<Role> FindByNameAsync(string roleName)
        {
            return _context.Roles.SingleOrDefaultAsync(r => r.Name == roleName);
        }

        public Task UpdateAsync(Role role)
        {
            return _context.SaveChangesAsync();
        }

        public void Dispose()
        {
        }
    }
}
