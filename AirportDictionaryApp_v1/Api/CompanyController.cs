using AirportDictionaryApp_v1.Model;
using AirportDictionaryApp_v1.Service;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Collections.Generic;

namespace AirportDictionaryApp_v1.Api
{
    // CompanyController - контроллер по работе с авиакомпаниями
    [Route("api/company")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly CompanyService _companies;
        private readonly AirportService _airport;

        public CompanyController(CompanyService companies, AirportService airports)
        {
            _companies = companies;
            _airport = airports;
        }
        [HttpPost]
        //добавление компании TODO обработать ошибки, если не найден один из добавляемый аэропортов
        public async Task<IActionResult> AddCompanyAsync(CompanyAddMessage message)
        {
            List<Airport> airports =await _airport.ListAllAsync();
           // List<Airport> find = new List<Airport>();
            HashSet<Airport> findedAir = new HashSet<Airport>();
            foreach (var item in message.idAirports)
            {
                findedAir.Add(airports.FirstOrDefault(a => a.Id == item));
            }
            if (findedAir == null)
            {
                return NotFound(new ErrorMessage(Type: "AirpostsError", Message: $"airports with this id are not found"));
            }
            else
            {
                Company company = new Company { Name = message.Name, Airports = findedAir };
                await  _companies.AddAsync(company);
                return Created();
            }
            
        }

        [HttpGet("{id:int}")]
        public async Task<Company> GetByIdAsync(int id)
        {
            return await _companies.GetAsyncById(id);
        }

        [HttpGet("air/{id:int}")]
        public async Task<List<Airport>> GetByCompanyIdAsync(int id)
        {
            List<Airport>? airports= await _companies.ListAllAirportsByCompanyId(id);
            return airports;
        }
    }
}
