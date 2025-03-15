using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DTOs
{
    public class AddCoursesToUserDto
    {
        public int UserId { get; set; }
        public int[] CoursesIds { get; set; }
    }
}
