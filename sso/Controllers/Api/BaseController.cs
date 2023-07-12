using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Sso.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected string GetCurrentUserId()
        {
            if (Request.HttpContext.Items["UserId"] != null)
            {
                return Request.HttpContext.Items["UserId"].ToString();
            }
            else
            {
                return null;
            }
        }
    }
}
