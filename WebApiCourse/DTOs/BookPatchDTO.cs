using System.ComponentModel.DataAnnotations;

namespace WebApiCourse.DTOs
{
    public class BookPatchDTO
    {
        [Required]
        [StringLength(maximumLength: 250)]
        public string Title { get; set; }
        public DateTime PublicationDate { get; set; }
    }
}
