using System.ComponentModel.DataAnnotations;

namespace WebApiCourse.Entidades
{
    public class Author
    {
        public int Id { get; set; }

        [Required]
        [StringLength(maximumLength:120)]
        public string Name { get; set; }
        public List<AuthorBook> AuthorsBook { get; set; }
    }
}
