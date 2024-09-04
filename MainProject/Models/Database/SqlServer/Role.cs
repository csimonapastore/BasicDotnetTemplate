using System.Text.Json.Serialization;

namespace BasicDotnetTemplate.MainProject.Models.Database.SqlServer
{
    public class Role : Base
    {
        public required string Name { get; set; }
    }
}



