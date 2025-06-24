namespace API.Configuration;

public class JwtSettings
{
    public string SecretKey { get; set; } = string.Empty;
    public int ExpirationHours { get; set; } = 24;
} 