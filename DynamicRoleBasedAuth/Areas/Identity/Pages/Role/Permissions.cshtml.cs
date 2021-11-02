using DynamicRoleBasedAuth.Data;
using DynamicRoleBasedAuth.Identity;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

using System.Data;
using System.Security.Claims;

namespace DynamicRoleBasedAuth.Areas.Identity.Pages.Role;

[Authorize(Roles = Roles.Admin)]
public class PermissionsModel : PageModel
{
    private readonly ActionDetection _actionDetection;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly AppDbContext _appDbContext;

    public PermissionsModel(ActionDetection actionDetection, RoleManager<IdentityRole> roleManager, AppDbContext appDbContext)
    {
        _actionDetection = actionDetection;
        _roleManager = roleManager;
        _appDbContext = appDbContext;
    }

    public List<Permission> Claims { get; set; } = default!;
    public string Role { get; set; } = default!;

    public async Task OnGetAsync(string roleId)
    {
        var role = await _roleManager.FindByIdAsync(roleId);
        Role = role.Name;
        var claims = await _roleManager.GetClaimsAsync(role);
        Claims = ActionDetection.Actions
            .Select(
            x =>
            new Permission(
                x.Value.DisplayName,
                x.Value.Claim,
                Url.Action(x.Value.Action, x.Value.Controller, new { x.Value.Area })!,
                claims.Any(cl => cl.Value == x.Value.Claim && cl.Type == AppClaimTypes.Permission)))
            .ToList();
    }

    public async Task<IActionResult> OnPostToggleAsync([FromBody] PermissionToggle permission)
    {
        var role = await _roleManager.FindByNameAsync(permission.Role);
        var hasClaim = await _appDbContext.Set<IdentityRoleClaim<string>>()
            .AnyAsync(x => x.RoleId == role.Id && x.ClaimType == AppClaimTypes.Permission && x.ClaimValue == permission.Claim);
        IdentityResult result;
        var claim = new Claim(AppClaimTypes.Permission, permission.Claim);
        if (hasClaim)
        {
            result = await _roleManager.RemoveClaimAsync(role, claim);

        }
        else
        {
            result = await _roleManager.AddClaimAsync(role, claim);
        }
        return result.Succeeded ? new OkResult() : new BadRequestResult();
    }
}

public record Permission(string Name, string Claim, string Path, bool IsGranted);
public record PermissionToggle(string Role, string Claim);
