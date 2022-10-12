using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace WebApiCourse.Entidades
{
    public class Comments
    {
        public int Id { get; set; }
        [StringLength(maximumLength: 250)]
        public string Content { get; set; }
        public int BookId { get; set; }
        public Books Book{ get; set; }
        public string UserId { get; set; }
        public IdentityUser User { get; set; }
    }
}