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

        [HttpGet]
        public async Task<List<CountryMessage>> ListAllAsync()
        {
            List<Country> countries = await _countries.ListAllAsync();
            return countries
                .Select(c => new CountryMessage(Name: c.Name, Code: c.Code))
                .ToList();
        }

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


    }
}
