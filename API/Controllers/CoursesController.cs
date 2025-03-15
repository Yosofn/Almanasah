using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Model.Data;
using Model.DTOs;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]


    public class CoursesController : ControllerBase

    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public CoursesController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }


        [HttpGet("GetAllCourses")]
        public async Task<IActionResult> GetAllCourses()
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
        [HttpGet("GetPaginatedCourses")]
        public async Task<IActionResult> GetPaginatedCourses( string? searchTerm,  int pageNumber = 1,  int pageSize = 10)
        {
            var query = _context.Courses.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(course => course.Name.Contains(searchTerm) || course.Descryption.Contains(searchTerm));
            }

            var totalCourses = await query.CountAsync();

            var paginatedCourses = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new
            {
                succeed = true,
                message = "Courses fetched successfully.",
                total= totalCourses,
                pagenumber=pageNumber,
                pagesize=pageSize,
                data = paginatedCourses,
            });
        }

        [HttpGet("GetUserCourses/{userId}")]
        public async Task<IActionResult> GetUserCourses(int userId)
        {
            var userCourses = await _context.UserCourses
                .Where(uc => uc.UserId == userId)
                .Select(uc => new
                {
                    uc.CourseId,
                    uc.Course.Name,
                    uc.Course.Descryption,
                    uc.Course.Price,
                    uc.Course.Date,
                    uc.Course.Order,
                    uc.Course.TeacherId,
                    uc.Course.YearId
                })
                .ToListAsync();

            if (!userCourses.Any())
            {
                return NotFound(new
                {
                    succeed = false,
                    message = $"No courses found for user with ID {userId}.",
                    data = (string)null
                });
            }

            return Ok(new
            {
                succeed = true,
                message = "User courses fetched successfully.",
                data = userCourses
            });
        }


        [HttpPost("CreateCourse")]
        public async Task<IActionResult> CreateCourse([FromForm] CreateUpdateCourseDTO courseDto)
        {
            //if (courseDto.Image == null || courseDto.Image.Length == 0)
            //    return BadRequest("No image file provided.");

           

            var course = new Course
            {
                Name = courseDto.Name,
                Descryption = courseDto.Description,
                Price = courseDto.Price,
                Date = DateTime.Now,
                Order = courseDto.order,
                TeacherId = courseDto.TeacherId,
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync(); // Save to get the course ID
            if (courseDto.Image != null && courseDto.Image.Length > 0)
            {
                var imageFileName = $"{course.Id}.jpg";
                var filePath = Path.Combine(_env.WebRootPath, "CourseImages", imageFileName);
                if (!Directory.Exists(Path.Combine(_env.WebRootPath, "CourseImages")))
                {
                    Directory.CreateDirectory(Path.Combine(_env.WebRootPath, "CourseImages"));
                }
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await courseDto.Image.CopyToAsync(stream);
                }
            }
            return Ok(course);
        }


        [HttpPut("UpdateCourse/{id}")]
        public async Task<IActionResult> UpdateCourse(int id, [FromForm] CreateUpdateCourseDTO courseDto)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound($"Course with ID {id} not found.");
            }

            course.Name = courseDto.Name;
            course.Descryption = courseDto.Description;
            course.Price = courseDto.Price;
            course.Date = DateTime.Now; 
            course.Order = courseDto.order;
            course.TeacherId = courseDto.TeacherId;

            await _context.SaveChangesAsync();

            if (courseDto.Image != null && courseDto.Image.Length > 0)
            {
                var imageFileName = $"{course.Id}.jpg";
                var filePath = Path.Combine(_env.WebRootPath, "CourseImages", imageFileName);

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await courseDto.Image.CopyToAsync(stream);
                }
            }

            return Ok(course);
        }


        [HttpDelete("DeleteUserCourse")]
        public async Task<IActionResult> DeleteUserCourse(int userId, int courseId)
        {
            var blockedLecture = await _context.UserCourses
                .FirstOrDefaultAsync(bl => bl.UserId == userId && bl.CourseId == courseId);

            if (blockedLecture == null)
            {
                return NotFound($"Lessons with UserId {userId} and LessonId {courseId} not found.");
            }

            _context.UserCourses.Remove(blockedLecture);
            await _context.SaveChangesAsync();

            return Ok("Lessons deleted successfully.");
        }


        //[HttpDelete("DeleteCourse/{id}")]
        //public async Task<IActionResult> DeleteCourse(int id)
        //{
        //    var course = await _context.Courses
        //        .Include(c => c.CoursePackages)
        //        .Include(c => c.Lectures)
        //        .Include(c => c.UserCourses).
        //        Include(y=>y.Year)
        //        .FirstOrDefaultAsync(c => c.Id == id);

        //    if (course == null)
        //    {
        //        return NotFound(new
        //        {
        //            succeed = false,
        //            message = $"Course with ID {id} not found."
        //        });
        //    }

        //    _context.CoursePackages.RemoveRange(course.CoursePackages);
        //    _context.Lectures.RemoveRange(course.Lectures);             
        //    _context.UserCourses.RemoveRange(course.UserCourses);
        //    _context.Years.RemoveRange(course.Year);


        //    _context.Courses.Remove(course);

        //    await _context.SaveChangesAsync();

        //    return Ok(new
        //    {
        //        succeed = true,
        //        message = $"Course with ID {id} and all related data have been deleted successfully."
        //    });
        //}

    }
}
