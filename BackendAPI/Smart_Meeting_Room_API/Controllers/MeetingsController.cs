using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smart_Meeting_Room_API.Dtos.Meetings;
using Smart_Meeting_Room_API.Models;

namespace Smart_Meeting_Room_API.Controllers
{

    [ApiController]
    [Route("[Controller]")]
    //[Authorize]

    public class MeetingsController : ControllerBase
    {
        private readonly SmartMeetingRoomDbContext _context;
        private readonly ILogger<MeetingsController> _logger;

        public MeetingsController(SmartMeetingRoomDbContext context
            , ILogger<MeetingsController> logger)
        {
            _context = context;
            _logger = logger;

        }

        [HttpGet]

        public async Task<IActionResult> GetMeetings(CancellationToken cancellationToken = default)
        {
            try
            {

                var meetings = await _context.Meetings.Select(m => new Meeting
                {
                    Id = m.Id,
                    RoomId = m.RoomId,
                    Title = m.Title,
                    Agenda = m.Agenda,
                    StartTime = m.StartTime,
                    EndTime = m.EndTime,
                    CreatedBy = m.CreatedBy,
                    CreatedAt = m.CreatedAt,
                    UpdatedAt = m.UpdatedAt,
                    Status = m.Status

                }).ToListAsync(cancellationToken);

                return Ok(new
                {
                    meetings,
                    Message = "Meetings Retrieved Successfully"
                });


            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error retrieving meetings");
                return StatusCode(500);

            }
        }

        [HttpGet("{id}")]

        public async Task<IActionResult> GetMeetingById([FromRoute] int id
            , CancellationToken cancellationToken = default)
        {
            try
            {

                if(id <= 0)
                {
                    return BadRequest("Invalid Id");
                }

                var meetingToRetrieve = await _context.Meetings
                    .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

                if (meetingToRetrieve == null)
                {
                    return NotFound("Meeting with id [" + id + " ] Not Found");
                }

                return Ok(new
                {
                    meetingToRetrieve,
                    Message = "Meeting Retrieved Successfully"

                });


            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting meeting with id [" + id + " ]");
                return StatusCode(500);
            }
        }

        [HttpPost]

        public async Task<IActionResult> CreateMeeting(
            [FromBody] CreateMeetingDto Meeting 
            , CancellationToken cancellationToken = default)
        {
            try
            {

                var newMeeting = new Meeting
                {
                    RoomId = Meeting.RoomId,
                    Title = Meeting.Title,
                    Agenda = Meeting.Agenda,
                    StartTime = Meeting.StartTime,
                    EndTime = Meeting.EndTime,
                    CreatedBy = Meeting.CreatedBy,
                    Status = Meeting.Status,
                };

                _context.Meetings.Add(newMeeting);
                await _context.SaveChangesAsync(cancellationToken);

                return Ok(new
                {
                    newMeeting,
                    Message = "Meeting Created with Title [" + newMeeting.Title + "]"
                });

            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating new meeting");
                return StatusCode(500);
            }
        }

        [HttpPut("{id}")]

        public async Task<IActionResult> UpdateMeetingById([FromRoute] int id
            , [FromBody] UpdateMeetingDto Meeting , CancellationToken cancellationToken = default)
        {
            try
            {

                if(id <= 0)
                {
                    return BadRequest("Invalid Id");
                }

                var meetingToUpdate = await _context.Meetings
                    .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

                if (meetingToUpdate == null)
                {
                    return NotFound("Meeting Not Found");
                }

                meetingToUpdate.RoomId = Meeting.RoomId;
                meetingToUpdate.Title = Meeting.Title;
                meetingToUpdate.Agenda = Meeting.Agenda;
                meetingToUpdate.StartTime = Meeting.StartTime;
                meetingToUpdate.EndTime = Meeting.EndTime;
                meetingToUpdate.CreatedBy = Meeting.CreatedBy;
                meetingToUpdate.Status = Meeting.Status;
                meetingToUpdate.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync(cancellationToken);

                return Ok(new
                {
                    meetingToUpdate,
                    Message = "Meeting [" + meetingToUpdate.Title + "] Updated Successfully"
                });

            }
            catch(DbUpdateException db_uE)
            {
                _logger.LogError(db_uE, "Error Updating Meeting with ID [" + id + "]");
                return StatusCode(500);
            }
            catch(Exception e)
            {
                _logger.LogError(e, "Error Updating Meeting");
                return StatusCode(500);
            }
        }


        [HttpGet("{meetingId}/attendees")]
        public async Task<IActionResult> GetMeetingAttendees(
        [FromRoute(Name = "meetingId")] int meetingId,
        CancellationToken cancellationToken = default)
        {
            try
            {
                if (meetingId <= 0)
                {
                    return BadRequest();
                }

                var meeting = await _context.Meetings
                    .Include(m => m.Users)
                    .FirstOrDefaultAsync(m => m.Id == meetingId, cancellationToken);

                if (meeting == null)
                {
                    return NotFound();
                }

                return Ok(new
                {
                    attendees = meeting.Users.Select(user => new
                    {
                        id = user.Id,
                        name = user.Name,
                        email = user.Email
                    }).ToList()
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error retrieving attendees for meeting with id [" + meetingId + "]");
                return StatusCode(500);
            }
        }


        [HttpGet("{meetingId}/action-items")]
        public async Task<IActionResult> GetMeetingActionItems(
            [FromRoute(Name = "meetingId")] int meetingId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (meetingId <= 0)
                {
                    return BadRequest("Invalid Meeting Id");
                }

                var meeting = await _context.Meetings
                    .FirstOrDefaultAsync(m => m.Id == meetingId, cancellationToken);

                if (meeting == null)
                {
                    return NotFound("Meeting Not Found");
                }


                var actionItems = await _context.ActionItems
                    .Include(ai => ai.Mom) 
                    .Where(ai => ai.Mom.MeetingId == meetingId) 
                    .Select(ai => new
                    {
                        id = ai.Id,
                        description = ai.Description,
                        assignedTo = ai.AssignedTo,
                        dueDate = ai.DueDate,
                        status = ai.Status,
                        createdAt = ai.CreatedAt,
                        momId = ai.MomId 
                    })
                    .ToListAsync(cancellationToken);

                return Ok(new
                {
                    meetingId,
                    meetingTitle = meeting.Title,
                    actionItems
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving action items for meeting with id [" + meetingId + "]");
                return StatusCode(500);
            }
        }


        [HttpPost("{meetingId}/attendees")]
        public async Task<IActionResult> AddAttendeeToMeeting(
        [FromRoute(Name = "meetingId")] int meetingId,
        [FromBody] AddAttendeeDto attendee,
        CancellationToken cancellationToken = default)
        {
            try
            {
                if (meetingId <= 0)
                {
                    return BadRequest("Invalid Meeting Id");
                }

                var meeting = await _context.Meetings
                    .Include(m => m.Users)
                    .FirstOrDefaultAsync(m => m.Id == meetingId, cancellationToken);

                if (meeting == null)
                {
                    return NotFound("Meeting Not Found");
                }

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == attendee.UserId, cancellationToken);

                if (user == null)
                {
                    return NotFound("User Not Found");
                }

                if (meeting.Users.Any(u => u.Id == attendee.UserId))
                {
                    return BadRequest("User is already an attendee");
                }

                meeting.Users.Add(user);
                await _context.SaveChangesAsync(cancellationToken);

                return Ok(new
                {
                    Message = "Attendee added successfully to meeting [" + meeting.Title + "]"
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error adding attendee to meeting with id [" + meetingId + "]");
                return StatusCode(500);
            }
        }


        [HttpPost("{meetingId}/cancel")]
        public async Task<IActionResult> CancelMeeting(
        [FromRoute(Name = "meetingId")] int meetingId,
        CancellationToken cancellationToken = default)
        {
            try
            {
                if (meetingId <= 0)
                {
                    return BadRequest("Invalid Meeting Id");
                }

                var meeting = await _context.Meetings
                    .FirstOrDefaultAsync(m => m.Id == meetingId, cancellationToken);

                if (meeting == null)
                {
                    return NotFound("Meeting Not Found");
                }

                if (meeting.Status == "Cancelled")
                {
                    return BadRequest("Meeting is already cancelled");
                }

                meeting.Status = "Cancelled";
                meeting.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync(cancellationToken);

                return Ok(new
                {
                    Message = "Meeting [" + meeting.Title + "] has been cancelled successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling meeting with id [" + meetingId + "]");
                return StatusCode(500);
            }
        }


        [HttpDelete("{meetingId}/attendees/{userId}")]
        public async Task<IActionResult> RemoveAttendeeFromMeeting(
        [FromRoute(Name = "meetingId")] int meetingId,
        [FromRoute(Name = "userId")] int userId,
        CancellationToken cancellationToken = default)
        {
            try
            {
                if (meetingId <= 0 || userId <= 0)
                {
                    return BadRequest("Invalid Id");
                }

                var meeting = await _context.Meetings
                    .Include(m => m.Users)
                    .FirstOrDefaultAsync(m => m.Id == meetingId, cancellationToken);

                if (meeting == null)
                {
                    return NotFound("Meeting Not Found");
                }

                var attendee = meeting.Users.FirstOrDefault(u => u.Id == userId);

                if (attendee == null)
                {
                    return NotFound("Attendee Not Found");
                }

                meeting.Users.Remove(attendee);
                await _context.SaveChangesAsync(cancellationToken);

                return Ok(new
                {
                    Message = "Attendee removed successfully from meeting [" + meeting.Title + "]"
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error removing attendee from meeting with id [" + meetingId + "]");
                return StatusCode(500);
            }
        }



        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteMeetingById([FromRoute] int id
            , CancellationToken cancellationToken = default)
        {
            try
            {

                if(id <= 0)
                {
                    return BadRequest();
                }

                var meetingToDelete = await _context.Meetings.FirstOrDefaultAsync
                    (m => m.Id == id, cancellationToken);

                if(meetingToDelete == null)
                {
                    return NotFound("Meeting Not Found");
                }

                _context.Meetings.Remove(meetingToDelete);
                await _context.SaveChangesAsync(cancellationToken);

                return NoContent();

            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error Deleting Meeting with ID [" + id + "]");
                return StatusCode(500);

            }
        }

    }
}
