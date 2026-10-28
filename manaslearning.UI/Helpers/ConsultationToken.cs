using System;
using System.Security.Cryptography;
using System.Text;

namespace manaslearning.UI.Helpers
{
    /// <summary>
    /// Order ids and signed return links for paid consultations.
    /// A paid consultation needs no database row: the order id is generated here, Paytm is the
    /// source of truth for whether it was paid, and the return link is signed with the Paytm
    /// merchant key (already a secret in Key Vault) so it cannot be guessed.
    /// </summary>
    public static class ConsultationToken
    {
        public const string OrderPrefix = "CONS-";

        public static string NewOrderId()
        {
            const string alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            var bytes = new byte[6];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            var suffix = new StringBuilder();
            foreach (var b in bytes)
            {
                suffix.Append(alphabet[b % alphabet.Length]);
            }
            return $"{OrderPrefix}{DateTime.UtcNow:yyyyMMddHHmmss}-{suffix}";
        }

        public static bool IsConsultationOrder(string orderId)
        {
            return !string.IsNullOrWhiteSpace(orderId)
                && orderId.Length <= 40
                && orderId.StartsWith(OrderPrefix, StringComparison.Ordinal);
        }

        public static string Sign(string orderId, string key)
        {
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes("consult|" + orderId));
                return BitConverter.ToString(hash).Replace("-", string.Empty).ToLowerInvariant();
            }
        }

        public static bool Verify(string orderId, string signature, string key)
        {
            if (string.IsNullOrWhiteSpace(orderId) || string.IsNullOrWhiteSpace(signature) || string.IsNullOrWhiteSpace(key))
                return false;

            var expected = Encoding.UTF8.GetBytes(Sign(orderId, key));
            var actual = Encoding.UTF8.GetBytes(signature.Trim().ToLowerInvariant());
            return CryptographicOperations.FixedTimeEquals(expected, actual);
        }
    }
}
