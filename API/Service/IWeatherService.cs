using Model;

namespace API.Services
{
    public interface IWeatherService
    {
        Task<List<ObservationData>> GetWeatherObservationsAsync(string stationId);
    }
}
