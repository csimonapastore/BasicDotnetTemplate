namespace BasicDotnetTemplate.MainProject.Models.Settings;

public class DatabaseSettings
{
#nullable enable
    public string? SqlServerConnectionString { get; set; }
    public MongoDbSettings? MongoDbSettings { get; set; }
#nullable disable
}