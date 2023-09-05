using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json;
using VillaAPI.Data;
using VillaAPI.Logging;
using VillaAPI.Migrations;
using VillaAPI.Models;
using VillaAPI.Models.VillaDTO;
using VillaAPI.Repository.IRepository;

namespace VillaAPI.Controllers.v1
{
    //[Route("api/[controller]")]
    [Route("api/v{version:apiVersion}/VillaAPI")]
    [ApiVersion("1.0")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IVillaRepository _dbVilla;
        protected APIResponse _response;
        //default
        /*private readonly ILogger<VillaAPIController> _logger;

        public VillaAPIController(ILogger<VillaAPIController> logger)
        {
            _logger = logger;
        }*/
        //custome logging
        /*private readonly ILogging _logger;
        public VillaAPIController(ILogging logger)
        {
            _logger = logger;
        }*/
        public VillaAPIController(IMapper mapper, IVillaRepository dbVilla)
        {
            _mapper = mapper;
            _dbVilla = dbVilla;
            _response = new();
        }
        [HttpGet]
        //[Authorize]
        //[ResponseCache(Duration =30)]
        [ResponseCache(CacheProfileName = "Default30")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetVillas([FromQuery(Name ="Filter Occupancy")] int? occupancy,
            [FromQuery] string? search, int pageSize = 0, int pageNumber = 1)
        {
            //_logger.Log("Get All Villas"+ VillaStore.villaList,"");
            //before using mapper
            //return Ok(await _db.Villas.ToListAsync());
            try
            {
                IEnumerable<Villa> villaList;
                if(occupancy > 0)
                {
                   villaList= await _dbVilla.GetAllAsync(u=>u.Occupancy == occupancy,
                       pageSize:pageSize,pageNumber:pageNumber);
                }
                else
                {
                    villaList = await _dbVilla.GetAllAsync(pageSize: pageSize, pageNumber: pageNumber);
                }
                if (!string.IsNullOrEmpty(search))
                {
                    villaList = villaList.Where(u =>  u.Name.ToLower().Contains(search));
                }
                Pagination pagination = new ()
                {
                    PageNumber = pageNumber,PageSize=pageSize
                };
                Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pagination));
                _response.Result = _mapper.Map<List<VillaDTO>>(villaList);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }
        [HttpGet("{id:int}", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        //[Authorize(Roles = "admin")]
        [ResponseCache(CacheProfileName = "Default30")]
        public async Task<ActionResult<APIResponse>> GetVilla(int id)
        {
            try
            {
                if (id == 0)
                {
                    //_logger.Log("error id =" + id,"error");
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    return BadRequest(_response);
                }
                var villa = await _dbVilla.GetAsync(x => x.Id == id);
                if (villa == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsSuccess = false;
                    return NotFound(_response);
                }
                //return Ok(villa);
                _response.Result = _mapper.Map<VillaDTO>(villa);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }
        [HttpPost]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> Create([FromBody] VillaCreateDTO createDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                if (await _dbVilla.GetAsync(u => u.Name.ToLower() == createDTO.Name.ToLower()) != null)
                {
                    ModelState.AddModelError("ErrorMessages", "Villa already exist");
                    return BadRequest(ModelState);
                }
                if (createDTO == null)
                {
                    return BadRequest();
                }
                /*if(villaDTO.Id > 0)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }*/
                //villaDTO.Id = _db.Villas.OrderByDescending(u => u.Id).FirstOrDefault().Id + 1;
                //instead of below object we can do this
                Villa villa = _mapper.Map<Villa>(createDTO);
                /*Villa model = new()
                {
                    Amenity= createDTO.Amenity,
                    Sqft= createDTO.Sqft,
                    Name= createDTO.Name,
                    //Id= villaDTO.Id,
                    ImageUrl= createDTO.ImageUrl,
                    Occupancy= createDTO.Occupancy,
                    Rate= createDTO.Rate,
                    Details= createDTO.Details
                };*/
                /*await _db.Villas.AddAsync(model);
                await _db.SaveChangesAsync();*/
                await _dbVilla.CreateAsync(villa);
                _response.Result = _mapper.Map<VillaDTO>(villa);
                _response.StatusCode = HttpStatusCode.Created;
                //return Ok();
                return CreatedAtRoute("GetVilla", new { id = villa.Id }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;

        }
        [HttpDelete("{id:int}", Name = "DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<APIResponse>> DeleteVilla(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }
                var villa = await _dbVilla.GetAsync(u => u.Id == id);
                if (villa == null) { return NotFound(); }
                //there is no RemoveAsync
                await _dbVilla.RemoveAsync(villa);
                //await _db.SaveChangesAsync();
                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }
        [HttpPut("{id:int}", Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<APIResponse>> UpdateVilla(int id, [FromBody] VillaUpdateDTO updateDTO)
        {
            try
            {
                if (updateDTO == null || id != updateDTO.Id)
                {
                    return BadRequest();
                }
                /*var villa = _db.Villas.FirstOrDefault(u => u.Id == id);
                villa.Name = villaDTO.Name;
                villa.Occupancy = villaDTO.Occupancy;
                villa.Sqft = villaDTO.Sqft;*/
                // instead of below bi object we can do this
                //Villa model = _mapper.Map<output>(source);
                Villa model = _mapper.Map<Villa>(updateDTO);
                /*Villa model = new()
                {
                    Amenity = updateDTO.Amenity,
                    Sqft = updateDTO.Sqft,
                    Name = updateDTO.Name,
                    Id = updateDTO.Id,
                    ImageUrl = updateDTO.ImageUrl,
                    Occupancy = updateDTO.Occupancy,
                    Rate = updateDTO.Rate,
                    Details = updateDTO.Details
                };*/
                //there is no updateAsync
                //_db.Villas.Update(model);
                //await _db.SaveChangesAsync();
                await _dbVilla.UpdateAsync(model);
                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
        }
        [HttpPatch("{id:int}", Name = "UpdatePatialVilla")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDTO> patchDTO)
        {
            if (patchDTO == null || id == 0)
            {
                return BadRequest();
            }
            // here we use AsNoTracking beacuse we didnot change the villa itself,but create another model
            // so we must do that otherwise will oucer an tracking error 
            var villa = await _dbVilla.GetAsync(u => u.Id == id, tracked: false);
            VillaUpdateDTO villaDTO = _mapper.Map<VillaUpdateDTO>(villa);
            /*VillaUpdateDTO villaUpdateDTO = new()
            {
                Amenity = villa.Amenity,
                Sqft = villa.Sqft,
                Name = villa.Name,
                Id = villa.Id,
                ImageUrl = villa.ImageUrl,
                Occupancy = villa.Occupancy,
                Rate = villa.Rate,
                Details = villa.Details
            };*/
            if (villa == null) { return BadRequest(); }
            patchDTO.ApplyTo(villaDTO, ModelState);
            //Villa model = _mapper.Map<output>(source);
            Villa model = _mapper.Map<Villa>(villaDTO);
            /*Villa model = new()
            {
                Amenity = villaUpdateDTO.Amenity,
                Sqft = villaUpdateDTO.Sqft,
                Name = villaUpdateDTO.Name,
                Id = villaUpdateDTO.Id,
                ImageUrl = villaUpdateDTO.ImageUrl,
                Occupancy = villaUpdateDTO.Occupancy,
                Rate = villaUpdateDTO.Rate,
                Details = villaUpdateDTO.Details
            };*/
            await _dbVilla.UpdateAsync(model);
            //await _db.SaveChangesAsync();
            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            return NoContent();
        }
    }



}
