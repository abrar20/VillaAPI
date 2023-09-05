using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Net;
using VillaAPI.Data;
using VillaAPI.Logging;
using VillaAPI.Migrations;
using VillaAPI.Models;
using VillaAPI.Models.VillaDTO;
using VillaAPI.Repository.IRepository;

namespace VillaAPI.Controllers.v1
{
    //[Route("api/[controller]")]
    [Route("api/v{version:apiVersion}/VillaNumberAPI")]

    [ApiController]
    [ApiVersion("1.0")]
    public class VillaNumberAPIv1Controller : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IVillaRepository _dbVillas;
        private readonly IVillaNumberRepository _dbVillaNumber;
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
        public VillaNumberAPIv1Controller(IMapper mapper, IVillaRepository dbVillas, IVillaNumberRepository dbVillaNumber)
        {
            _mapper = mapper;
            _dbVillaNumber = dbVillaNumber;
            _response = new();
            _dbVillas = dbVillas;
        }
        [HttpGet]

        //[MapToApiVersion("1.0")]
        public async Task<ActionResult<APIResponse>> GetVillasNumber()
        {
            //_logger.Log("Get All Villas"+ VillaStore.villaList,"");
            //before using mapper
            //return Ok(await _db.Villas.ToListAsync());
            try
            {
                // value of includeProperties:"Villa" is sensetive we can not use villa
                IEnumerable<VillaNumber> villaNumberList = await _dbVillaNumber.GetAllAsync(includeProperties: "Villa");
                _response.Result = _mapper.Map<List<VillaNumberDTO>>(villaNumberList);
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

        [HttpGet("{id:int}", Name = "GetVillaNumber")]
        public async Task<ActionResult<APIResponse>> GetVillaNumber(int id)
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
                // value of includeProperties:"Villa" is sensetive we can not use villa
                var villaNumber = await _dbVillaNumber.GetAsync(x => x.VillaNo == id, includeProperties: "Villa");
                if (villaNumber == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.IsSuccess = false;
                    return NotFound(_response);
                }
                //return Ok(villa);
                _response.Result = _mapper.Map<VillaNumberDTO>(villaNumber);
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
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> Create([FromBody] VillaNumberCreateDTO createDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                if (await _dbVillaNumber.GetAsync(u => u.VillaNo == createDTO.VillaNo) != null)
                {
                    ModelState.AddModelError("ErrorMessages", "Villa number already exist");
                    return BadRequest(ModelState);
                }
                if (await _dbVillas.GetAsync(u => u.Id == createDTO.VillaID) == null)
                {
                    ModelState.AddModelError("ErrorMessages", "Villa Id Not Found");
                    return NotFound(ModelState);
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
                VillaNumber villaNumber = _mapper.Map<VillaNumber>(createDTO);
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
                await _dbVillaNumber.CreateAsync(villaNumber);
                _response.Result = _mapper.Map<VillaNumberDTO>(villaNumber);
                _response.StatusCode = HttpStatusCode.Created;
                //return Ok();
                return CreatedAtRoute("GetVillaNumber", new { id = villaNumber.VillaNo }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;

        }
        [Authorize(Roles = "admin")]
        [HttpDelete("{id:int}", Name = "DeleteVillaNumber")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> DeleteVillaNumber(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }
                var villaNumber = await _dbVillaNumber.GetAsync(u => u.VillaNo == id);
                if (villaNumber == null) { return NotFound(); }
                //there is no RemoveAsync
                await _dbVillaNumber.RemoveAsync(villaNumber);
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
        [Authorize(Roles = "admin")]
        [HttpPut("{id:int}", Name = "UpdateVillaNumber")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> UpdateVillaNumber(int id, [FromBody] VillaNumberUpdateDTO updateDTO)
        {
            try
            {
                if (updateDTO == null || id != updateDTO.VillaNo)
                {
                    return BadRequest();
                }
                if (await _dbVillas.GetAsync(u => u.Id == updateDTO.VillaID) == null)
                {
                    ModelState.AddModelError("ErrorMessages", "Villa Id Not Found");
                    return NotFound(ModelState);
                }
                /*var villa = _db.Villas.FirstOrDefault(u => u.Id == id);
                villa.Name = villaDTO.Name;
                villa.Occupancy = villaDTO.Occupancy;
                villa.Sqft = villaDTO.Sqft;*/
                // instead of below bi object we can do this
                //Villa model = _mapper.Map<output>(source);
                VillaNumber model = _mapper.Map<VillaNumber>(updateDTO);
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
                await _dbVillaNumber.UpdateAsync(model);
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
        [HttpPatch("{id:int}", Name = "UpdatePatialVillaNumber")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdatePartialVillaNumber(int id, JsonPatchDocument<VillaNumberUpdateDTO> patchDTO)
        {
            if (patchDTO == null || id == 0)
            {
                return BadRequest();
            }
            // here we use AsNoTracking beacuse we didnot change the villa itself,but create another model
            // so we must do that otherwise will oucer an tracking error 
            var villaNumber = await _dbVillaNumber.GetAsync(u => u.VillaNo == id, tracked: false);
            VillaNumberUpdateDTO villaNumberDTO = _mapper.Map<VillaNumberUpdateDTO>(villaNumber);
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
            if (villaNumber == null) { return BadRequest(); }
            patchDTO.ApplyTo(villaNumberDTO, ModelState);
            //Villa model = _mapper.Map<output>(source);
            VillaNumber model = _mapper.Map<VillaNumber>(villaNumberDTO);
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
            await _dbVillaNumber.UpdateAsync(model);
            //await _db.SaveChangesAsync();
            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            return NoContent();
        }
    }



}
