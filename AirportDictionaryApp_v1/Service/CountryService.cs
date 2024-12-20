using AirportDictionaryApp_v1.Model;
using Microsoft.EntityFrameworkCore;

namespace AirportDictionaryApp_v1.Service
{
    // CountryService - класс для выполнения операций со странами
    public class CountryService
    {
        // объект для доступа в БД для выполнения операций
        private readonly ApplicationDbContext _db;

        public CountryService(ApplicationDbContext db)
        {
            _db = db;
        }

        // получить список стран
        public async Task<List<Country>> ListAllAsync()
        {
            return await _db.Countries.ToListAsync();
        }

        // импортировать список стран
        public async Task ImportAsync(List<Country> countries)
        {
            // убрали повторения кода во входных данных
            countries = countries
                .GroupBy(countries => countries.Code)
                .Select(countries => countries.First())
                .ToList();
            // уберем повторения которые уже есть в БД
            List<Country> existingCountries = await _db.Countries.ToListAsync();
            countries = countries
                .Where(c => existingCountries.All(ec => ec.Code != c.Code))
                .ToList();
            // сохранить оставшиеся страны
            await _db.Countries.AddRangeAsync(countries);
            await _db.SaveChangesAsync();
        }

        //получить страну по коду

        public async Task<int?> GetCountryAsyncByCode(string code)
        {

            Country? country =await _db.Countries.FirstOrDefaultAsync(c => c.Code == code);
            if (country == null)
            {
                return null;
            }
            else return country.Id;
        }
    }
}
