using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Smart_Meeting_Room_API.Models;
using Smart_Meeting_Room_API.Dtos.Attachments;

namespace Smart_Meeting_Room_API.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    //[Authorize]

    public class AttachmentsController : ControllerBase
    {

        private readonly SmartMeetingRoomDbContext _context;
        private readonly ILogger<AttachmentsController> _logger;

        public AttachmentsController(SmartMeetingRoomDbContext context, ILogger<AttachmentsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]

        public async Task<IActionResult> GetAttachments(CancellationToken cancellationToken = default)
        {
            try
            {

                var attachments = await _context.Attachments.Select(att => new Attachment
                {
                    Id = att.Id,
                    FileName = att.FileName,
                    FilePath = att.FilePath,
                    MomId = att.MomId,
                    CreatedAt = att.CreatedAt,
                    UpdatedAt = att.UpdatedAt

                }).ToListAsync(cancellationToken);

                return Ok(attachments);

            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error retrieving Attachments");
                return StatusCode(500);
            }
        }

        [HttpGet("{id}")]

        public async Task<IActionResult> GetAttachmentById([FromRoute] int id,
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest();
                }

                var attachmentToReturn = await _context.Attachments.FirstOrDefaultAsync(
                    att => att.Id == id, cancellationToken);

                if (attachmentToReturn == null)
                {
                    return NotFound();
                }

                return Ok(attachmentToReturn);

            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error retrieving attachment with id [" + id + "]");
                return StatusCode(500);

            }
        }


        [HttpPost]

        public async Task<IActionResult> CreateAttachment([FromBody] CreateAttachmentDto Attachment
            , CancellationToken cancellationToken = default)
        {
            try
            {


                var attachmentToCreate = new Attachment
                {
                    FileName = Attachment.FileName,
                    FilePath = Attachment.FilePath,
                    MomId = Attachment.MomId,
                    CreatedAt = DateTime.UtcNow,

                };

                _context.Attachments.Add(attachmentToCreate);
                await _context.SaveChangesAsync();

                return CreatedAtAction(
                    nameof(GetAttachmentById),
                    new { id = attachmentToCreate.Id },
                    attachmentToCreate

                    );

            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating attachment");
                return StatusCode(500);
            }
        }

        [HttpPut("{id}")]

        public async Task<IActionResult> UpdateAttachmentById([FromRoute] int id,
            [FromBody] UpdateAttachmentDto Attachment, CancellationToken cancellationToken = default)
        {
            try
            {

                if (id <= 0)
                {
                    return BadRequest();
                }

                var attachmentToUpdate = await _context.Attachments.FirstOrDefaultAsync(att => att.Id == id
                , cancellationToken);

                if (attachmentToUpdate == null)
                {
                    return NotFound();
                }

                attachmentToUpdate.FileName = Attachment.FileName;
                attachmentToUpdate.FilePath = Attachment.FilePath;
                attachmentToUpdate.MomId = Attachment.MomId;
                attachmentToUpdate.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(attachmentToUpdate);

            }
            catch (DbUpdateException db_uE)
            {
                _logger.LogError(db_uE, "Error updating attachment with ID [" + id + "]");
                return StatusCode(500);
            }
        }

        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteAttachmentById([FromRoute] int id,
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest();
                }

                var attachmentToDelete = await _context.Attachments.FindAsync(id, cancellationToken);

                if (attachmentToDelete == null)
                {
                    return NotFound();
                }

                _context.Attachments.Remove(attachmentToDelete);
                await _context.SaveChangesAsync(cancellationToken);

                return NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error Deleting attachment with ID [" + id + "]");
                return StatusCode(500);
            }
        }


    }
}