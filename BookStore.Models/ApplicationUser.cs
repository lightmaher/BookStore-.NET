﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Models
{
  public  class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public string StreetAdress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        [NotMapped]
        public string Role { get; set; }
        public int? CompanyId { get; set; }
        public Company Company { get; set; }



    }
}
