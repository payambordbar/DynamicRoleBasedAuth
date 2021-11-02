using DynamicRoleBasedAuth.Identity;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DynamicRoleBasedAuth.Areas.Identity.Pages.User;

[Authorize(Roles = Roles.Admin)]
public class ChangeRoleModel : PageModel
{
	private readonly UserManager<IdentityUser> _userManager;
	private readonly RoleManager<IdentityRole> _roleManager;

	public ChangeRoleModel(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
	{
		_userManager = userManager;
		_roleManager = roleManager;
	}

	public IdentityUser SelectedUser { get; set; } = default!;

	public List<SelectListItem> RolesSelect { get; set; } = default!;

	[BindProperty]
	public string Role { get; set; } = default!;

	public async Task OnGetAsync(string userId)
	{
		SelectedUser = await _userManager.FindByIdAsync(userId);
		var role = (await _userManager.GetRolesAsync(SelectedUser)).FirstOrDefault();
		RolesSelect = await _roleManager.Roles.Select(x => new SelectListItem(x.Name, x.Name, x.Name == role)).ToListAsync();
	}

	public async Task<IActionResult> OnPostAsync(string userId)
	{
		var user = await _userManager.FindByIdAsync(userId);
		var oldRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
		if (oldRole is not null)
			await _userManager.RemoveFromRoleAsync(user, oldRole);
		await _userManager.AddToRoleAsync(user, Role);
		return RedirectToPage("Index");
	}
}
