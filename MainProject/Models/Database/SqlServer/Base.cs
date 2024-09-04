namespace BasicDotnetTemplate.MainProject.Models.Database.SqlServer
{
    public class Base
    {
        public int Id { get; set; }
        public DateTime CreationTime { get; set; }
        public int CreationUserId { get; set; }
        public DateTime UpdateTime { get; set; }
        public int UpdateUserId { get; set; }
        public DateTime DeletionTime { get; set; }
        public int DeletionUserId { get; set; }
    }
}



