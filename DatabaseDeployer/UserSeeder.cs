using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Domain.Entities;
using DomainDrivenDatabaseDeployer;
using FizzWare.NBuilder;
using NHibernate;

namespace DatabaseDeployer
{
    class UserSeeder : IDataSeeder
    {
                readonly ISession _session;

        public UserSeeder(ISession session)
        {
            _session = session;
        }


        public void Seed()
        {
            var operacion = new User
            {
                Name = "Gabriel",

                Email = "jgpaz5@gmail.com",

                Password = Encrypt("thenetwork13"),

                Telephone = "95744687",

                Info = "Im not a company, just a dude"
            };
                _session.Save(operacion);
            
        }

        static readonly string PasswordHash = "P@@Sw0rd";
        static readonly string SaltKey = "S@LT&KEY";
        static readonly string VIKey = "@1B2c3D4e5F6g7H8";

        public static string Encrypt(string plainText)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.Zeros };
            var encryptor = symmetricKey.CreateEncryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));

            byte[] cipherTextBytes;

            using (var memoryStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                    cryptoStream.FlushFinalBlock();
                    cipherTextBytes = memoryStream.ToArray();
                    cryptoStream.Close();
                }
                memoryStream.Close();
            }
            return Convert.ToBase64String(cipherTextBytes);
        }
    }


}
