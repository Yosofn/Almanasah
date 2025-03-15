using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Model.Data;
using Model.DTOs;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        public UsersController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserDto registerDto)
        {
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == registerDto.Email || u.Phone == registerDto.Phone);

            if (existingUser != null)
            {
                return BadRequest(new { succeed = false, message = "User already exists.", data = (string)null });
            }

            var user = new User
            {
                Name = registerDto.Name,
                Email = registerDto.Email,
                Password = registerDto.Password,
                Phone = registerDto.Phone,
                ParentPhone = registerDto.ParentPhone,
                Government = registerDto.Government,
                NationalId = registerDto.NationalId,
                RegisterDate = DateTime.Now,
                UserType = registerDto.UserType
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { succeed = true, message = "User registered successfully.", data = user });
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(string EmailOrPhone, string Password)
        {
            // تحقق من وجود المستخدم باستخدام البريد الإلكتروني أو رقم الهاتف
            var user = await _context.Users
                .FirstOrDefaultAsync(u => (u.Email == EmailOrPhone || u.Phone == EmailOrPhone) && u.Password == Password);

            if (user == null)
            {
                return Unauthorized(new { succeed = false, message = "Invalid email/phone or password.", data = (string)null, });
            }

            // لو كان تسجيل الدخول ناجح، يمكنك إضافة منطق إنشاء JWT أو جلسة
            return Ok(new { succeed = true, message = "Login successful.", data = user });
        }


        [HttpPut("EditUser{id}")]
        public async Task<IActionResult> EditUser(int id, UserDto updateUserDto)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound(new { succeed = false, message = "User not found.", data = (string)null });
            }

            // تحديث المعلومات
            user.Name = updateUserDto.Name ?? user.Name;
            user.Email = updateUserDto.Email ?? user.Email;
            user.Phone = updateUserDto.Phone ?? user.Phone;
            user.ParentPhone = updateUserDto.ParentPhone ?? user.ParentPhone;
            user.Government = updateUserDto.Government ?? user.Government;
            user.NationalId = updateUserDto.NationalId ?? user.NationalId;
            user.UserType = updateUserDto.UserType ?? user.UserType;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Users.Any(e => e.Id == id))
                {
                    return NotFound(new { succeed = false, message = "User not found.", data = (string)null, errorDetails = (string)null });
                }
                else
                {
                    throw;


                }



            }
            return Ok(new { succeed = true, message = "User updated successfully.", data = user, errorDetails = (string)null });
        }

        [HttpGet("SearchUserWithCourses")]
        public async Task<IActionResult> SearchUserWithCourses(string searchTerm, int pageNumber, int pageSize)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                return BadRequest("Page number and page size must be greater than 0.");
            }

            var usersQuery = _context.Users
                .Where(u => u.Phone.Contains(searchTerm) || u.Email.Contains(searchTerm) || u.ParentPhone.Contains(searchTerm))
                .Select(u => new
                {
                    u.Id,
                    u.Name,
                    u.Phone,
                    u.Email,
                    u.ParentPhone,
                    u.Password,
                    u.NationalId,

                    u.UserType,
                    Courses = u.UserCourses.Select(uc => new { uc.CourseId, uc.Course.Name }).ToList()
                });

            var totalUsers = await usersQuery.CountAsync();
            var users = await usersQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            if (!users.Any())
            {
                return NotFound(new { succeed = false, message = "No users found matching the search criteria.", data = (string)null, errorDetails = (string)null });
            }

            return Ok(new
            {
                succeed = true,
                message = "Users fetched successfully.",
                data = new
                {
                    TotalUsers = totalUsers,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    Users = users
                },
            });
        }
        [HttpPost("AddCourses")]
        public async Task<IActionResult> AddCoursesToUser(AddCoursesToUserDto addCoursesToUserDto)
        {
            var user = await _context.Users.FindAsync(addCoursesToUserDto.UserId);

            if (user == null)
            {
                return NotFound(new { succeed = false, message = "User not found.", data = (string)null });
            }

            var existingCoursesIds = new List<int>();
            var newCoursesIds = new List<int>();

            foreach (var courseId in addCoursesToUserDto.CoursesIds)
            {
                //var courseExists = await _context.Courses.AnyAsync(c => c.Id == courseId);

                //if (!courseExists)
                //{
                //    return BadRequest(new { succeed = false, message = $"Course with ID {courseId} does not exist.", data = (string)null, errorDetails = (string)null });
                //}
                var exists = await _context.UserCourses.AnyAsync(uc => uc.UserId == addCoursesToUserDto.UserId && uc.CourseId == courseId);
                if (exists)
                {
                    existingCoursesIds.Add(courseId);
                }
                else
                {
                    newCoursesIds.Add(courseId);
                }
            }

            foreach (var courseId in newCoursesIds)
            {
                var userCourse = new UserCourse
                {
                    UserId = user.Id,
                    CourseId = courseId
                };
                _context.UserCourses.Add(userCourse);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return BadRequest(new { succeed = false, message = "Error adding courses to user.", data = (string)null, errorDetails = (string)null });
            }

            return Ok(new
            {
                succeed = true,
                message = "Courses added to user successfully.",
                data = new { existingCoursesIds, newCoursesIds },
                errorDetails = (string)null
            });
        }

        [HttpGet("GetInstructors")]
        public async Task<IActionResult> GetInstructors()
        {
            var instructors = await _context.Users
                .Where(u => u.UserType == 1)
                .ToListAsync();

            if (!instructors.Any())
            {
                return NotFound(new { succeed = false, message = "No instructors found.", data = (string)null });
            }

            return Ok(new
            {
                succeed = true,
                message = "Instructors fetched successfully.",
                data = instructors
                
            });
        }


        [HttpPost("UploadImage/{id}")]
        public async Task<IActionResult> UploadImage(int id, IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                return BadRequest(new { succeed = false, message = "Invalid image file.", data = (string)null });
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { succeed = false, message = "User not found.", data = (string)null });
            }
            var folderPath = Path.Combine(_env.WebRootPath, "Users");
            var imagePath = Path.Combine(_env.WebRootPath, "Users", $"{id}.jpg");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
            using (var stream = new FileStream(imagePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            return Ok(new { succeed = true, message = "Image uploaded successfully.", data = (string)null });
        }
            [HttpGet("GetImage/{id}")]
            public IActionResult GetImage(int id)
            {
                var imagePath = Path.Combine(_env.WebRootPath, "Users", $"{id}.jpg");
                if (!System.IO.File.Exists(imagePath))
                {
                    return NotFound(new { succeed = false, message = "Image not found", data = (string)null });
                }

                var imageFileStream = System.IO.File.OpenRead(imagePath);
                return File(imageFileStream, "image/jpeg");
            }

        
            [HttpDelete("{id}")]
            public async Task<IActionResult> DeleteUser(int id)
            {
                var user = await _context.Users.FindAsync(id);

                if (user == null)
                {
                    return NotFound(new { succeed = false, message = "User not found.", data = (string)null });
                }

                // حذف السجلات المرتبطة بالمستخدم في الجداول الأخرى
                var userLectures = _context.UserLectures.Where(ul => ul.UserId == id);
                var userCourses = _context.UserCourses.Where(uc => uc.UserId == id);

                _context.UserLectures.RemoveRange(userLectures);
                _context.UserCourses.RemoveRange(userCourses);

                // حذف المستخدم
                _context.Users.Remove(user);

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    return BadRequest(new { succeed = false, message = "Error deleting user.", data = (string)null, errorDetails = (string)null });
                }

                return Ok(new { succeed = true, message = "User deleted successfully.", data = (string)null, errorDetails = (string)null });
            }




        }
    } 
