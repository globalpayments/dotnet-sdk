using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using GlobalPayments.Api.Utils;

namespace GlobalPayments.Api.Entities.Payroll {
    internal class PayrollEncoder : IRequestEncoder {
        public string Username { get; set; }
        public string ApiKey { get; set; }

        public PayrollEncoder(string username = null, string apiKey = null) {
            this.Username = username;
            this.ApiKey = apiKey;
        }

        public string Encode(object value) {
            if (value == null)
                return null;

            using (var aes = Aes.Create()) {
                var key = new Rfc2898DeriveBytes(ApiKey, Encoding.UTF8.GetBytes(Username.PadRight(8, ' ')), 1000);

                var ems = new MemoryStream();
                using (var encrypt = new CryptoStream(ems, aes.CreateEncryptor(key.GetKey(), key.GetVector()), CryptoStreamMode.Write)) {
                    byte[] textBytes = Encoding.UTF8.GetBytes(value.ToString());
                    encrypt.Write(textBytes, 0, textBytes.Length);
                    encrypt.FlushFinalBlock();
                }
                return Convert.ToBase64String(ems.ToArray());
            }
        }

        public string Decode(object value) {
            if (value == null)
                return null;

            using (var aes = Aes.Create()) {
                var key = new Rfc2898DeriveBytes(ApiKey, Encoding.UTF8.GetBytes(Username.PadRight(8, ' ')), 1000);

                var dms = new MemoryStream();
                using (var decrypt = new CryptoStream(dms, aes.CreateDecryptor(key.GetKey(), key.GetVector()), CryptoStreamMode.Write)) {
                    byte[] cypherBytes = Convert.FromBase64String(value.ToString());

                    decrypt.Write(cypherBytes, 0, cypherBytes.Length);
                    decrypt.Flush();
                }
                return Encoding.UTF8.GetString(dms.ToArray());
            }
        }
    }
}