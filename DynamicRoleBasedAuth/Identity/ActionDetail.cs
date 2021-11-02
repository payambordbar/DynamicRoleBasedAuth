using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Controllers;

using System.ComponentModel;

namespace DynamicRoleBasedAuth.Identity;

public record ActionDetail(string? Area, string Controller, string Action, string Method = "GET", string DisplayName = "_")
{
    public string Claim { get => $"{Area}.{Controller}.{Action}:{Method}".ToUpper(); }

    public ActionDetail(RouteValueDictionary? routeValues)
        : this((string?)routeValues?["area"],
              (string)routeValues?["controller"]!,
              (string)routeValues?["action"]!)
    { }

    public ActionDetail(ControllerActionDescriptor action) : this(
              action.RouteValues["area"],
              action.RouteValues["controller"]!,
              action.RouteValues["action"]!,
              action.ActionConstraints?.OfType<HttpMethodActionConstraint>().FirstOrDefault()?.HttpMethods.First() ?? "GET",
              action.EndpointMetadata.OfType<DisplayNameAttribute>().FirstOrDefault()?.DisplayName ?? "_")
    { }
}
