using System.ComponentModel.DataAnnotations;
using WebApiCourse.Entidades;

namespace WebApiCourse.DTOs
{
    public class BookDTOs
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public DateTime PublicationDate { get; set; }

        //public List<CommentsDTOs> Comments { get; set; }

    }
}
