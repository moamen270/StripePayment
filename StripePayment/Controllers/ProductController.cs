using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StripePayment.Data;
using StripePayment.Models;

namespace StripePayment.Controllers
{
	public class ProductController : Controller
	{
		private readonly ApplicationDbContext _context;

		public ProductController(ApplicationDbContext context)
		{
			_context = context;
		}

		[HttpGet("Index")]
		public IActionResult Index()
		{
			var products = _context.Products.ToList();
			return View(products);
		}

		[HttpGet("Create")]
		public IActionResult Create()
		{
			return View();
		}

		[HttpPost("Create")]
		public IActionResult Create(Product product)
		{
			if (!ModelState.IsValid)
				return View(product);
			_context.Products.Add(product);
			_context.SaveChanges();
			return RedirectToAction("Index", "Product");
		}
	}
}