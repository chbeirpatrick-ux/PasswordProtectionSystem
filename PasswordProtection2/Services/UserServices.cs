using PasswordProtectionSystem.Data;
using PasswordProtectionSystem.Model;
using System;

namespace PasswordProtectionSystem.Services
{
    public static class UserService
    {
        
        public static void RegisterUser(string username, string password)
        {
            
            string userId = Guid.NewGuid().ToString();

          
            DateTime timestamp = DateTime.UtcNow;

           
            PlainPasswordRecord plainRecord = new PlainPasswordRecord
            {
                UserId = userId,
                Username = username,
                PasswordPlain = password
            };
            InMemoryDatabase.PlainTable.Add(plainRecord);

            
            string hash = HashService.ComputeSha256(password);
            HashedPasswordRecord hashedRecord = new HashedPasswordRecord
            {
                UserId = userId,
                Username = username,
                PasswordHash = hash
            };
            InMemoryDatabase.HashedTable.Add(hashedRecord);

            
            string salt = SaltService.GenerateSalt(username, userId, timestamp);
            string saltedHash = HashService.ComputeSha256(salt + password);

            SaltedPasswordRecord saltedRecord = new SaltedPasswordRecord
            {
                UserId = userId,
                Username = username,
                Salt = salt,
                PasswordSaltedHash = saltedHash,
                Timestamp = timestamp
            };
            InMemoryDatabase.SaltedTable.Add(saltedRecord);

            
            Console.WriteLine("\n....... REGISTRATION SUCCESSFUL......");
            Console.WriteLine($"User ID   : {userId}");
            Console.WriteLine($"Username  : {username}");
            Console.WriteLine($"Timestamp : {timestamp:O}");

            Console.WriteLine("\n...Table 1: PLAIN TEXT (INSECURE)...");
            Console.WriteLine($"Password  : {plainRecord.PasswordPlain}");

            Console.WriteLine("\n... Table 2: SHA-256 HASH (NO SALT) ...");
            Console.WriteLine($"Hash      : {hashedRecord.PasswordHash}");

            Console.WriteLine("\n... Table 3: SALTED SHA-256 (SECURE)...");
            Console.WriteLine($"Salt      : {saltedRecord.Salt}");
            Console.WriteLine($"SaltedHash: {saltedRecord.PasswordSaltedHash}");
        }

        
        public static void LoginUser(string username, string password)
        {
            Console.WriteLine("\n........ LOGIN ATTEMPT ..........");

           
            var plainUser = InMemoryDatabase.PlainTable.Find(u => u.Username == username);
            if (plainUser != null && plainUser.PasswordPlain == password)
                Console.WriteLine(" Login OK (plain match)");
            else
                Console.WriteLine(" Login FAILED");

            
            var hashedUser = InMemoryDatabase.HashedTable.Find(u => u.Username == username);
            if (hashedUser != null)
            {
                string inputHash = HashService.ComputeSha256(password);
                if (inputHash == hashedUser.PasswordHash)
                    Console.WriteLine(" Login OK (hash match)");
                else
                    Console.WriteLine(" Login FAILED");
            }

            
            var saltedUser = InMemoryDatabase.SaltedTable.Find(u => u.Username == username);
            if (saltedUser != null)
            {
                
                string recomputedSalt = SaltService.GenerateSalt(
                    saltedUser.Username, saltedUser.UserId, saltedUser.Timestamp);

                string inputSaltedHash = HashService.ComputeSha256(recomputedSalt + password);

                if (inputSaltedHash == saltedUser.PasswordSaltedHash)
                    Console.WriteLine(" Login OK (salted hash match)");
                else
                    Console.WriteLine(" Login FAILED");
            }
        }
    }
}