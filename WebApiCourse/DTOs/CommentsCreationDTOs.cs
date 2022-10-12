using System.ComponentModel.DataAnnotations;

namespace WebApiCourse.DTOs
{
    public class CommentsCreationDTOs
    {
        [StringLength(maximumLength: 250)]
        public string Content { get; set; }

    }
}
