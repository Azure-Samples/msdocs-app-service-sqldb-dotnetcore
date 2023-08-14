using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetCoreSqlDb.Models
{
    public class Account
    {
        public User User { get; set; }        
        [Key] 
        public string AccountId { get; set; }
        public string AccountIdKey { get; set; }
        public string AccountMode { get; set; }
        public string AccountDesc { get; set; }
        public string AccountName { get; set; }
        public string AccountType { get; set; }
        public string InstitutionType { get; set; }
        public string AccountStatus { get; set; }
        public int ClosedDate { get; set; }
        public bool ShareWorksAccount { get; set; }
        public bool FcManagedMssbClosedAccount { get; set; }

        
    }
}
