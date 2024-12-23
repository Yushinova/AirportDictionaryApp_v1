using AirportDictionaryApp_v1.Model;
using AirportDictionaryApp_v1.Service;
using Microsoft.AspNetCore.Mvc;

namespace AirportDictionaryApp_v1.Api
{
    // CountryController - контроллер для работы со странами
    [Route("api/country")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        // сервис для работы со странами
        private readonly CountryService _countries;

        public CountryController(CountryService countries)
        {
            _countries = countries;
        }
        //все страны
        [HttpGet]
        public async Task<List<CountryMessage>> ListAllAsync()
        {
            List<Country> countries = await _countries.ListAllAsync();
            return countries
                .Select(c => new CountryMessage(Name: c.Name, Code: c.Code))
                .ToList();
        }
        //добавление страны
        [HttpPut]
        public async Task<IActionResult> ImportAsync(List<CountryMessage> countries)
        {
            List<Country> imported = countries
                .Select(c => new Country() { Name = c.Name, Code = c.Code })
                .ToList();
            await _countries.ImportAsync(imported);
            // 204
            return NoContent();

        }
        //получение аэропортов страны-задано по коду
        [HttpGet("airports/{code:alpha}")]
        public async Task<IActionResult> GetByCompanyIdAsync(string code)
        {
            List<Airport>? airports = await _countries.GetCountryAirportsAsync(code);
            if (airports == null)
            {
                return NotFound(new ErrorMessage(Type: "CountryError", Message: $"country with this id is not found"));
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

        //очистить данные всех стран с аэропортами

        [HttpDelete]
        public async Task<IActionResult> DeleteAllCountries()
        {
             await _countries.DeleteAllAsync();
             return Ok();
        }

    }
}
