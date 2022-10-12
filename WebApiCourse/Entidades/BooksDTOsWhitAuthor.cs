using WebApiCourse.DTOs;

namespace WebApiCourse.Entidades
{
    public class BooksDTOsWhitAuthor: BookDTOs
    {
        public List<AuthorDTOs> Authors { get; set; }

    }
}
