namespace DigitalGoldWallet.API.Configuration;
//Ekta
public class AppSettings
{
    public string ApplicationName { get; set; } = string.Empty;

    public string Environment { get; set; } = string.Empty;

    public JwtSettings JwtSettings { get; set; } = new();
}

//Ekta