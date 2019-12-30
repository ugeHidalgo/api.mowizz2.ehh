﻿using API.Mowizz2.EHH.Models;
using API.Mowizz2.EHH.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Mowizz2.EHH.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CostCentresController : ControllerBase
    {
        private readonly CostCentresService _service;

        public CostCentresController(CostCentresService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<CostCentre>>> GetAll()
        {
            return Ok(await _service.GetAll());
        }

        [HttpGet("{company}", Name = "GetCostCentresForCompany")]
        public async Task<ActionResult<List<CostCentre>>> GetAllForCompany(string company)
        {
            return Ok(await _service.GetAllForCompany(company));
        }

        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<CostCentre>> GetById(string id)
        {
            CostCentre CostCentre = await _service.GetById(id);

            if (CostCentre == null)
            {
                return NotFound(string.Format("No cost centre with Id: \'{0}\' exist", id));
            }

            return CostCentre;
        }

        [HttpPost]
        public async Task<List<CostCentre>> CreateCostCentres([FromBody] List<CostCentre> CostCentres)
        {
            List<CostCentre> newCostCentres = await _service.CreateCostCentres(CostCentres);

            return newCostCentres;
        }
    }
}