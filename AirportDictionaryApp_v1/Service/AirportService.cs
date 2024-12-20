using AirportDictionaryApp_v1.Api;
using AirportDictionaryApp_v1.Model;
using Microsoft.EntityFrameworkCore;

namespace AirportDictionaryApp_v1.Service
{
    // AirportService - класс для выполнения операций с аэропортами
    public class AirportService
    {
        private readonly ApplicationDbContext _db;

        public AirportService(ApplicationDbContext db)
        {
            _db = db;
        }

        // получить список всех аэропортов
        public async Task<List<Airport>> ListAllAsync()
        {
            return await _db.Airports.ToListAsync();
        }

        // получить аэропорт по id с выгрзкой информации о стране
        public async Task<Airport?> GetAsync(int id)
        {
            return await _db.Airports
                .Include(airport => airport.Country)
                .FirstOrDefaultAsync(airport => airport.Id == id);
        }

        // получить аэропорт по коду с выгрзкой информации о стране
        public async Task<Airport?> GetAsync(string code)
        {
            return await _db.Airports
                .Include(airport => airport.Country)
                .FirstOrDefaultAsync(airport => airport.Code == code);
        }

        //добавить аэропорт

        public async Task AddAsync(Airport airport)
        {
            await _db.AddAsync(airport);
            await _db.SaveChangesAsync();
        }
        //удалить аэропорт по id

        public async Task DeleteAirport(string code)
        {
            Airport? airport = await _db.Airports.FirstOrDefaultAsync(a => a.Code == code);
            if (airport != null)
            {
                _db.Remove(airport);
                await _db.SaveChangesAsync();
            }
        }
      //проверяем есть ли аэропорт с таким кодом

        public async Task<bool> IsExists(string code)
        {
            return await _db.Airports.Where(a => a.Code == code).CountAsync() > 0;
        }

        //содержит ли аэропорт компанию
        //public async Task<bool> isInclude(Airport airport, int id)
        //{
        //    Company? company = airport.Companies.FirstOrDefault(c => c.Id == id);
            
        //}
    }



}
