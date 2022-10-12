namespace WebApiCourse.DTOs
{
    public class DatasHATEOAS
    {
        public string Link { get; private set; }
        public string Description { get; private set; }

        public string Method { get; private set; }

        public DatasHATEOAS(string link, string description, string method)
        {
            Link = link;
            Description = description;
            Method = method;
        }   
    }
}
