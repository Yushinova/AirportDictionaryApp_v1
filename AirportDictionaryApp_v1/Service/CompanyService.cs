using AirportDictionaryApp_v1.Api;
using AirportDictionaryApp_v1.Model;
using Microsoft.EntityFrameworkCore;

namespace AirportDictionaryApp_v1.Service
{
    // CompanyService - класс для работы с компаниями
    public class CompanyService
    {
        private readonly ApplicationDbContext _db;

        public CompanyService(ApplicationDbContext db)
        {
            _db = db;
        }
        // получить список компаний
        public async Task<List<Company>> ListAllAsync()
        {
            return await _db.Companies.ToListAsync();
        }
        //добавить компанию
        public async Task AddAsync(Company company)
        {
            await _db.AddAsync(company);
            await _db.SaveChangesAsync();
        }
        //получить авиакомпанию по id
        public async Task<Company?> GetAsyncById(int id)
        {
            Company? company=await _db.Companies.FirstOrDefaultAsync(c => c.Id == id);
            if (company == null)
            {
                return null;
            }
            return company;
        }       
        //получить список аэропортов, в которых присутствует заданная авиакомпания по id
        public async Task<List<Airport>?> ListAllAirportsByCompanyId(int companyid)
        {
            Company? company = await _db.Companies.Include(c=>c.Airports).FirstOrDefaultAsync(c=>c.Id==companyid);
            if (company == null)
            {
                return null;
            }
            List<Airport> airports = company.Airports.ToList();
            return airports;
        }
        //проверяем есть ли аэропорт с таким кодом
        public async Task<bool> IsExists(int id)
        {
            return await _db.Companies.Where(c => c.Id== id).CountAsync() > 0;
        }
        //удалить компанию
        public async Task DeleteAsync(int id)
        {
            Company? company = await _db.Companies.FirstOrDefaultAsync(c => c.Id == id);
            if (company != null)
            {
                _db.Remove(company);
                await _db.SaveChangesAsync();
            }
        }
    }
}
