using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Smart_Meeting_Room_API.Models;
using Smart_Meeting_Room_API.Dtos.Features;

namespace Smart_Meeting_Room_API.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    //[Authorize]

    public class FeaturesController : ControllerBase
    {

        private readonly SmartMeetingRoomDbContext _context;
        private readonly ILogger<FeaturesController> _logger;

        public FeaturesController(SmartMeetingRoomDbContext context, ILogger<FeaturesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]

        public async Task<IActionResult> GetFeatures(CancellationToken cancellationToken = default)
        {
            try
            {

                var features = await _context.Features.Select(ft => new Feature
                {
                    Id = ft.Id,
                    FeatureName = ft.FeatureName,
                    CreatedAt = ft.CreatedAt,
                    UpdatedAt = ft.UpdatedAt

                }).ToListAsync(cancellationToken);

                return Ok(features);

            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error retrieving Features");
                return StatusCode(500);
            }
        }

        [HttpGet("{id}")]

        public async Task<IActionResult> GetFeatureById([FromRoute] int id,
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest();
                }

                var featureToReturn = await _context.Features.FirstOrDefaultAsync(
                    ft => ft.Id == id, cancellationToken);

                if (featureToReturn == null)
                {
                    return NotFound();
                }

                return Ok(featureToReturn);

            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error retrieving feature with id [" + id + "]");
                return StatusCode(500);

            }
        }


        [HttpPost]

        public async Task<IActionResult> CreateFeature([FromBody] CreateFeatureDto Feature
            , CancellationToken cancellationToken = default)
        {
            try
            {


                var featureToCreate = new Feature
                {
                    FeatureName = Feature.FeatureName,
                    CreatedAt = DateTime.UtcNow,

                };

                _context.Features.Add(featureToCreate);
                await _context.SaveChangesAsync(cancellationToken);

                return CreatedAtAction(
                    nameof(GetFeatureById),
                    new { id = featureToCreate.Id },
                    featureToCreate

                    );

            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating feature");
                return StatusCode(500);
            }
        }

        [HttpPut("{id}")]

        public async Task<IActionResult> UpdateFeatureById([FromRoute] int id,
            [FromBody] UpdateFeatureDto Feature, CancellationToken cancellationToken = default)
        {
            try
            {

                if (id <= 0)
                {
                    return BadRequest();
                }

                var featureToUpdate = await _context.Features.FirstOrDefaultAsync(ft => ft.Id == id
                , cancellationToken);

                if (featureToUpdate == null)
                {
                    return NotFound();
                }

                featureToUpdate.FeatureName = Feature.FeatureName;
                featureToUpdate.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync(cancellationToken);

                return Ok(featureToUpdate);

            }
            catch (DbUpdateException db_uE)
            {
                _logger.LogError(db_uE, "Error updating feature with ID [" + id + "]");
                return StatusCode(500);
            }
        }

        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteFeatureById([FromRoute] int id,
            CancellationToken cancellationToken = default)
        {
            try
            {
                if(id <= 0)
                {
                    return BadRequest();
                }

                var featureToDelete = await _context.Features.FindAsync(id, cancellationToken);

                if(featureToDelete == null)
                {
                    return NotFound();
                }

                _context.Features.Remove(featureToDelete);
                await _context.SaveChangesAsync(cancellationToken);

                return NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error Deleting feature with ID [" + id + "]");
                return StatusCode(500);
            }
        }


    }
}
