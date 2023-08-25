using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;

namespace DotNetCoreSqlDb.Models
{
    public class User
    {
        public User()
        {
            Accounts = new List<Account>();
        }

      
        public List<Account> Accounts { get; set; }

        [Key]
        public string ConsumerKey { get; set; }     

        public string ConsumerSecret { get; set; }
        public string UserName { get; set; }
       
        public string UserEmail { get; set; }
        
        public DateTime UserCreatedDate { get; set; }

        public string EtradeBaseUrl { get; set; }
        public string AccessToken { get; set; }
        public string AccessTokenSecret { get; set; }
        public string Nonce { get; set; }
        public long Timestamp { get; set; }   
        public string RequestToken { get; set; }
        public string RequestTokenSecret { get; set; }
        public string Verifier { get; set; }
      
        public DateTime TokenCreatedDate { get; set; }
    }
}
