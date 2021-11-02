using DynamicRoleBasedAuth.Data;
using DynamicRoleBasedAuth.Identity;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DynamicRoleBasedAuth.Areas.Identity.Pages.User
{
	[Authorize(Roles = Roles.Admin)]
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public List<UserRole> Users { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Users = await (from user in _context.Users
                           join userRole in _context.Set<IdentityUserRole<string>>()
                           on user.Id equals userRole.UserId
                           into urs
                           from userR in urs.DefaultIfEmpty()
                           join role in _context.Roles
                           on userR.RoleId equals role.Id
                           into userRoles
                           from userRole in userRoles.DefaultIfEmpty()
                           select new UserRole(user.Id, user.UserName, user.Email, userRole.Name)).ToListAsync();
        }
    }

    public record UserRole(string Id, string UserName, string Email, string Role);

}
