using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using PablitoJere.DTOs;
using System.Security.Cryptography;

namespace PablitoJere.Services
{
    public class HashService
    {
        public HashResult Hash(string text)
        {
            var salt = new byte[16];
            using(var random = RandomNumberGenerator.Create())
            {
                random.GetBytes(salt);
            }
            return Hash(text, salt);
        }

        public HashResult Hash(string text, byte[] salt)
        {
            var key = KeyDerivation.Pbkdf2(
                password: text,
                salt: salt,
                iterationCount: 10000,
                numBytesRequested: 32,
                prf: KeyDerivationPrf.HMACSHA1
                );

            var hash = Convert.ToBase64String(key);
            return new HashResult()
            {
                Hash = hash,
                Salt = salt
            };
        }
    }
}
