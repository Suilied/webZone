namespace webZone.Models
{
    public class RotideFile
    {
        public string ProjectName { get; set; }
        public string FilePath { get; set; }
        public string FileContents { get; set; }
    }

    public class RotideNewProject
    {
        public string projectName { get; set; }
        public string projectRoot { get; set; }
    }
}
