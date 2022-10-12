using AutoMapper;
using WebApiCourse.DTOs;
using WebApiCourse.Entidades;

namespace WebApiCourse.Utils
{
    public class AuthoMapperProfile: Profile
    {
        public AuthoMapperProfile()
        {
            CreateMap<AuthorCreationDTEOs, Author>();
            CreateMap<Author, AuthorDTOs>();
            CreateMap<Author, AuthorDTOsWhithBooks>()
                .ForMember(authorDTOs => authorDTOs.Books, options => options.MapFrom(MapAuhtorDTOBook));

            CreateMap<BookCreationDTOs, Books>()
                .ForMember(book => book.AuthorsBook, options => options.MapFrom(MapAuthorsBooks));
            CreateMap<Books, BookDTOs>();
            CreateMap<Books, BooksDTOsWhitAuthor>()
                .ForMember(bookDTEo => bookDTEo.Authors, options => options.MapFrom(MapBookDTOAuthors));

            CreateMap<CommentsCreationDTOs, Comments>();
            CreateMap<Comments, CommentsDTOs>();
            
            CreateMap<BookPatchDTO, Books>().ReverseMap();
            

        }

        private List<BookDTOs> MapAuhtorDTOBook(Author author, AuthorDTOs authorDTOs)
        {
            var results = new List<BookDTOs>();

            if (author.AuthorsBook == null) { return results; }

            foreach (var authorBook in author.AuthorsBook)
            {
                results.Add(new BookDTOs()
                {
                    Id = authorBook.BookId,
                    Title = authorBook.book.Title
                });
            }

            return results;
        }

        private List<AuthorDTOs> MapBookDTOAuthors(Books book, BookDTOs bookDTOs)
        {
            var results = new List<AuthorDTOs>();

            if (book.AuthorsBook == null) { return results; }

            foreach (var authorBook in book.AuthorsBook)
            {
                results.Add(new AuthorDTOs() 
                { 
                    Id = authorBook.AuthorId,
                    Name = authorBook.author.Name
                });
            }

            return results;
        }
        private List<AuthorBook> MapAuthorsBooks(BookCreationDTOs bco, Books book)
        {
            var results = new List<AuthorBook>();

            if (bco.AuthorsId == null) { return results; }

            foreach (var authorID in bco.AuthorsId )
            {
                results.Add(new AuthorBook() { AuthorId = authorID });
            }

            return results;
        }
    }
}
