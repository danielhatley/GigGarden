using GigGarden.Models.Entities;
using GigGarden.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace GigGarden.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BandController : ControllerBase
    {
        private readonly BandRepository _bandRepository;

        public BandController(IConfiguration config)
        {
            _bandRepository = new BandRepository(config);
        }

        [HttpGet("GetBands")]
        public IActionResult GetBands(bool includeDeleted = false, bool onlyDeleted = false)
        {
            var bands = _bandRepository.GetBands(includeDeleted, onlyDeleted);
            return Ok(bands);
        }

        [HttpGet("GetSingleBand/{bandId}")]
        public IActionResult GetSingleBand(int bandId)
        {
            var band = _bandRepository.GetSingleBand(bandId);
            return band != null ? Ok(band) : NotFound("Band not found.");
        }

        [HttpPut("EditBand")]
        public IActionResult EditBand(Band band)
        {
            bool success = _bandRepository.EditBand(band);
            return success ? Ok() : BadRequest("No fields provided for update.");
        }

        [HttpPost("AddBand")]
        public IActionResult AddBand(Band band)
        {
            bool success = _bandRepository.AddBand(band);
            return success ? Ok() : BadRequest("Failed to add band.");
        }

        [HttpDelete("DeleteBand/{bandId}")]
        public IActionResult DeleteBand(int bandId, int deletedBy)
        {
            bool success = _bandRepository.SoftDeleteBand(bandId, deletedBy);
            return success ? Ok() : BadRequest("Failed to delete band.");
        }

        [HttpDelete("PermanentlyDeleteBand/{bandId}")]
        public IActionResult PermanentlyDeleteBand(int bandId)
        {
            bool success = _bandRepository.PermanentlyDeleteBand(bandId);
            return success ? Ok() : BadRequest("Failed to permanently delete band.");
        }
    }
}
