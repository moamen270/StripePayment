﻿using Microsoft.AspNetCore.Identity;

namespace StripePayment.Models
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
    }
}