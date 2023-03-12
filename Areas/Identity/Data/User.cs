using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Project_MMXXIII.Areas.Identity.Data;

// Add profile data for application users by adding properties to the User class
public class User : IdentityUser {
    public string FristName { get; set; }

    public string LastName { get; set; }

    public string Nickname { get; set; }
    
}

