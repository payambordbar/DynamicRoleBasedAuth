using DynamicRoleBasedAuth.Filters;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;

using System.Collections.Concurrent;

namespace DynamicRoleBasedAuth.Identity;

public class ActionDetection
{
    private readonly IActionDescriptorCollectionProvider _actionDescriptor;
    private readonly LinkParser _linkParser;

    /// <summary>
    /// List of ControllerActions with DynamicAccess attribute
    /// </summary>
    public static ConcurrentDictionary<string, ActionDetail> Actions { get; private set; } = default!;

    public ActionDetection(IActionDescriptorCollectionProvider actionDescriptor, LinkParser linkParser)
    {
        _actionDescriptor = actionDescriptor;
        _linkParser = linkParser;
        GetAvailableActions();
    }

    private void GetAvailableActions()
    {
        Actions ??= new ConcurrentDictionary<string, ActionDetail>(_actionDescriptor.ActionDescriptors.Items
            .OfType<ControllerActionDescriptor>()
            .Where(a => a.EndpointMetadata.All(x => x.GetType() != typeof(AllowAnonymousAttribute))
            && a.EndpointMetadata.Any(x => x.GetType() == typeof(DynamicAccessAttribute)))
            .Select(a => new ActionDetail(a))
            .ToDictionary(x => x.Claim));
    }

    /// <summary>
    /// Parse Url to areas and default endpoints
    /// </summary>
    /// <param name="url">Url</param>
    /// <param name="method">Http Method</param>
    /// <returns>ActionDetail</returns>
    public ActionDetail ParsePath(string url, string method = "GET")
    {
        ArgumentNullException.ThrowIfNull(nameof(url));

        var routeValues = _linkParser.ParsePathByEndpointName("areas", url);
        routeValues ??= _linkParser.ParsePathByEndpointName("default", url);

        return method is "GET" ? new ActionDetail(routeValues) : new ActionDetail(routeValues) with { Method = method };
    }
}
