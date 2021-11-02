using DynamicRoleBasedAuth.Filters;
using DynamicRoleBasedAuth.Identity;
using DynamicRoleBasedAuth.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System.ComponentModel;
using System.Diagnostics;

namespace DynamicRoleBasedAuth.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ActionDetection _actionDetection;

    public HomeController(ILogger<HomeController> logger, ActionDetection actionDetection)
    {
        _logger = logger;
        _actionDetection = actionDetection;
    }

    public IActionResult Index()
    {
       
        return View();
    }

    [DynamicAccess]
    [DisplayName($"custom{nameof(Privacy)}")]
    public IActionResult Privacy()
    {
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
    }
}