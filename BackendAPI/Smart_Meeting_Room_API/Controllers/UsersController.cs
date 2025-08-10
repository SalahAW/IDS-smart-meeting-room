using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Smart_Meeting_Room_API.Dtos.Users;
using Smart_Meeting_Room_API.Models;
using Smart_Meeting_Room_API.Services.PasswordHasher;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Smart_Meeting_Room_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    //[Authorize]
    public class UsersController : ControllerBase
    {
        private readonly SmartMeetingRoomDbContext _context;
        private readonly ILogger<UsersController> _logger;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IConfiguration _configuration;

        public UsersController(SmartMeetingRoomDbContext context,
            ILogger<UsersController> logger , IPasswordHasher passwordHasher
            , IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            pageSize = Math.Min(pageSize, 100); // Limit page size to 100

            try
            {

                var users = await _context.Users
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    RoleId = u.RoleId,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt
                }).ToListAsync(cancellationToken);

                return Ok(new
                {
                    users,
                    Message = "All Users Retrieved Successfully"
                });

            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while retrieving users.");
                return StatusCode(500, new { Message = "Internal Server Error" });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById([FromRoute]
        int id, CancellationToken cancellationToken = default)

        {
            if (id <= 0)
            {
                return BadRequest(new { Message = "Invalid User ID" });
            }

            var user = await _context.Users.FindAsync(id, cancellationToken);

            if (user == null)
            {
                return NotFound(new { Message = "User not found" });
            }

            var userDto = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                RoleId = user.RoleId,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };

            return Ok(new
            {
                user = userDto,
                Message = "User Retrieved Successfully"
            });

        }

        [HttpGet("{userId}/meetings")]
        public async Task<IActionResult> GetUserMeetings(
        [FromRoute(Name = "userId")] int userId,
        CancellationToken cancellationToken = default)
        {
            try
            {
                if (userId <= 0)
                {
                    return BadRequest("Invalid User Id");
                }

                var user = await _context.Users
                    .Include(u => u.Meetings)
                    .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

                if (user == null)
                {
                    return NotFound("User Not Found");
                }

                return Ok(new
                {
                    userId = user.Id,
                    userName = user.Name,
                    meetings = user.Meetings.Select(m => new
                    {
                        id = m.Id,
                        title = m.Title,
                        startTime = m.StartTime,
                        endTime = m.EndTime,
                        status = m.Status,
                        roomId = m.RoomId
                    }).ToList()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving meetings for user with id [" + userId + "]");
                return StatusCode(500);
            }
        }

        [HttpGet("{userId}/action-items")]
        public async Task<IActionResult> GetUserActionItems(
            [FromRoute(Name = "userId")] int userId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (userId <= 0)
                {
                    return BadRequest("Invalid User Id");
                }

                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

                if (user == null)
                {
                    return NotFound("User Not Found");
                }

                
                var actionItems = await _context.ActionItems
                    .Include(ai => ai.Mom)
                        .ThenInclude(m => m.Meeting) 
                    .Where(ai => ai.AssignedTo == userId)
                    .Select(ai => new
                    {
                        id = ai.Id,
                        description = ai.Description,
                        dueDate = ai.DueDate,
                        status = ai.Status,
                        createdAt = ai.CreatedAt,
                        meetingId = ai.Mom.MeetingId,
                        meetingTitle = ai.Mom.Meeting.Title,
                        momId = ai.MomId
                    })
                    .ToListAsync(cancellationToken);

                return Ok(new
                {
                    userId,
                    userName = user.Name,
                    actionItems
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving action items for user with id [" + userId + "]");
                return StatusCode(500);
            }
        }


        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto createUser,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var user = new User
                {
                    Name = createUser.Name,
                    Email = createUser.Email,
                    PasswordHash = _passwordHasher.HashPassword(createUser.Password),
                    RoleId = createUser.RoleId
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync(cancellationToken);

                var userDto = new UserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    RoleId = user.RoleId,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt
                };

                return CreatedAtAction("GetUserById", new { id = user.Id },
                    new
                    {
                        user = userDto,
                        Message = "User Created Successfully"
                    });
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error occurred while creating user.");
                return StatusCode(500, new { Message = "Database Error" });
            }

            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while creating user.");
                return StatusCode(500, new { Message = "Internal Server Error" });
            }
        }

        [HttpPut("{id}")]

        public async Task<IActionResult> UpdateUserById(
            [FromBody] UpdateUserDto updateUser,
            [FromRoute] int id,
            CancellationToken cancellationToken = default)
        {

            if (id <= 0)
            {
                return BadRequest(new { Message = "Invalid Id!" });
            }

            try
            {

                var existingUser = await _context.Users.FindAsync(id, cancellationToken);

                if (existingUser == null)
                {
                    return NotFound(new { Message = "User Not Found" });
                }

                existingUser.Name = updateUser.Name;
                existingUser.Email = updateUser.Email;
                existingUser.RoleId = updateUser.RoleId;
                existingUser.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync(cancellationToken);

                var userDto = new UserDto
                {
                    Id = existingUser.Id,
                    Name = existingUser.Name,
                    Email = existingUser.Email,
                    RoleId = existingUser.RoleId,
                    CreatedAt = existingUser.CreatedAt,
                    UpdatedAt = DateTime.UtcNow

                };

                return Ok(userDto);

            }
            catch (DbUpdateException e)
            {

                _logger.LogError(e, "Error Updating user ID = {UserId}", id);

                return StatusCode(500, "Error Saving Changes");

            }

            catch (Exception e)
            {
                _logger.LogError(e, "Error Updating User");
                return StatusCode(500);
            }
        }

        [HttpPost("Login")]

        [AllowAnonymous]
        public async Task<IActionResult> LoginUser([FromBody] UserLoginDto userLogin 
            , CancellationToken cancellationToken = default)
        {
            try
            {

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Name == userLogin.Identifier
                || u.Email == userLogin.Identifier , cancellationToken);

                if(user == null)
                {
                    return NotFound(new { Message = "User Not Found" });
                }

                //if user is matched we compare the password

                if(!_passwordHasher.ComparePassword(userLogin.Password , user.PasswordHash))
                {
                    return Unauthorized(new { Message = "Password Does Not Match"});
                }

                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name , user.Name),
                    new Claim(ClaimTypes.Email, user.Email),
                };

                var key = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(1),
                    signingCredentials: creds
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                return Ok(new {Message = "Login Successful", Token = tokenString});

            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error During User Login");
                return StatusCode(500);
            }

        }

        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteUserById([FromRoute] int id,
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { Message = "Invalid ID" });
                }

                var userToDelete = await _context.Users.FindAsync(id, cancellationToken);

                if (userToDelete == null)
                {
                    return NotFound(new { Message = "User Not Found" });
                }

                _context.Users.Remove(userToDelete);
                await _context.SaveChangesAsync(cancellationToken);

                return NoContent();

            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error deleting user {UserId}", id);
                return StatusCode(500, new
                {
                    Message = "Failed to delete user due to database error",
                    Details = ex.InnerException?.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error deleting user {UserId}", id);
                return StatusCode(500, new
                {
                    Message = "An unexpected error occurred while deleting user",
                    Details = ex.Message
                });

            }

        }
    }
}
