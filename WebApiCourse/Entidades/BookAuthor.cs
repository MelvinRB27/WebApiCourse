namespace WebApiCourse.Entidades
{
    public class BookAuthor
    {
        public int BookId { get; set; }
        public int AuthorId { get; set; }
        public int Order { get; set; }
        public Books Book { get; set; }
        public Author Author { get; set; }
    }
}
