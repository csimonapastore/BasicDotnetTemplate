namespace BasicDotnetTemplate.MainProject.Models.Settings;

public class MongoDbSettings
{
#nullable enable
    public string? MongoDbConnectionString { get; set; }
    public string? DatabaseName { get; set; }
#nullable disable
}