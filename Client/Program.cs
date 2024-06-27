using Newtonsoft.Json;
using Model;
using System.Configuration;
using Client;

class Program
{
    private static readonly HttpClient client = new HttpClient();

    static async Task Main(string[] args)
    {
        // Build configuration
        string? baseUrl = ConfigurationManager.AppSettings["BaseUrl"];
        if (string.IsNullOrEmpty(baseUrl))
        {
            Console.WriteLine("Error: BaseUrl is not configured in appsettings.json.");
            return;
        }

        string stationId = ""; // Initialize with an empty string
        bool searchAgain = true;

        while (searchAgain)
        {
            try
            {
                // Prompt user to enter station ID
                Console.Write("Enter Station ID: ");
                stationId = Console.ReadLine() ?? ""; // Read user input for station ID and trim any extra spaces

                // Validate stationId input
                if (string.IsNullOrEmpty(stationId))
                {
                    Console.WriteLine("Station ID cannot be null or empty.");
                    continue; // Skip further processing if stationId is null or empty
                }

                // Construct the API endpoint URL with user-provided station ID
                string apiUrl = $"{baseUrl}/api/weather/{stationId}";

                // Make a GET request to the API
                HttpResponseMessage apiResponse = await client.GetAsync(apiUrl);

                // Check if the response is successful
                if (apiResponse.IsSuccessStatusCode)
                {
                    // Read the response content as a string
                    string jsonResponse = await apiResponse.Content.ReadAsStringAsync();

                    // Deserialize JSON array into List<ObservationData>
                    List<ObservationData>? observationDataList = null;

                    if (!string.IsNullOrEmpty(jsonResponse))
                    {
                        observationDataList = JsonConvert.DeserializeObject<List<ObservationData>>(jsonResponse);
                    }


                    // Check if deserialization succeeded and list is not null
                    if (observationDataList != null && observationDataList.Count > 0)
                    {
                        // Calculate average temperature
                        double averageTemperature = ObservationDataUtility.CalculateAverageTemperature(observationDataList);

                        // Display average temperature as a top-level statement
                        Console.WriteLine();
                        Console.WriteLine($"Average Temperature: {averageTemperature}°C");
                        Console.WriteLine();

                        // Display each observation data
                        Console.WriteLine("Observation Data:");
                        foreach (var observationData in observationDataList)
                        {
                            Console.WriteLine($"Name: {observationData.Name}");
                            Console.WriteLine($"Local Date Time: {observationData.local_date_time}");
                            Console.WriteLine($"Weather: {observationData.weather}°C");
                            Console.WriteLine($"Cloud: {observationData.cloud}");
                            Console.WriteLine($"Latitude: {observationData.Lat}");
                            Console.WriteLine($"Longitude: {observationData.Lon}");
                            Console.WriteLine($"Apparent Temperature: {observationData.apparent_t}°C");
                            Console.WriteLine($"Air Temperature: {observationData.air_temp}°C");
                            Console.WriteLine($"Dew Point: {observationData.dewpt}°C");
                            Console.WriteLine();
                        }

                        Console.WriteLine();
                        Console.WriteLine($"Average Temperature: {averageTemperature}°C");
                        Console.WriteLine();
                    }
                    else
                    {
                        Console.WriteLine("No observation data received.");
                    }
                }
                else if (apiResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    Console.WriteLine($"No data found for station ID: {stationId}");
                }
                else
                {
                    Console.WriteLine($"Failed to retrieve weather data. Status code: {apiResponse.StatusCode}");
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP Request Exception: {ex.Message}");
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JSON Deserialization Exception: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            // Ask user if they want to search again or exit
            Console.Write("Do you want to search again? (Y/N): ");
            string userInput = Console.ReadLine()?.ToUpper() ?? ""; // Ensure userInput is not null, handle null with empty string

            if (userInput != "Y")
            {
                searchAgain = false; // Exit the loop if user does not want to search again
            }

            Console.WriteLine();
        }

        Console.WriteLine("Exiting program. Press any key to close...");
        Console.ReadKey();
    }

}
