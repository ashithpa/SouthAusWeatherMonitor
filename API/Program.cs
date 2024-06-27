using System.Net.Http.Headers;
using API.Services;
using Model;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure WeatherApiSettings
builder.Services.Configure<WeatherApiSettings>(builder.Configuration.GetSection("WeatherApiSettings"));

// Configure HTTP Client for WeatherService
builder.Services.AddHttpClient<IWeatherService, WeatherService>(client =>
{
    var weatherApiSettings = builder.Configuration.GetSection("WeatherApiSettings").Get<WeatherApiSettings>();
    client.BaseAddress = new Uri(weatherApiSettings.BaseUrl);
    client.DefaultRequestHeaders.Accept.Clear();
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});

// Add Swagger/OpenAPI documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
