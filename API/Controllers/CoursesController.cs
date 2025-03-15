using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Model.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]


    public class CoursesController : ControllerBase

    {
        private readonly AppDbContext _context;

        public CoursesController(AppDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<IActionResult> GetCourses()
        {
            var courses = await _context.Courses.ToListAsync();
            return Ok(new
            {
                succeed = true,
                message = "Courses fetched successfully.",
                data = courses,
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound(new 
                    {
                    succeed = false,
                message = "Not found",
                data = course,
            });


            }

            return Ok(new
            {
                succeed = true,
                message = "Course fetched successfully.",
                data = course,
            });
        }

    }
}
