using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AirportDictionaryApp_v1.Api
{
    // record-ы сообщений API

    // StringMessage - строковое сообщение
    public record StringMessage(string Message);

    // ErrorMessage - сообщение с ошибкой
    public record ErrorMessage(string Type,  string Message);

    // CountryMessage - сообщение с данными о стране
    public record CountryMessage(string Name, string Code);

    // AirportListItemMessage - сообщение с данными об аэропорте в списке аэропортов
    public record AirportListItemMessage(int Id, string Name, string Code, string Location, string CountryCode);

    // AirportMessage - сообщение с полными данными об аэропорте
    public record AirportMessage(
        int Id, 
        string Name, 
        string Code, 
        int OpeningYear,
        int RunwayCount,
        long AnnualPassengerTraffic,
        string Location,
        int CountryId,
        CountryMessage Country
    );

    //AirportAddMessage-сообщение для добавления аэропорта

    public record AirportAddMessage(
        string Name,
        string Code,
        int OpeningYear,
        int RunwayCount,
        long AnnualPassengerTraffic,
        string Location,
        string CountryCode
        );

    //AddCompany
    public record CompanyAddMessage(
        string Name,
        int[] idAirports
        );

}
