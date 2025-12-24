using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using FoodSphere.Data.Models;

namespace FoodSphere.Controllers
{
    [ApiController]
    public class AppController : ControllerBase
    {
        protected static string GetContollerName(string name)
        {
            // get class dynamically, directly -> use type argument
            return name.EndsWith("Controller") ? name[..^"Controller".Length] : name;
        }
    }

    [AdminAuthorize]
    public class AdminController : AppController
    {
        protected string UserId
        {
            get
            {
                var userId = User.FindFirstValue(AppClaimType.Identity.UserIdClaimType)
                    ?? throw new InvalidOperationException();

                return userId;
            }
        }

        protected string UserRole
        {
            get
            {
                var role = User.FindFirstValue(AppClaimType.Identity.RoleClaimType)
                    ?? throw new InvalidOperationException();

                return role;
            }
        }
    }

    [ClientAuthorize]
    public class ClientController : AppController
    {
        protected UserType UserType
        {
            get
            {
                return (UserType)HttpContext.Items[nameof(UserType)]!;
            }
        }

        protected MasterUser MasterUser
        {
            get
            {
                var user = HttpContext.Items[nameof(Data.Models.MasterUser)];

                if (user is not Data.Models.MasterUser)
                {
                    throw new InvalidOperationException();
                }

                return (MasterUser)user;
            }
        }

        protected StaffUser StaffUser
        {
            get
            {
                var user = HttpContext.Items[nameof(Data.Models.StaffUser)];

                if (user is not Data.Models.StaffUser)
                {
                    throw new InvalidOperationException();
                }

                return (StaffUser)user;
            }
        }
    }

    [ClientAuthorize]
    public class MasterController : AppController
    {
        protected string UserId
        {
            get
            {
                var userId = User.FindFirstValue(AppClaimType.Identity.UserIdClaimType)
                    ?? throw new InvalidOperationException();

                return userId;
            }
        }
    }

    [ConsumerAuthorize]
    public class ConsumerController : AppController
    {
        protected Guid UserId
        {
            get
            {
                var userId = User.FindFirstValue(AppClaimType.Identity.UserIdClaimType)
                    ?? throw new InvalidOperationException();

                return Guid.Parse(userId);
            }
        }
    }

    [OrderingAuthorize]
    public class BaseSelfOrderingController : AppController
    {
        protected BillMember BillMember
        {
            get
            {
                var billMember = HttpContext.Items[nameof(Data.Models.BillMember)];

                if (billMember is not Data.Models.BillMember)
                {
                    throw new InvalidOperationException();
                }

                return (BillMember)billMember;
            }
        }
    }

    [Route("current")]
    public class CurrentController : AppController
    {
        [AdminAuthorize]
        [HttpGet("admin/claims")]
        public ActionResult<string> GetAdminClaims()
        {
            return Ok(User.Claims.Select(c => new { c.Type, c.Value }));
        }

        [ClientAuthorize]
        [HttpGet("client/claims")]
        public ActionResult<string> GetClientClaims()
        {
            return Ok(User.Claims.Select(c => new { c.Type, c.Value }));
        }

        [ConsumerAuthorize]
        [HttpGet("consumer/claims")]
        public ActionResult<string> GetConsumerClaims()
        {
            return Ok(User.Claims.Select(c => new { c.Type, c.Value }));
        }
    }
}