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

namespace VillaAPI.Controllers.v2
{
    //[Route("api/[controller]")]
    [Route("api/v{version:apiVersion}/VillaNumberAPI")]

    [ApiController]
    [ApiVersion("2.0")]
    public class VillaNumberAPIv2Controller : ControllerBase
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
        public VillaNumberAPIv2Controller(IMapper mapper, IVillaRepository dbVillas, IVillaNumberRepository dbVillaNumber)
        {
            _mapper = mapper;
            _dbVillaNumber = dbVillaNumber;
            _response = new();
            _dbVillas = dbVillas;
        }

        //[MapToApiVersion("2.0")]
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "Value1", "Valu2" };
        }

    }



}
