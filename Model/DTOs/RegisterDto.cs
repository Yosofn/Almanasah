using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DTOs
{
    public class UserDto
    {
       
            public string Name { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public string Phone { get; set; }
            public string ParentPhone { get; set; }
            public string Government { get; set; }
            public string NationalId { get; set; }
            public int? UserType { get; set; }
        }
    
}
