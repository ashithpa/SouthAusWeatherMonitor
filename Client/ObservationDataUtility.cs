using Model;

namespace Client
{
    public static class ObservationDataUtility
    {
        public static double CalculateAverageTemperature(List<ObservationData> observations)
        {
            double sum = 0.0;

            foreach (var observation in observations)
            {
                sum += observation.air_temp; // Assuming air_temp is the property representing temperature
            }

            return sum / observations.Count;
        }
    }
}
