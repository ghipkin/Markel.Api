using Markel.Api.Mapping;
using Markel.Service;
using Markel.Service.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using EXT = Markel.Api.ExternalModel;
using INT = Markel.Service.InternalModel;

namespace Markel.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {

        private readonly ILogger<HomeController> _logger;
        private readonly IMarkelService _markelService;

        public HomeController(ILogger<HomeController> logger, IMarkelService markelService)
        {
            _logger = logger;
            _markelService = markelService;
        }

        [HttpGet]
        [Route("/[controller]/[action]/{companyId}")]
        public async Task<ActionResult<EXT.Company>> GetCompany(int companyId)
        {
            try
            {
                INT.Company company = await _markelService.GetCompany(companyId);

                if (company == null || company.IsNull)
                {
                    return StatusCode(StatusCodes.Status404NotFound);
                }

                return new ActionResult<EXT.Company>(CompanyMapping.MapCompanyToExternal(company));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpGet]
        [Route("/[controller]/[action]/{companyId}")]
        public async Task<ActionResult<IEnumerable<EXT.Claim>>> GetClaims(int companyId)
        {
            try 
            {
                IEnumerable<INT.Claim> claims = await _markelService.GetClaims(companyId);

                if (claims == null || !claims.Any())
                {
                    return StatusCode(StatusCodes.Status204NoContent);
                }

                return new ActionResult<IEnumerable<EXT.Claim>>(ClaimsMapping.MapClaimsToExternal(claims));

            }
            catch (Exception ex) 
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpGet]
        [Route("/[controller]/[action]/{ucr}")]
        public async Task<ActionResult<EXT.Claim>> GetClaimDetails(string ucr)
        {

            try
            {
                INT.Claim claim = await _markelService.GetClaim(ucr);

                if (claim == null || claim.IsNull)
                {
                    return StatusCode(StatusCodes.Status404NotFound);
                }

                return new ActionResult<EXT.Claim>(ClaimsMapping.MapClaimToExternal(claim));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateClaim(EXT.Claim claim)
        {
            try
            {
                if (claim == null || string.IsNullOrWhiteSpace(claim.Ucr))
                {
                    return StatusCode(StatusCodes.Status422UnprocessableEntity);
                }

                _markelService.UpdateClaim(ClaimsMapping.MapClaimToInternal(claim));

                return Ok();

            }
            catch (ClaimNotFoundException ex)
            {
                return StatusCode(StatusCodes.Status404NotFound, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
    }
}
