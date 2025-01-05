using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace SharedModels.Helpers
{
    public static class PasswordHelper
    {
        public static string HashPassword(string password)
        {
            //var salt = Guid.NewGuid().ToString();
            //var hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            //	password: password,
            //	salt: Convert.FromBase64String(salt),
            //	prf: KeyDerivationPrf.HMACSHA256,
            //	iterationCount: 10000,
            //	numBytesRequested: 256 / 8));
            //return $"{salt}${hashedPassword}";

            return password;
        }

        public static bool ValidatePassword(string storedHash, string password)
        {
            //var parts = storedHash.Split('$');
            //var salt = parts[0];
            //var hash = parts[1];
            //var hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            //	password: password,
            //	salt: Convert.FromBase64String(salt),
            //	prf: KeyDerivationPrf.HMACSHA256,
            //	iterationCount: 10000,
            //	numBytesRequested: 256 / 8));
            return storedHash == password;
        }
    }
}

