using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using MvcApi1.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net;
using Newtonsoft.Json;

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
        public async Task<List<Product>> GetProducts(string id)
        {
            if (id == null)
            {
                return await _context.Products.ToListAsync();
            }
            return await _context.Products.Where(e => e.Id == id).ToListAsync();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct(Product product)
        {
            if (product != null)
            {
            }
            return new EmptyResult();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProductAsString(string product)
        {
            bool result = false;
            if (product != null)
            {
                var model = JsonConvert.DeserializeObject<Product>(product);
                var removed = _context.Products.Remove(model);
                result = (removed.State == EntityState.Deleted);
                if (result)
                {
                    await _context.SaveChangesAsync();
                    return new StatusCodeResult(200);
                }
            }
            return new StatusCodeResult(404);
            //return new EmptyResult();
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
