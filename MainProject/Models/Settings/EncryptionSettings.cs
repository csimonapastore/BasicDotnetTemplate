namespace BasicDotnetTemplate.MainProject.Models.Settings;

public class EncryptionSettings
{
#nullable enable
    public string? Salt { get; set; }
    public string? Pepper { get; set; }
#nullable disable
}