using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace UniversalProviders_Identity_OWIN
{
    // Taken from Universal Providers
    public static class Util
    {
        public static string GenerateSalt()
        {
            string result;
            using (RNGCryptoServiceProvider rNGCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                byte[] array = new byte[16];
                rNGCryptoServiceProvider.GetBytes(array);
                result = Convert.ToBase64String(array);
            }
            return result;
        }
    }
}
