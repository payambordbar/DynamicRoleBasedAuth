using DynamicRoleBasedAuth.Data;
using DynamicRoleBasedAuth.Identity;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

using System.ComponentModel.DataAnnotations;
using System.Data;

using AppRoles = DynamicRoleBasedAuth.Identity.Roles;


namespace DynamicRoleBasedAuth.Areas.Identity.Pages.Role
{
	[Authorize(Roles = AppRoles.Admin)]
    public class CreateModel : PageModel
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _dbContext;

        public CreateModel(RoleManager<IdentityRole> roleManager, AppDbContext dbContext)
        {
            _roleManager = roleManager;
            _dbContext = dbContext;
        }

        [BindProperty]
        public CreateRoleModel Input { get; set; } = default!;
        public List<IdentityRole> Roles { get; set; } = default!;

        public async Task OnGet()
        {
            Roles = await _roleManager.Roles.Select(x => new IdentityRole { Id = x.Id, Name = x.Name }).ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var role = new IdentityRole { Name = Input.Name };

            var result = await _roleManager.CreateAsync(role);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.TryAddModelError(string.Empty, error.Description);
                }
                return Page();
            }
            if (Input.PareentId is not null)
            {
                var baseRole = await _roleManager.FindByIdAsync(Input.PareentId);
                var claims = (await _roleManager.GetClaimsAsync(baseRole))
                    .Where(x => x.Type == AppClaimTypes.Permission)
                    .Select(x => new IdentityRoleClaim<string> { ClaimType = AppClaimTypes.Permission, ClaimValue = x.Value, RoleId = role.Id })
                    .ToArray();
                await _dbContext.Set<IdentityRoleClaim<string>>().AddRangeAsync(claims);
                await _dbContext.SaveChangesAsync();
            }

            return RedirectToPage("Permissions", new {roleId = role.Id});
        }
    }
    public class CreateRoleModel
    {
        [Required]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "{0} name must be between {2} and {1} charecters")]
        public string Name { get; set; } = string.Empty;
        public string? PareentId { get; set; }
    }
}
