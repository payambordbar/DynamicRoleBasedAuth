using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DynamicRoleBasedAuth.TagHelpers
{
    [HtmlTargetElement("*", Attributes = "render-if-has-sub-menu")]
    public class RenderIfHasSubMenuTagHelper : TagHelper
    {
        public override int Order => int.MaxValue;

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var hasSubMenu = (await output.GetChildContentAsync()).GetContent().Contains("class=\"dropdown-item\"");
            if (!hasSubMenu)
                output.SuppressOutput();
        }
    }
}
