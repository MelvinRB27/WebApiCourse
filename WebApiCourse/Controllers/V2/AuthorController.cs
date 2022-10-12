using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;
using WebApiCourse.DTOs;
using WebApiCourse.Entidades;

namespace WebApiCourse.Controllers.V2
{
    [ApiController]
    [Route("api/v2/authors")]
    public class AuthorController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IAuthorizationService authorizationService;

        public AuthorController(ApplicationDbContext context, IMapper mapper, IAuthorizationService authorizationService)
        {
            this.context = context;
            this.mapper = mapper;
            this.authorizationService = authorizationService;
        }

        //endpoints 

        [HttpGet(Name = "getAuthorv2")]
        [ResponseCache(Duration = 10)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [AllowAnonymous]
        public async Task<ResourceColletion<AuthorDTOs>> Get()
        {
            var authors = await context.Authors.ToListAsync();
            var dto = mapper.Map<List<AuthorDTOs>>(authors);

            var isAdmin = await authorizationService.AuthorizeAsync(User, "isAdmin");

            dto.ForEach(dto => GenerateLinks(dto, isAdmin.Succeeded));

            var result = new ResourceColletion<AuthorDTOs> { Valors = dto };
            result.Links.Add(new DatasHATEOAS(link: Url.Link("getAuthor", new { }),
                description: "selft",
                method: "GETV2"));

            if (isAdmin.Succeeded)
            {
                result.Links.Add(new DatasHATEOAS(link: Url.Link("postAuthor", new { }),
                    description: "create-author",
                    method: "POST"));
            }
            return result;
        }

        [HttpGet("{id:int}", Name = "getAuthorByIdv2")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthorDTOsWhithBooks>> Get(int id)
        {
            var author = await context.Authors
                .Include(authorDB => authorDB.AuthorsBook)
                .ThenInclude(authorBookDTOs => authorBookDTOs.book)
                .FirstOrDefaultAsync(authorDB => authorDB.Id == id);

            var isAdmin = await authorizationService.AuthorizeAsync(User, "isAdmin");


            if (author == null)
            {
                return NotFound();
            }

            var dto = mapper.Map<AuthorDTOsWhithBooks>(author);
            GenerateLinks(dto, isAdmin.Succeeded);
            return dto;
        }



        [NonAction]
        private void GenerateLinks(AuthorDTOs authorDTOs, bool isAdmin)
        {
            authorDTOs.Links.Add(new DatasHATEOAS(link: Url.Link("getAuthor", new
            { id = authorDTOs.Id }), description: "selft", method: "GET"));

            if (isAdmin)
            {
                authorDTOs.Links.Add(new DatasHATEOAS(link: Url.Link("updateAuthor", new
                { id = authorDTOs.Id }), description: "update-author", method: "PUT"));

                authorDTOs.Links.Add(new DatasHATEOAS(link: Url.Link("deleteAuthor", new
                { id = authorDTOs.Id }), description: "delete-author", method: "DELETE"));

            }
        }


        [HttpPost(Name = "postAuthorv2")]
        public async Task<ActionResult> Post(AuthorCreationDTEOs acd)
        {
            var author = mapper.Map<Author>(acd);

            context.Add(author);
            await context.SaveChangesAsync();

            var authorDTOs = mapper.Map<AuthorDTOs>(author);

            return CreatedAtRoute("getAuthorByIdv2", new { id = author.Id }, authorDTOs);
        }

        [HttpPut("{id:int}", Name = "updateAuthorv2")] //api/authors/1
        public async Task<ActionResult> Put(Author author, int id)
        {

            var exist = await context.Authors.AnyAsync(authorDB => authorDB.Id == id);
            if (exist)
            {
                if (author.Id != id)
                {
                    return BadRequest("Author id does not match url id");
                }

                context.Update(author);
                await context.SaveChangesAsync();

                return Ok();
            }
            return NotFound();
        }

        [HttpDelete("{id:int}", Name = "deleteAuthorv2")]
        public async Task<ActionResult> Delete(Author author, int id)
        {
            var exist = await context.Authors.AnyAsync(authorDB => authorDB.Id == id);
            if (exist)
            {
                if (author.Id != id)
                {
                    return BadRequest("Author id does not match url id");
                }

                context.Remove(author);
                await context.SaveChangesAsync();

                return Ok();
            }
            return NotFound();
        }
    }

}
