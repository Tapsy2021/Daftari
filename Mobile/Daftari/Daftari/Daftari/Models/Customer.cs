using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Daftari.Models
{
    public class Customer
    {
        [PrimaryKey]
        public Guid CustomerID { get; set; }
        public long PersonID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhotoMD { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
