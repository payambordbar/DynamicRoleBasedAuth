using DynamicRoleBasedAuth.Identity;

using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DynamicRoleBasedAuth.TagHelpers;

[HtmlTargetElement("*", Attributes = DynamicAccessAttribute)]
public class DynamicAccessTagHelper : TagHelper
{
    private const string DynamicAccessAttribute = "dynamic-access";
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ActionDetection _actionDetection;

    public DynamicAccessTagHelper(IHttpContextAccessor httpContextAccessor, ActionDetection actionDetection)
    {
        _httpContextAccessor = httpContextAccessor;
        _actionDetection = actionDetection;
    }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var attr = output.Attributes.First(x => x.Name == DynamicAccessAttribute);
        output.Attributes.Remove(attr);

        if (_httpContextAccessor.HttpContext!.User.IsInRole(Roles.Admin) || context.TagName is not ("a" or "form")) return;

        string? url = null;
        string method = "GET";
        switch (output.TagName)
        {
            case "form":
                url = (string?)output.Attributes["action"]?.Value ?? _httpContextAccessor.HttpContext.Request.Path;
                method = ((string?)output.Attributes["method"]?.Value)?.ToUpper() ?? "GET";
                break;
            case "a":
                url = (string?)output.Attributes["href"]?.Value;
                break;
            default:
                break;
        }

        ArgumentNullException.ThrowIfNull(url);

        var actionDetail = _actionDetection.ParsePath(url, method);

        var hasAccess = _httpContextAccessor.HttpContext!.User.HasClaim(AppClaimTypes.Permission, actionDetail.Claim);
        if (!hasAccess)
        {
            output.SuppressOutput();
        }
    }
}
