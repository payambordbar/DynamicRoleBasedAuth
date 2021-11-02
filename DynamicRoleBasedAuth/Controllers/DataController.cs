using DynamicRoleBasedAuth.Filters;

using Microsoft.AspNetCore.Mvc;

using System.ComponentModel;

namespace DynamicRoleBasedAuth.Controllers
{
	[DynamicAccess]
    public class DataController : Controller
    {
        [DisplayName("Index")]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [DisplayName("Create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [DisplayName("Create")]
        public IActionResult Create(object data)
        {
            return View();
        }

        [HttpGet]
        [DisplayName("Edit")]
        public IActionResult Edit()
        {
            return View();
        }

        [HttpPost]
        [DisplayName("Edit")]
        public IActionResult Edit(object data)
        {
            return View();
        }

        [HttpGet]
        [DisplayName("Delete")]
        public IActionResult Delete()
        {
            return View();
        }

        [HttpPost]
        [DisplayName("Delete")]
        public IActionResult Delete(object data)
        {
            return View();
        }
    }
}
