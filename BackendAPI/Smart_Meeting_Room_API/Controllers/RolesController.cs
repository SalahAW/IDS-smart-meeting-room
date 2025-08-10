using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smart_Meeting_Room_API.Dtos.Roles;
using Smart_Meeting_Room_API.Models;

namespace Smart_Meeting_Room_API.Controllers
{

    [ApiController]
    [Route("[Controller]")]
    //[Authorize]
    public class RolesController : ControllerBase
    {
        private readonly SmartMeetingRoomDbContext _context;
        private readonly ILogger<RolesController> _logger;

        public RolesController(SmartMeetingRoomDbContext context
            , ILogger<RolesController> logger)
        {
            _context = context;
            _logger = logger;
        }


        [HttpGet]
        public async Task<IActionResult> GetRoles(CancellationToken cancellationToken = default)
        {

            try
            {
                var roles = await _context.Roles.Select(r => new RoleDto
                {
                    Id = r.Id,
                    RoleName = r.RoleName,
                    CreatedAt = r.CreatedAt,
                    UpdatedAt = r.UpdatedAt

                }).ToListAsync(cancellationToken);

                return Ok(new
                {
                    roles,
                    Message = "Roles retrieved"
                });

            }
            catch(Exception e)
            {
                _logger.LogError(e, "Error occured while retrieving roles");
                return StatusCode(500);
            }

        }


        [HttpGet("{id}")]

        public async Task<ActionResult> GetRoleById([FromRoute] int id,
            CancellationToken cancellationToken = default)
        {

            try
            {

                if(id <= 0)
                {
                    return BadRequest("Invalid Id");
                }

                var role = await _context.Roles.FindAsync(id , cancellationToken);

                if(role == null)
                {
                    return NotFound(new { Message = "Role With {RoleId} not found" });
                }

                return Ok(role);

            }
            catch(Exception e)
            {
                _logger.LogError(e, "Error getting role by id");
                return StatusCode(500);
            }

        }

        [HttpPost]

        public async Task<IActionResult> CreateRole([FromBody] CreateRoleDto Role
            , CancellationToken cancellationToken = default)
        {

            try
            {

                var new_role = new Role()
                {
                    RoleName = Role.RoleName,
                    CreatedAt = DateTime.UtcNow,
                };

                _context.Roles.Add(new_role);
                await _context.SaveChangesAsync(cancellationToken);


                return Ok(new {
                    new_role,
                    Message = "Role [" + Role.RoleName + "] Created Successfully"
                });

            }
            catch(Exception e)
            {
                _logger.LogError(e, "Error Creating Role");
                return StatusCode(500);
            }

        }

        [HttpPut("{id}")]

        public async Task<IActionResult> UpdateRoleById([FromRoute] int id ,
            [FromBody] UpdateRoleDto Role , CancellationToken cancellationToken = default)
        {
            try
            {

                if(id <= 0)
                {
                    return BadRequest("Invalid Id");
                }

                var role = await _context.Roles.FindAsync(id , cancellationToken);

                if(role == null)
                {
                    return NotFound("Role with id " + id + " not found");
                }

                role.RoleName = Role.RoleName;
                role.UpdatedAt = DateTime.UtcNow;

               await _context.SaveChangesAsync(cancellationToken);

                return Ok(role);

            }
            catch(DbUpdateException db_uE)
            {
                _logger.LogError(db_uE, "Database Update Error Occured");
                return StatusCode(500);
            }
            catch(Exception e)
            {
                _logger.LogError(e, "Error Updating Role");
                return StatusCode(500);
            }
        }

        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteRoleById([FromRoute] int id
            , CancellationToken cancellationToken = default)
        {

            try
            {

                if(id <= 0)
                {
                    return BadRequest("Invalid Id");
                }

                var roleToDelete = await _context.Roles.FindAsync(id , cancellationToken);

                if(roleToDelete == null)
                {
                    return NotFound("Role Not Found");
                }

                _context.Roles.Remove(roleToDelete);
                await _context.SaveChangesAsync(cancellationToken);


                return NoContent();

            }
            catch(Exception e)
            {
                _logger.LogError(e, "Error occured while deleting role with id [" + id + " ]");
                return StatusCode(500);
            }

        }

    }
}
