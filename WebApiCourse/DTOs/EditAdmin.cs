using System.ComponentModel.DataAnnotations;

namespace WebApiCourse.DTOs
{
    public class EditAdmin
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
