using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Smart_Meeting_Room_API.Models;
using Smart_Meeting_Room_API.Dtos.ActionItems;

namespace Smart_Meeting_Room_API.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    //[Authorize]

    public class ActionItemsController : ControllerBase
    {

        private readonly SmartMeetingRoomDbContext _context;
        private readonly ILogger<ActionItemsController> _logger;

        public ActionItemsController(SmartMeetingRoomDbContext context, ILogger<ActionItemsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]

        public async Task<IActionResult> GetActionItems(CancellationToken cancellationToken = default)
        {
            try
            {

                var actionItems = await _context.ActionItems.Select(ai => new ActionItem
                {
                    Id = ai.Id,
                    Description = ai.Description,
                    Status = ai.Status,
                    DueDate = ai.DueDate,
                    AssignedTo = ai.AssignedTo,
                    MomId = ai.MomId,
                    CreatedAt = ai.CreatedAt,
                    UpdatedAt = ai.UpdatedAt

                }).ToListAsync(cancellationToken);

                return Ok(actionItems);

            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error retrieving ActionItems");
                return StatusCode(500);
            }
        }

        [HttpGet("{id}")]

        public async Task<IActionResult> GetActionItemById([FromRoute] int id,
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest();
                }

                var actionItemToReturn = await _context.ActionItems.FirstOrDefaultAsync(
                    ai => ai.Id == id, cancellationToken);

                if (actionItemToReturn == null)
                {
                    return NotFound();
                }

                return Ok(actionItemToReturn);

            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error retrieving actionItem with id [" + id + "]");
                return StatusCode(500);

            }
        }


        [HttpPost]

        public async Task<IActionResult> CreateActionItem([FromBody] CreateActionItemDto ActionItem
            , CancellationToken cancellationToken = default)
        {
            try
            {


                var actionItemToCreate = new ActionItem
                {
                    Description = ActionItem.Description,
                    Status = ActionItem.Status,
                    DueDate = ActionItem.DueDate,
                    AssignedTo = ActionItem.AssignedTo,
                    MomId = ActionItem.MomId,
                    CreatedAt = DateTime.UtcNow,

                };

                _context.ActionItems.Add(actionItemToCreate);
                await _context.SaveChangesAsync();

                return CreatedAtAction(
                    nameof(GetActionItemById),
                    new { id = actionItemToCreate.Id },
                    actionItemToCreate

                    );

            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating actionItem");
                return StatusCode(500);
            }
        }

        [HttpPut("{id}")]

        public async Task<IActionResult> UpdateActionItemById([FromRoute] int id,
            [FromBody] UpdateActionItemDto ActionItem, CancellationToken cancellationToken = default)
        {
            try
            {

                if (id <= 0)
                {
                    return BadRequest();
                }

                var actionItemToUpdate = await _context.ActionItems.FirstOrDefaultAsync(ai => ai.Id == id
                , cancellationToken);

                if (actionItemToUpdate == null)
                {
                    return NotFound();
                }

                actionItemToUpdate.Description = ActionItem.Description;
                actionItemToUpdate.Status = ActionItem.Status;
                actionItemToUpdate.DueDate = ActionItem.DueDate;
                actionItemToUpdate.AssignedTo = ActionItem.AssignedTo;
                actionItemToUpdate.MomId = ActionItem.MomId;
                actionItemToUpdate.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(actionItemToUpdate);

            }
            catch (DbUpdateException db_uE)
            {
                _logger.LogError(db_uE, "Error updating actionItem with ID [" + id + "]");
                return StatusCode(500);
            }
        }

        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteActionItemById([FromRoute] int id,
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest();
                }

                var actionItemToDelete = await _context.ActionItems.FindAsync(id, cancellationToken);

                if (actionItemToDelete == null)
                {
                    return NotFound();
                }

                _context.ActionItems.Remove(actionItemToDelete);
                await _context.SaveChangesAsync(cancellationToken);

                return NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error Deleting actionItem with ID [" + id + "]");
                return StatusCode(500);
            }
        }


    }
}