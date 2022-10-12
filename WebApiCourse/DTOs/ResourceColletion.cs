namespace WebApiCourse.DTOs
{
    public class ResourceColletion<T>: Resources where T : Resources
    {
        public List<T> Valors { get; set; }
    }
}
