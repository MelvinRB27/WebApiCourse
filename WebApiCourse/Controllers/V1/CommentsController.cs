using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using WebApiCourse.DTOs;
using WebApiCourse.Entidades;

namespace WebApiCourse.Controllers.V1
{
    [ApiController]
    [Route("api/v1/books/{bookId:int}/comments")]
    public class CommentsController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly UserManager<IdentityUser> userManager;

        public CommentsController(ApplicationDbContext context,
            IMapper mapper,
            UserManager<IdentityUser> userManager)
        {
            this.context = context;
            this.mapper = mapper;
            this.userManager = userManager;
        }

        [HttpGet(Name = "getCommentsv1")]
        public async Task<ActionResult<List<CommentsDTOs>>> Get(int bookId)
        {
            var exists = await context.Books.AnyAsync(bookDB => bookDB.Id == bookId);
            if (!exists)
            {
                return NotFound();
            }

            var comments = await context.Comments
                .Where(commentDB => commentDB.BookId == bookId).ToListAsync();
            return mapper.Map<List<CommentsDTOs>>(comments);
        }

        [HttpGet("{id:int}", Name = "getCommentByIdv1")]
        public async Task<ActionResult<CommentsDTOs>> GetById(int id)
        {
            var comment = await context.Comments
                .FirstOrDefaultAsync(commentDB => commentDB.Id == id);

            if (comment == null)
            {
                return NotFound();
            }

            return mapper.Map<CommentsDTOs>(comment);
        }

        [HttpPost(Name = "createCommentsv1")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Post(int bookId, CommentsCreationDTOs ccd)
        {
            var emailClam = HttpContext.User.Claims.Where(clam => clam.Type == "email").FirstOrDefault();
            var email = emailClam.Value;
            var user = await userManager.FindByEmailAsync(email);
            var userID = user.Id;

            var exists = await context.Books.AnyAsync(bookDB => bookDB.Id == bookId);
            if (!exists)
            {
                return NotFound();
            }

            var comment = mapper.Map<Comments>(ccd);
            comment.BookId = bookId;
            comment.UserId = userID;
            context.Add(comment);
            await context.SaveChangesAsync();

            var commentDTOs = mapper.Map<CommentsDTOs>(comment);

            return CreatedAtRoute("getCommentByIdv1", new { id = comment.Id, bookID = bookId }, commentDTOs);
        }

        [HttpPut("{id:int}", Name = "updateCommentsv1")]
        public async Task<ActionResult> Put(int bookId, int id, CommentsCreationDTOs cd)
        {
            var exist = await context.Books.AnyAsync(bookDB => bookDB.Id == bookId);
            if (exist)
            {
                var commentexist = await context.Comments.AnyAsync(commentDB => commentDB.Id == id);
                if (!commentexist)
                {
                    return NotFound();
                }

                var comment = mapper.Map<Comments>(cd);
                comment.Id = id;
                comment.BookId = bookId;
                context.Update(comment);
                await context.SaveChangesAsync();
                return NoContent();
            }

            return NotFound();
        }

        [HttpDelete("{id:int}", Name = "deleteCommentsv1")]
        public async Task<ActionResult> Delete(int bookId, int id)
        {
            var exist = await context.Books.AnyAsync(bookDB => bookDB.Id == bookId);
            if (exist)
            {

                var commentExists = await context.Comments.AnyAsync(commentDB => commentDB.Id == id);

                if (!commentExists)
                {
                    return NotFound();
                }

                var comment = await context.Comments
                .FirstOrDefaultAsync(commentDB => commentDB.Id == id);

                context.Remove(comment);
                await context.SaveChangesAsync();
                return Ok();
            }

            return NotFound();
        }
    }
}
