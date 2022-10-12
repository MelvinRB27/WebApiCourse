using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApiCourse.DTOs;

namespace WebApiCourse.Controllers.V1
{
    [ApiController]
    [Route("api/v1")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RootController : ControllerBase
    {
        private readonly IAuthorizationService authorizationService;

        public RootController(IAuthorizationService authorizationService)
        {
            this.authorizationService = authorizationService;
        }

        [HttpGet(Name = "GetRootv1")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<DatasHATEOAS>>> Get()
        {
            var datasHATEOS = new List<DatasHATEOAS>();

            var isAdmin = await authorizationService.AuthorizeAsync(User, "isAdmin");

            if (isAdmin.Succeeded)
            {
                datasHATEOS.Add(new DatasHATEOAS(link: Url.Link("postAuthorV1", new { }),
                description: "create-author", method: "POST"));

                datasHATEOS.Add(new DatasHATEOAS(link: Url.Link("postBookV1", new { }),
                description: "create-book", method: "POST"));

            }

            datasHATEOS.Add(new DatasHATEOAS(link: Url.Link("GetRootV1", new { }),
                description: "selft", method: "GET"));

            datasHATEOS.Add(new DatasHATEOAS(link: Url.Link("getAuthorV1", new { }),
                description: "authors", method: "GET"));

            datasHATEOS.Add(new DatasHATEOAS(link: Url.Link("getBooksV1", new { }),
                description: "books", method: "GET"));


            return datasHATEOS;
        }
    }
}
