using WebApiCourse.DTOs;

namespace WebApiCourse.Entidades
{
    public class AuthorDTOsWhithBooks: AuthorDTOs
    {
        public List<BookDTOs> Books { get; set; }

    }
}
