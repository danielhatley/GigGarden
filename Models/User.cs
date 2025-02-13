namespace GigGarden.Models
{

public class User
{
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = "";
    public string Email { get; set; } = "";
}


//    public class WeatherForecast
//    {
//        public DateOnly Date { get; set; }

//        public int TemperatureC { get; set; }

//        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

//        public string? Summary { get; set; }
//    }
}
