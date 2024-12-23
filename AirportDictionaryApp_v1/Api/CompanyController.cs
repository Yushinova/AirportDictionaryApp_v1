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
        private readonly CountryService _countries;

        public CompanyController(CompanyService companies, AirportService airports, CountryService countries)
        {
            _companies = companies;
            _airport = airports;
            _countries = countries;
        }
        [HttpPost]
        //добавление компании с массивом индексов аэропортов
        public async Task<IActionResult> AddCompanyAsync(CompanyAddMessage message)
        {
            List<Airport>? airports =await _airport.ListAllAsync();
            HashSet<Airport> findAir = new HashSet<Airport>();
            if(airports==null)
            {
                 return NotFound(new ErrorMessage(Type: "DataBaseError", Message: $"airports are not found"));
            }
            //добавляем в хэш аэропорты компании
            foreach (var item in message.idAirports)
            {
                Airport? airport = airports.FirstOrDefault(a => a.Id == item);
                if (airport == null)
                {
                    return NotFound(new ErrorMessage(Type: "AirpostsError", Message: $"airports with this id are not found"));
                }
                findAir.Add(airport);
            }
            Company company = new Company { Name = message.Name, Airports = findAir };
            await  _companies.AddAsync(company);
            return Created();  
        }
        //получить список всех авиокомпаний
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            List<Company> companies =await _companies.ListAllAsync();
            if (companies == null)
            {
                return NotFound(new ErrorMessage(Type: "CompanyError", Message: $"companies not found"));
            }
            return Ok(companies);
        }
        //ищем компанию по индексу
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            Company? company =await _companies.GetAsyncById(id);
            if (company == null)
            {
                return NotFound(new ErrorMessage(Type: "CompanyError", Message: $"company with this id is not found"));
            }
            return Ok(company);
        }
        //показать аэропорты компании (по id компании)
        [HttpGet("airports/{id:int}")]
        public async Task<IActionResult> GetByCompanyIdAsync(int id)
        {
            List<Airport>? airports= await _companies.ListAllAirportsByCompanyId(id);
            if(airports==null)
            {
                return NotFound(new ErrorMessage(Type: "CompanyError", Message: $"company with this id is not found"));
            }
            List<Country> countries = await _countries.ListAllAsync();

           // преобразовать список стран в словарь с ключами - id и значениями-кодами
            Dictionary<int, string> countryCodeById =
                countries.ToDictionary(
                    country => country.Id,
                    country => country.Code
                );
            //нужно список аэропортов перевести в читабельный результат
            return Ok(airports.Select(airport => new AirportAddMessage(
              Name: airport.Name,
              Code: airport.Code,
              OpeningYear: airport.OpeningYear,
              RunwayCount: airport.RunwayCount,
              AnnualPassengerTraffic: airport.AnnualPassengerTraffic,
              Location: airport.Location,
             CountryCode: countryCodeById[airport.CountryId]
            )).ToList());
        }
        //удаление компании (каскадное удаление работает)
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCompanyById(int id)
        {
            if (await _companies.IsExists(id) == false)
            {
                return NotFound(new ErrorMessage(Type: "CompanyNotFound", Message: $"company with id '{id}' is not found"));
            }
            else
            {
                await _companies.DeleteAsync(id);
                return NoContent();
            }

        }
    }
}
