using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace UniversalProviders_Identity_OWIN
{
    public class User : IUser
    {
        public User()
        {
            UserId = Guid.NewGuid();
            Roles = new List<Role>();
            ExternalLogins = new List<UserLogin>();
            LastActivityDate = DateTime.Now;
        }

        public System.Guid ApplicationId { get; set; }
        public System.Guid UserId { get; set; }
        public string UserName { get; set; }
        public bool IsAnonymous { get; set; }
        public System.DateTime LastActivityDate { get; set; }


        public string Id
        {
            get { return UserId.ToString(); }
        }

        public string PasswordHash
        {
            get
            {
                return PasswordLogin.Password;
            }
            set
            {
                PasswordLogin.Password = value;
            }
        }

        public string SecurityStamp
        {
            get;
            set;
        }

        public virtual UserClaim Claims { get; private set; }

        public virtual ICollection<Role> Roles { get; private set; }

        public virtual PasswordLogin PasswordLogin { get; set; }

        public ICollection<UserLogin> ExternalLogins { get; set; }

        public void CreatePasswordLogin()
        {
            if (this.PasswordLogin == null)
            {
                this.PasswordLogin = new PasswordLogin()
                {
                    CreateDate = DateTime.Now,
                    IsApproved = false,
                    LastLoginDate = DateTime.Now,
                    LastPasswordChangedDate = DateTime.Now,
                    ApplicationId = this.ApplicationId,
                    LastLockoutDate=DateTime.Parse("1/1/1754"),
                    FailedPasswordAnswerAttemptWindowsStart = DateTime.Parse("1/1/1754"),
                    FailedPasswordAttemptWindowStart = DateTime.Parse("1/1/1754"),
                };
            }
        }
    }

    public class PasswordLogin
    {
        public System.Guid UserId { get; set; }

        public System.Guid ApplicationId { get; set; }

        public string Password { get; set; }
        public int PasswordFormat { get; set; }
        public string PasswordSalt { get; set; }
        public string Email { get; set; }
        public string PasswordQuestion { get; set; }
        public string PasswordAnswer { get; set; }
        public bool IsApproved { get; set; }
        public bool IsLockedOut { get; set; }
        public System.DateTime CreateDate { get; set; }
        public System.DateTime LastLoginDate { get; set; }
        public System.DateTime LastPasswordChangedDate { get; set; }
        public System.DateTime LastLockoutDate { get; set; }
        public int FailedPasswordAttemptCount { get; set; }
        public System.DateTime FailedPasswordAttemptWindowStart { get; set; }
        public int FailedPasswordAnswerAttemptCount { get; set; }
        public System.DateTime FailedPasswordAnswerAttemptWindowsStart { get; set; }

        public virtual User User { get; set; }
        public virtual Application Application { get; set; }

    }

    public class UserPasswordHasher : IPasswordHasher
    {
        public UserPasswordHasher()
        {
            PasswordFormat = 1;
            PasswordSalt = Util.GenerateSalt();
        }

        private string _HashAlgorithm = null;

        public string PasswordSalt { get; set; }

        public int PasswordFormat { get; set; }

        private MembershipPasswordCompatibilityMode _legacyPasswordCompatibilityMode = MembershipPasswordCompatibilityMode.Framework40;
        internal MembershipPasswordCompatibilityMode LegacyPasswordCompatibilityMode
        {
            get
            {
                return _legacyPasswordCompatibilityMode;
            }
            set
            {
                _legacyPasswordCompatibilityMode = value;
            }
        }

        // Taken from Universal Providers
        public string HashPassword(string password)
        {
            if (PasswordFormat == 0)
                return password;

            byte[] bIn = Encoding.Unicode.GetBytes(password);
            byte[] bSalt = Convert.FromBase64String(PasswordSalt);
            byte[] bRet = null;

            // We'll assume PasswordFormat = 1. This is because we would then have to implement
            // legacy provider EncryptPassword method from the System.Web.Security

            HashAlgorithm hm = GetHashAlgorithm();
            KeyedHashAlgorithm kha = hm as KeyedHashAlgorithm;
            if (kha != null)
            {
                if (kha.Key.Length == bSalt.Length)
                {
                    kha.Key = bSalt;
                }
                else if (kha.Key.Length < bSalt.Length)
                {
                    byte[] bKey = new byte[kha.Key.Length];
                    Buffer.BlockCopy(bSalt, 0, bKey, 0, bKey.Length);
                    kha.Key = bKey;
                }
                else
                {
                    byte[] bKey = new byte[kha.Key.Length];
                    for (int iter = 0; iter < bKey.Length; )
                    {
                        int len = Math.Min(bSalt.Length, bKey.Length - iter);
                        Buffer.BlockCopy(bSalt, 0, bKey, iter, len);
                        iter += len;
                    }
                    kha.Key = bKey;
                }
                bRet = kha.ComputeHash(bIn);
            }
            else
            {
                byte[] bAll = new byte[bSalt.Length + bIn.Length];
                Buffer.BlockCopy(bSalt, 0, bAll, 0, bSalt.Length);
                Buffer.BlockCopy(bIn, 0, bAll, bSalt.Length, bIn.Length);
                bRet = hm.ComputeHash(bAll);
            }


            return Convert.ToBase64String(bRet);
        }

        public PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            string encodedpassword = HashPassword(providedPassword);

            bool passwordsmatch = string.Compare(encodedpassword, hashedPassword, StringComparison.Ordinal) == 0;

            return passwordsmatch ? PasswordVerificationResult.Success : PasswordVerificationResult.Failed;
        }

        // Taken from Universal Providers
        internal HashAlgorithm GetHashAlgorithm()
        {
            if (_HashAlgorithm != null)
            {
                return HashAlgorithm.Create(_HashAlgorithm);
            }

            string temp = System.Web.Security.Membership.HashAlgorithmType;
            if (LegacyPasswordCompatibilityMode == MembershipPasswordCompatibilityMode.Framework20 &&
                temp != "MD5")
            {
                temp = "SHA1";
            }

            HashAlgorithm hashAlgo = HashAlgorithm.Create(temp);
            if (hashAlgo == null)
            {
                throw new Exception("Invalid hash algorithm");
            }
            _HashAlgorithm = temp;
            return hashAlgo;
        }
    }
}
