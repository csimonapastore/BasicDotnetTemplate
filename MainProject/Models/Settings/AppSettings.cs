namespace BasicDotnetTemplate.MainProject.Models.Settings;

public class AppSettings
{
#nullable enable
    public Settings? Settings { get; set; }
    public PrivateSettings? PrivateSettings { get; set; }
    public OpenApiSettings? OpenApiSettings { get; set; }
    public DatabaseSettings? DatabaseSettings { get; set; }
    public JWTSettings? JWTSettings { get; set; }
    public EncryptionSettings? EncryptionSettings { get; set; }

#nullable disable
}