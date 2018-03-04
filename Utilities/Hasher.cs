using System;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace webZone.Utilities
{
    public class Hasher
    {
        static private Byte[] _salt;

        public Hasher()
        {
            Random rnd = new Random();
            Byte[] randomSalt = new byte[16];
            rnd.NextBytes(randomSalt);

            _salt = randomSalt;
        }

        public Hasher(string salt)
        {
            _salt = Convert.FromBase64String(salt);
        }

        public string GetSalt()
        {
            return Convert.ToBase64String(_salt);
        }

        public string GetPassword(string password)
        {
            Byte[] hashedPassword = KeyDerivation.Pbkdf2(
                password: password,
                salt: _salt,
                prf: KeyDerivationPrf.HMACSHA512,
                iterationCount: 1000,
                numBytesRequested: 32
            );

            return Convert.ToBase64String(hashedPassword);
        }

    }
}
