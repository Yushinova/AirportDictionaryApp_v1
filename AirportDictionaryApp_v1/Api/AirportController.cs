using AirportDictionaryApp_v1.Model;
using AirportDictionaryApp_v1.Service;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace AirportDictionaryApp_v1.Api
{
    // AirportController - контроллер для работы с аэропортами
    [Route("api/airport")]
    [ApiController]
    public class AirportController : ControllerBase
    {
        // используемые сервисы
        private readonly AirportService _airports;
        private readonly CountryService _countries;

        public AirportController(AirportService airports, CountryService countries)
        {
            _airports = airports;
            _countries = countries;
        }

        [HttpGet]
        public async Task<List<AirportListItemMessage>> GetAllAsync()
        {
            // получить аэропорты и страны
            List<Airport> airports = await _airports.ListAllAsync();
            List<Country> countries = await _countries.ListAllAsync();
            
            // преобразовать список стран в словарь с ключами-id и значениями-кодами
            Dictionary<int, string> countryCodeById = 
                countries.ToDictionary(
                    country => country.Id, 
                    country => country.Code
                );

            // собрать список сообщений со аэропортами
            return airports.Select(airport => new AirportListItemMessage(
                Id: airport.Id,
                Name: airport.Name,
                Code: airport.Code,
                Location: airport.Location,
                CountryCode: countryCodeById[airport.CountryId]
            )).ToList();
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            Airport? airport = await _airports.GetAsync(id);
            if (airport == null)
            {
                // 404
                return NotFound(new ErrorMessage(Type: "AirportNotFound", Message: $"airport with id '{id}' not found"));
            }
            // 200
            AirportMessage result = new AirportMessage(
                Id: airport.Id,
                Name: airport.Name,
                Code: airport.Code,
                OpeningYear: airport.OpeningYear,
                RunwayCount: airport.RunwayCount,
                AnnualPassengerTraffic: airport.AnnualPassengerTraffic,
                Location: airport.Location,
                CountryId: airport.CountryId,
                Country: new CountryMessage(Name: airport.Country!.Name, Code: airport.Country!.Code)
            );
            return Ok(result);
        }

        [HttpGet("{code:alpha}")]
        public async Task<IActionResult> GetByCodeAsync(string code)
        {
            Airport? airport = await _airports.GetAsync(code);
            if (airport == null)
            {
                // 404
                return NotFound(new ErrorMessage(Type: "AirportNotFound", Message: $"airport with code '{code}' not found"));
            }
            // 200
            AirportMessage result = new AirportMessage(
                Id: airport.Id,
                Name: airport.Name,
                Code: airport.Code,
                OpeningYear: airport.OpeningYear,
                RunwayCount: airport.RunwayCount,
                AnnualPassengerTraffic: airport.AnnualPassengerTraffic,
                Location: airport.Location,
                CountryId: airport.CountryId,
                Country: new CountryMessage(Name: airport.Country!.Name, Code: airport.Country!.Code)
            );
            return Ok(result);
        }
        //добавдение аэропорта
        [HttpPost]

        public async Task<IActionResult> AddAirportFromMessage(AirportAddMessage message)
        {
            int? countryId = await _countries.GetCountryAsyncByCode(message.CountryCode);
            if (countryId == null)
            {
                // 404
                return NotFound(new ErrorMessage(Type: "CountryNotFound", Message: $"country with code '{message.CountryCode}' not found"));
            }
            if(await _airports.IsExists(message.Code))
            {
                return NotFound(new ErrorMessage(Type: "DublicatedCode", Message: $"airport with code '{message.Code}' is added"));
            }
            else
            {
                Airport airport = new Airport()
                {
                    Name = message.Name,
                    Code = message.Code,
                    OpeningYear = message.OpeningYear,
                    RunwayCount = message.RunwayCount,
                    AnnualPassengerTraffic = message.AnnualPassengerTraffic,
                    Location = message.Location,
                    CountryId = countryId.Value
                };

               await _airports.AddAsync(airport);
               return Created();
            }
        }

        [HttpDelete("{code:alpha}")]
        public async Task<IActionResult> DeleteAirportByCode(string code)
        {
            if (await _airports.IsExists(code)==false)
            {
                return NotFound(new ErrorMessage(Type: "AirportNotFound", Message: $"airport with code '{code}' not found"));
            }
            else
            {
                await _airports.DeleteAirport(code);
                return NoContent();
            }
           
        }
    }
}
