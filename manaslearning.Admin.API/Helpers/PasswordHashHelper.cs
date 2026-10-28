using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Security.Cryptography;
using System.Text;

namespace manaslearning.Admin.API.Helpers
{
    public class PasswordHashHelper
    {
        /*
            https://github.com/aspnet/Identity/blob/master/src/Core/UserManager.cs
            https://github.com/aspnet/Identity/blob/feedcb5c53444f716ef5121d3add56e11c7b71e5/src/Core/Base32.cs
        */
        private static readonly string _base32Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
        private static readonly string specialCharcters = "!@#$*";
        private static readonly string digits = "1234567890";

        public string HashPassword(string password = null)
        {
            if (password == null)
            {
                password = GenerateRandomPassword();
            }
            return Convert.ToBase64String(HashPasswordV3(password));
        }

        public string HashPassword()
        {
            return Convert.ToBase64String(HashPasswordV3(null));
        }

        private static byte[] HashPasswordV3(string password)
        {
            return HashPasswordV3(password, RandomNumberGenerator.Create(),
                prf: KeyDerivationPrf.HMACSHA256,
                iterCount: 1,
                saltSize: 128 / 8,
                numBytesRequested: 256 / 8);
        }

        private static byte[] HashPasswordV3(string password, RandomNumberGenerator rng, KeyDerivationPrf prf, int iterCount, int saltSize, int numBytesRequested)
        {
            // Produce a version 3 (see comment above) text hash.
            byte[] salt = new byte[saltSize];
            rng.GetBytes(salt);
            byte[] subkey = KeyDerivation.Pbkdf2(password, salt, prf, iterCount, numBytesRequested);

            var outputBytes = new byte[13 + salt.Length + subkey.Length];
            outputBytes[0] = 0x01; // format marker
            WriteNetworkByteOrder(outputBytes, 1, (uint)prf);
            WriteNetworkByteOrder(outputBytes, 5, (uint)iterCount);
            WriteNetworkByteOrder(outputBytes, 9, (uint)saltSize);
            Buffer.BlockCopy(salt, 0, outputBytes, 13, salt.Length);
            Buffer.BlockCopy(subkey, 0, outputBytes, 13 + saltSize, subkey.Length);
            return outputBytes;
        }

        private static void WriteNetworkByteOrder(byte[] buffer, int offset, uint value)
        {
            buffer[offset + 0] = (byte)(value >> 24);
            buffer[offset + 1] = (byte)(value >> 16);
            buffer[offset + 2] = (byte)(value >> 8);
            buffer[offset + 3] = (byte)(value >> 0);
        }

        public static string NewSecurityStamp()
        {
            byte[] bytes = new byte[20];
            RandomNumberGenerator _rng = RandomNumberGenerator.Create();
            _rng.GetBytes(bytes);
            return ToBase32(bytes);
        }

        public static string ToBase32(byte[] input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            StringBuilder sb = new StringBuilder();
            for (int offset = 0; offset < input.Length;)
            {
                byte a, b, c, d, e, f, g, h;
                int numCharsToOutput = GetNextGroup(input, ref offset, out a, out b, out c, out d, out e, out f, out g, out h);

                sb.Append((numCharsToOutput >= 1) ? _base32Chars[a] : '=');
                sb.Append((numCharsToOutput >= 2) ? _base32Chars[b] : '=');
                sb.Append((numCharsToOutput >= 3) ? _base32Chars[c] : '=');
                sb.Append((numCharsToOutput >= 4) ? _base32Chars[d] : '=');
                sb.Append((numCharsToOutput >= 5) ? _base32Chars[e] : '=');
                sb.Append((numCharsToOutput >= 6) ? _base32Chars[f] : '=');
                sb.Append((numCharsToOutput >= 7) ? _base32Chars[g] : '=');
                sb.Append((numCharsToOutput >= 8) ? _base32Chars[h] : '=');
            }

            return sb.ToString();
        }

        // returns the number of bytes that were output
        private static int GetNextGroup(byte[] input, ref int offset, out byte a, out byte b, out byte c, out byte d, out byte e, out byte f, out byte g, out byte h)
        {
            uint b1, b2, b3, b4, b5;

            int retVal;
            switch (offset - input.Length)
            {
                case 1: retVal = 2; break;
                case 2: retVal = 4; break;
                case 3: retVal = 5; break;
                case 4: retVal = 7; break;
                default: retVal = 8; break;
            }

            b1 = (offset < input.Length) ? input[offset++] : 0U;
            b2 = (offset < input.Length) ? input[offset++] : 0U;
            b3 = (offset < input.Length) ? input[offset++] : 0U;
            b4 = (offset < input.Length) ? input[offset++] : 0U;
            b5 = (offset < input.Length) ? input[offset++] : 0U;

            a = (byte)(b1 >> 3);
            b = (byte)(((b1 & 0x07) << 2) | (b2 >> 6));
            c = (byte)((b2 >> 1) & 0x1f);
            d = (byte)(((b2 & 0x01) << 4) | (b3 >> 4));
            e = (byte)(((b3 & 0x0f) << 1) | (b4 >> 7));
            f = (byte)((b4 >> 2) & 0x1f);
            g = (byte)(((b4 & 0x3) << 3) | (b5 >> 5));
            h = (byte)(b5 & 0x1f);

            return retVal;
        }

        public static string GenerateConcurrencyStampAsync()
        {
            return Guid.NewGuid().ToString();
        }

        public string GenerateRandom(int length, bool isUpper = true)
        {
            StringBuilder result = new StringBuilder();
            Random random = new Random();
            char letter;
            for (int i = 0; i < length; i++)
            {
                double flt = random.NextDouble();
                int shift = Convert.ToInt32(Math.Floor(25 * flt));
                letter = Convert.ToChar(shift + 65);
                if (!isUpper)
                {
                    letter = Char.ToLower(letter);
                }
                result.Append(letter);
            }
            return result.ToString();
        }

        public string GetRandom(string chars)
        {
            Random rnd = new Random();
            int index = rnd.Next(chars.Length);
            return chars.ToCharArray()[index].ToString();
        }

        public string GenerateRandomPassword()
        {
           // Example: Aaaa&1aa 
           return GenerateRandom(1) + GenerateRandom(3, false) + GetRandom(specialCharcters) + GetRandom(digits) + GenerateRandom(2, false);
        }
    }
}
