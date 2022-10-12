using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiCourse.DTOs;
using WebApiCourse.Entidades;

namespace WebApiCourse.Controllers.V1
{
    [ApiController]
    [Route("api/v1/books")]
    public class BooksController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public BooksController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet(Name = "getBooksv1")] // api/books/
        public async Task<ActionResult<List<BookDTOs>>> Get()
        {
            var books = await context.Books.ToListAsync();
            return mapper.Map<List<BookDTOs>>(books);
        }

        [HttpGet("{id:int}", Name = "getBookByIdv1")] // api/books/1
        public async Task<ActionResult<BooksDTOsWhitAuthor>> Get(int id)
        {
            var book = await context.Books
                .Include(bookDB => bookDB.AuthorsBook)
                .ThenInclude(authorBookDB => authorBookDB.author)
                .FirstOrDefaultAsync(bookDB => bookDB.Id == id);

            if (book == null)
            {
                return NotFound();
            }


            return mapper.Map<BooksDTOsWhitAuthor>(book);
        }

        [HttpPost(Name = "postBookv1")] // api/books
        public async Task<ActionResult> Post(BookCreationDTOs bco)
        {

            if (bco.AuthorsId == null)
            {
                return BadRequest("Unable to create a workbook without an author");
            }

            var authorsId = await context.Authors
                .Where(authorDB => bco.AuthorsId.Contains(authorDB.Id)).Select(x => x.Id).ToListAsync();

            if (bco.AuthorsId.Count != authorsId.Count)
            {
                return BadRequest("There is no one of the sent authores");
            }

            var book = mapper.Map<Books>(bco);

            AssignOrderAuthor(book);



            context.Add(book);
            await context.SaveChangesAsync();

            var bookDTO = mapper.Map<BookDTOs>(book);

            return CreatedAtRoute("getBookByIdv1", new { id = book.Id }, bookDTO);

        }



        [HttpPut("{id:int}", Name = "updateBookv1")] // api/books/1
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]

        public async Task<ActionResult> Put(int id, BookCreationDTOs bco)
        {
            var book = await context.Books.Include(bookDB => bookDB.AuthorsBook).FirstOrDefaultAsync(x => x.Id == id);
            if (book != null)
            {
                var books = mapper.Map(bco, book);
                AssignOrderAuthor(books);

                context.Update(books);
                await context.SaveChangesAsync();
                return NoContent();
            }

            return NotFound();

        }

        [HttpDelete("{id:int}", Name = "deleteBookv1")] // api/books/1
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
        public async Task<ActionResult> Delete(int id)
        {
            var exist = await context.Books.AnyAsync(bookDB => bookDB.Id == id);
            if (exist)
            {
                context.Remove(new Books() { Id = id });
                await context.SaveChangesAsync();
                return NoContent();
            }

            return NotFound();
        }

        private void AssignOrderAuthor(Books book)
        {
            if (book.AuthorsBook != null)
            {
                for (int i = 0; i < book.AuthorsBook.Count; i++)
                {
                    book.AuthorsBook[i].Order = i;
                }
            }
        }

        [HttpPatch("{id:int}", Name = "pathBookv1")]
        public async Task<ActionResult> Patch(int id, JsonPatchDocument<BookPatchDTO> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            var bookDB = await context.Books.FirstOrDefaultAsync(x => x.Id == id);
            if (bookDB == null)
            {
                return NotFound();
            }

            var bookDTO = mapper.Map<BookPatchDTO>(bookDB);

            patchDocument.ApplyTo(bookDTO, ModelState);

            var isValid = TryValidateModel(bookDTO);
            if (!isValid)
            {
                return BadRequest(ModelState);
            }

            mapper.Map(bookDTO, bookDB);
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}