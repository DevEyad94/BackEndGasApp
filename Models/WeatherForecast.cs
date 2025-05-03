namespace BackEndGasApp.Models;

public class WeatherForecast
{
    /// <summary>
    /// The date of the forecast
    /// </summary>
    public DateOnly Date { get; set; }

    /// <summary>
    /// Temperature in Celsius
    /// </summary>
    public int TemperatureC { get; set; }

    /// <summary>
    /// Temperature in Fahrenheit, calculated from Celsius
    /// </summary>
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    /// <summary>
    /// Text summary of the weather condition
    /// </summary>
    public string? Summary { get; set; }
} 