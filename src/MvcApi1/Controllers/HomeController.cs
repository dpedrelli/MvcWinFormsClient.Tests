using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using MvcApi1.Data;
using Microsoft.EntityFrameworkCore;

namespace MvcApi1.Controllers
{
    [Authorize(Roles = "Role 1")]
    public class HomeController : Controller
    {
        private MvcApi1DbContext _context;

        public HomeController(MvcApi1DbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetAntiforgeryToken()
        {
            return View("~/Views/Home/AntiforgeryToken.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult TestAntiForgeryToken()
        {
            return View("~/Views/Home/Index.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<List<Product>> GetProducts()
        {
            return await _context.Products.ToListAsync();
            //return View("~/Views/Home/Index.cshtml");
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
