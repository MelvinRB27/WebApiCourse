using System.ComponentModel.DataAnnotations;

namespace WebApiCourse.DTOs
{
    public class BookCreationDTOs
    {
        [Required]
        [StringLength(maximumLength: 250)]
        public string Title { get; set; }
        public DateTime PublicationDate { get; set; }
        public List<int> AuthorsId { get; set; }
    }
}
