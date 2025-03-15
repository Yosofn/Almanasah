using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DTOs
{
    public class CreateUpdateCourseDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal? Price { get; set; }
        public IFormFile? Image { get; set; } // For image upload
        public int? TeacherId { get; set; }
        public int order { get; set; }

    }

}
