using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smart_Meeting_Room_API.Dtos.Rooms;
using Smart_Meeting_Room_API.Models;

namespace Smart_Meeting_Room_API.Controllers
{

    [ApiController]
    [Route("[Controller]")]
    //[Authorize]

    public class RoomsController : ControllerBase
    {

        private readonly SmartMeetingRoomDbContext _context;
        private readonly ILogger<RoomsController> _logger;
       
        public RoomsController(SmartMeetingRoomDbContext context
            , ILogger<RoomsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]

        public async Task<IActionResult> GetRooms(CancellationToken cancellationToken = default)
        {
            try
            {

                var rooms = await _context.Rooms.Select(rm => new RoomDto
                {
                    Id = rm.Id,
                    Name = rm.Name,
                    Location = rm.Location,
                    Capacity = rm.Capacity,
                    CreatedAt = rm.CreatedAt,
                    UpdatedAt = rm.UpdatedAt

                }).ToListAsync(cancellationToken);

                return Ok(new
                {
                    rooms,
                    Message = "Rooms retrieved"
                });

            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error Retrieving Rooms");
                return StatusCode(500);
            }
        }

        [HttpGet("{id}")]

        public async Task<IActionResult> GetRoomById([FromRoute] int id
            , CancellationToken cancellationToken = default)
        {
            try
            {

                if(id <= 0)
                {
                    return BadRequest("Invalid Id");

                }

                var room = await _context.Rooms
                    .Include(rm => rm.Meetings)
                    .Include(rm => rm.Features)
                    .FirstOrDefaultAsync(rm => rm.Id == id, cancellationToken);

                if(room == null)
                {
                    return NotFound("Room Not Found");
                }

                return Ok(new
                {
                    room,
                    Message = "Room Retrieved"
                });

            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error Retreiving room with id " + id);
                return StatusCode(500);
            }
        }

        [HttpGet("{roomId}/meetings")]
        public async Task<IActionResult> GetRoomMeetings(
    [FromRoute(Name = "roomId")] int roomId,
    CancellationToken cancellationToken = default)
        {
            try
            {
                if (roomId <= 0)
                {
                    return BadRequest("Invalid Room Id");
                }

                var room = await _context.Rooms
                    .Include(r => r.Meetings)
                    .FirstOrDefaultAsync(r => r.Id == roomId, cancellationToken);

                if (room == null)
                {
                    return NotFound("Room Not Found");
                }

                return Ok(new
                {
                    roomId = room.Id,
                    roomName = room.Name,
                    meetings = room.Meetings.Select(m => new
                    {
                        id = m.Id,
                        title = m.Title,
                        agenda = m.Agenda,
                        startTime = m.StartTime,
                        endTime = m.EndTime,
                        status = m.Status,
                        createdBy = m.CreatedBy
                    }).ToList()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving meetings for room with id [" + roomId + "]");
                return StatusCode(500);
            }
        }


        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableRooms(
    [FromQuery] DateTime startTime,
    [FromQuery] DateTime endTime,
    [FromQuery] int? capacity = null,
    CancellationToken cancellationToken = default)
        {
            try
            {
                if (startTime >= endTime)
                {
                    return BadRequest("Start time must be before end time");
                }

                var busyRoomIds = await _context.Meetings
                    .Where(m => m.Status != "Cancelled" &&
                               ((m.StartTime < endTime && m.EndTime > startTime)))
                    .Select(m => m.RoomId)
                    .Distinct()
                    .ToListAsync(cancellationToken);

                var availableRoomsQuery = _context.Rooms
                    .Where(r => !busyRoomIds.Contains(r.Id));

                if (capacity.HasValue)
                {
                    availableRoomsQuery = availableRoomsQuery.Where(r => r.Capacity >= capacity.Value);
                }

                var availableRooms = await availableRoomsQuery
                    .Select(r => new
                    {
                        id = r.Id,
                        name = r.Name,
                        location = r.Location,
                        capacity = r.Capacity
                    })
                    .ToListAsync(cancellationToken);

                return Ok(new
                {
                    requestedTimeSlot = new { startTime, endTime },
                    minimumCapacity = capacity,
                    availableRooms
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving available rooms");
                return StatusCode(500);
            }
        }



        [HttpPost]

        public async Task<IActionResult> CreateRoom([FromBody] CreateRoomDto Room
            , CancellationToken cancellationToken = default)
        {
            try
            {

                var room = new Room()
                {
                    Name = Room.Name,
                    Location = Room.Location,
                    Capacity = Room.Capacity,
                    CreatedAt = DateTime.UtcNow,
                };

                _context.Rooms.Add(room);
                await _context.SaveChangesAsync(cancellationToken);

                var roomToReturn = new RoomDto()
                {
                    Id = room.Id,
                    Name = room.Name,
                    Location = room.Location,
                    Capacity = room.Capacity,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = room.UpdatedAt

                };

                return Ok(new
                {
                    roomToReturn,
                    Message = "Room Created Successfully"
                });

            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error Creating Room");
                return StatusCode(500);
            }
        }

        [HttpPut("{id}")]

        public async Task<IActionResult> UpdateRoomById([FromRoute] int id
            , [FromBody] UpdateRoomDto updateRoom
            , CancellationToken cancellationToken = default)
        {
            try
            {
                if(id <= 0)
                {
                    return BadRequest("Invalid Id");
                }

                var roomToUpdate = await _context.Rooms.FirstOrDefaultAsync(rm => rm.Id == id, cancellationToken);

                if(roomToUpdate == null)
                {
                    return NotFound("Room Not Found");
                }

                roomToUpdate.Name = updateRoom.Name;
                roomToUpdate.Location = updateRoom.Location;
                roomToUpdate.Capacity = updateRoom.Capacity;
                roomToUpdate.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync(cancellationToken);

                return Ok(new
                {
                    roomToUpdate,
                    Message = "Room Updated Successfully"
                });

            }
            catch(DbUpdateException db_uE)
            {
                _logger.LogError(db_uE, "Error Updating Room with id [" + id + " ]");
                return StatusCode(500);
            }
        }


        

        [HttpGet("{id}/features")]
        public async Task<IActionResult> GetRoomFeatures([FromRoute] int id,
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid Id");
                }

                var room = await _context.Rooms
                    .Include(rm => rm.Features)
                    .FirstOrDefaultAsync(rm => rm.Id == id, cancellationToken);

                if (room == null)
                {
                    return NotFound("Room Not Found");
                }

                return Ok(new
                {
                    features = room.Features,
                    Message = "Room features retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving features for room with id [" + id + "]");
                return StatusCode(500);
            }
        }

        [HttpPost("{id}/features")]
        public async Task<IActionResult> AddFeatureToRoom([FromRoute] int id,
            [FromBody] AddRoomFeatureDto roomFeature, CancellationToken cancellationToken = default)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid Room Id");
                }

                var room = await _context.Rooms
                    .Include(rm => rm.Features)
                    .FirstOrDefaultAsync(rm => rm.Id == id, cancellationToken);

                if (room == null)
                {
                    return NotFound("Room Not Found");
                }

                var feature = await _context.Features.FirstOrDefaultAsync(f => f.Id == roomFeature.FeatureId, cancellationToken);

                if (feature == null)
                {
                    return NotFound("Feature Not Found");
                }

                if (room.Features.Any(f => f.Id == roomFeature.FeatureId))
                {
                    return BadRequest("Feature is already assigned to this room");
                }

                room.Features.Add(feature);
                await _context.SaveChangesAsync(cancellationToken);

                return Ok(new
                {
                    Message = "Feature [" + feature.FeatureName + "] added successfully to room [" + room.Name + "]"
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error adding feature to room with id [" + id + "]");
                return StatusCode(500);
            }
        }

        [HttpDelete("{id}/features/{featureId}")]
        public async Task<IActionResult> RemoveFeatureFromRoom([FromRoute] int id,
            [FromRoute] int featureId, CancellationToken cancellationToken = default)
        {
            try
            {
                if (id <= 0 || featureId <= 0)
                {
                    return BadRequest("Invalid Id");
                }

                var room = await _context.Rooms
                    .Include(rm => rm.Features)
                    .FirstOrDefaultAsync(rm => rm.Id == id, cancellationToken);

                if (room == null)
                {
                    return NotFound("Room Not Found");
                }

                var feature = room.Features.FirstOrDefault(f => f.Id == featureId);

                if (feature == null)
                {
                    return NotFound("Feature Not Found in this room");
                }

                room.Features.Remove(feature);
                await _context.SaveChangesAsync(cancellationToken);

                return Ok(new
                {
                    Message = "Feature [" + feature.FeatureName + "] removed successfully from room [" + room.Name + "]"
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error removing feature from room with id [" + id + "]");
                return StatusCode(500);
            }
        }



        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteRoomById([FromRoute] int id
            , CancellationToken cancellationToken = default)
        {
            try
            {

                if(id <= 0)
                {
                    return BadRequest("Invalid Id");
                }

                var roomToDelete = await _context.Rooms.FindAsync(id , cancellationToken);

                if(roomToDelete == null)
                {
                    return NotFound("Room not found");
                }

                _context.Rooms.Remove(roomToDelete);
                await _context.SaveChangesAsync(cancellationToken);

                return NoContent();

            }
            catch (Exception e)
            {

                _logger.LogError(e, "Error deleting room with id [" + id + " ]");
                return StatusCode(500);
            }
        }

    }
}
