using System.Collections.Generic;
using PasswordProtectionSystem.Model;

namespace PasswordProtectionSystem.Data
{
    public static class InMemoryDatabase
    {
        
        public static List<PlainPasswordRecord> PlainTable = new List<PlainPasswordRecord>();

        
        public static List<HashedPasswordRecord> HashedTable = new List<HashedPasswordRecord>();

        
        public static List<SaltedPasswordRecord> SaltedTable = new List<SaltedPasswordRecord>();
    }
}