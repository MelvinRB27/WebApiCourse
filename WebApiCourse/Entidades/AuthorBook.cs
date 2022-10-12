namespace WebApiCourse.Entidades
{
    public class AuthorBook
    {
        public int AuthorId { get; set; }
        public int BookId { get; set; }
        public int Order { get; set; }
        public Author author { get; set; }
        public Books book { get; set; }
    }
}
