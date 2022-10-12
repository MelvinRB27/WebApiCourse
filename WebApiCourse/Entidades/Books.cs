using System.ComponentModel.DataAnnotations;

namespace WebApiCourse.Entidades
{
    public class Books
    {
        public int Id { get; set; }

        [Required]
        [StringLength(maximumLength: 250)]
        public string Title { get; set; }
        public DateTime? PublicationDate { get; set; }
        public List<Comments> Comments { get; set; }
        public List<AuthorBook> AuthorsBook { get; set; }

    }
}
