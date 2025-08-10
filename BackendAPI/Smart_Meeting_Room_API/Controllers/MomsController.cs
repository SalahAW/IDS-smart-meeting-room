using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Smart_Meeting_Room_API.Models;
using Smart_Meeting_Room_API.Dtos.Moms;

namespace Smart_Meeting_Room_API.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    //[Authorize]

    public class MomsController : ControllerBase
    {

        private readonly SmartMeetingRoomDbContext _context;
        private readonly ILogger<MomsController> _logger;

        public MomsController(SmartMeetingRoomDbContext context, ILogger<MomsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]

        public async Task<IActionResult> GetMoms(CancellationToken cancellationToken = default)
        {
            try
            {

                var moms = await _context.Moms.Select(m => new Mom
                {
                    Id = m.Id,
                    Notes = m.Notes,
                    Decisions = m.Decisions,
                    MeetingId = m.MeetingId,
                    CreatedAt = m.CreatedAt,
                    UpdatedAt = m.UpdatedAt

                }).ToListAsync(cancellationToken);

                return Ok(moms);

            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error retrieving Moms");
                return StatusCode(500);
            }
        }

        [HttpGet("{id}")]

        public async Task<IActionResult> GetMomById([FromRoute] int id,
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest();
                }

                var momToReturn = await _context.Moms.FirstOrDefaultAsync(
                    m => m.Id == id, cancellationToken);

                if (momToReturn == null)
                {
                    return NotFound();
                }

                return Ok(momToReturn);

            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error retrieving mom with id [" + id + "]");
                return StatusCode(500);

            }
        }


        [HttpPost]

        public async Task<IActionResult> CreateMom([FromBody] CreateMomDto Mom
            , CancellationToken cancellationToken = default)
        {
            try
            {


                var momToCreate = new Mom
                {
                    Notes = Mom.Notes,
                    Decisions = Mom.Decisions,
                    MeetingId = Mom.MeetingId,
                    CreatedAt = DateTime.UtcNow,

                };

                _context.Moms.Add(momToCreate);
                await _context.SaveChangesAsync();

                return CreatedAtAction(
                    nameof(GetMomById),
                    new { id = momToCreate.Id },
                    momToCreate

                    );

            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating mom");
                return StatusCode(500);
            }
        }

        [HttpPut("{id}")]

        public async Task<IActionResult> UpdateMomById([FromRoute] int id,
            [FromBody] UpdateMomDto Mom, CancellationToken cancellationToken = default)
        {
            try
            {

                if (id <= 0)
                {
                    return BadRequest();
                }

                var momToUpdate = await _context.Moms.FirstOrDefaultAsync(m => m.Id == id
                , cancellationToken);

                if (momToUpdate == null)
                {
                    return NotFound();
                }

                momToUpdate.Notes = Mom.Notes;
                momToUpdate.Decisions = Mom.Decisions;
                momToUpdate.MeetingId = Mom.MeetingId;
                momToUpdate.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(momToUpdate);

            }
            catch (DbUpdateException db_uE)
            {
                _logger.LogError(db_uE, "Error updating mom with ID [" + id + "]");
                return StatusCode(500);
            }
        }

        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteMomById([FromRoute] int id,
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest();
                }

                var momToDelete = await _context.Moms.FindAsync(id, cancellationToken);

                if (momToDelete == null)
                {
                    return NotFound();
                }

                _context.Moms.Remove(momToDelete);
                await _context.SaveChangesAsync(cancellationToken);

                return NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error Deleting mom with ID [" + id + "]");
                return StatusCode(500);
            }
        }


    }
}