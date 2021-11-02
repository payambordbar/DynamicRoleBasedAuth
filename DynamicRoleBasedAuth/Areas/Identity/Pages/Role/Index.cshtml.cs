using DynamicRoleBasedAuth.Identity;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AppRoles = DynamicRoleBasedAuth.Identity.Roles;

namespace DynamicRoleBasedAuth.Areas.Identity.Pages.Role
{
	[Authorize(Roles =AppRoles.Admin)]
	public class IndexModel : PageModel
	{
		private readonly RoleManager<IdentityRole> _roleManager;

		public IndexModel(RoleManager<IdentityRole> roleManager)
		{
			_roleManager = roleManager;
		}

		public List<IdentityRole>? Roles { get; set; } = default!;

		public async Task OnGetAsync()
		{
			Roles = await _roleManager.Roles
				.Select(x => new IdentityRole { Id = x.Id, Name = x.Name }).ToListAsync();
		}
	}
}
