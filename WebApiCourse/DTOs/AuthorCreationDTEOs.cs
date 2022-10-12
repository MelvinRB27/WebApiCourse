using System.ComponentModel.DataAnnotations;

namespace WebApiCourse.DTOs
{
    public class AuthorCreationDTEOs
    {
        [Required]
        [StringLength(maximumLength: 120)]
        public string Name { get; set; }
    }
}
