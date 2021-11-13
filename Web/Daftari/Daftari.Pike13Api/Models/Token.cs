using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Daftari.Pike13Api.Models
{
    public class Token
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long TokenID { get; set; }

        public string Username { get; set; }

        public string AccessToken { get; set; }
 
        public DateTime RefreshTime { get; set; }

        public ICollection<AccountPeople> People { get; set; }
    }
}