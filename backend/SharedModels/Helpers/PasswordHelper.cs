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
            // Generate a salt as a base64 string
            var salt = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            
            // Hash the password with the salt
            var hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: Convert.FromBase64String(salt),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
            
            // Return the combined salt and hash
            return $"{salt}${hashedPassword}";
        }

        public static bool ValidatePassword(string storedHash, string password)
        {
            // Split the stored hash into salt and hash
            var parts = storedHash.Split('$');
            if (parts.Length != 2)
                return false; // Invalid stored hash format

            var salt = parts[0];
            var storedHashedPassword = parts[1];
            
            // Hash the input password with the same salt
            var hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: Convert.FromBase64String(salt),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
            
            // Compare the hashes
            return storedHashedPassword == hashedPassword;
        }
    }
}

