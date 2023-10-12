using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe.Checkout;
using StripePayment.Data;
using StripePayment.Models;
using StripePayment.Models.Dtos;

namespace StripePayment.Controllers
{
	public class OrderController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<User> _userManager;

		public OrderController(ApplicationDbContext context, UserManager<User> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		[HttpGet]
		public async Task<IActionResult> CheckOut(int id)
		{
			var user = await _userManager.GetUserAsync(User);
			var product = _context.Products.FirstOrDefault(product => product.Id == id);

			var orderDto = new OrderDto
			{
				ProductName = product.Name,
				Price = product.Price,
				ProductId = id,
				UserFirstName = user.FirstName,
				UserLastName = user.LastName,
			};
			return View(orderDto);
		}

		public async Task<IActionResult> CheckOut(OrderDto orderDto)
		{
			var user = await _userManager.GetUserAsync(User);
			var product = _context.Products.FirstOrDefault(product => product.Id == orderDto.ProductId);
			var order = new Order
			{
				CreationDate = DateTime.Now,
				ProductId = orderDto.ProductId,
				UserId = user.Id,
				Quantity = orderDto.Quantity,
				TotalPrice = orderDto.Quantity * product.Price,
				Status = "Pending"
			};
			_context.Orders.Add(order);
			_context.SaveChanges();

			//stripe settings
			var domain = "https://localhost:44339/";
			var options = new SessionCreateOptions
			{
				PaymentMethodTypes = new List<string>
				{
				  "card",
				},
				LineItems = new List<SessionLineItemOptions>(),
				Mode = "payment",
				SuccessUrl = domain + $"Order/Confirm?id={order.Id}",
				CancelUrl = domain + $"Order/Deny?id={order.Id}",
			};

			var sessionLineItem = new SessionLineItemOptions
			{
				PriceData = new SessionLineItemPriceDataOptions
				{
					UnitAmount = (long)(product.Price * 100),//20.00 -> 2000
					Currency = "usd",
					ProductData = new SessionLineItemPriceDataProductDataOptions
					{
						Name = product.Name,
						Description = product.Description,
						Images = new List<string> { product.ImageUrl }
					},
				},
				Quantity = orderDto.Quantity,
			};

			options.LineItems.Add(sessionLineItem);

			var service = new SessionService();
			Session session = service.Create(options);

			order.SessionId = session.Id;
			_context.Orders.Update(order);
			_context.SaveChanges();

			Response.Headers.Add("Location", session.Url);
			return new StatusCodeResult(303);

			/*return RedirectToAction("Index", "Home");*/
		}

		public async Task<IActionResult> Confirm(int id)
		{
			var order = _context.Orders.FirstOrDefault(order => order.Id == id);
			order.Status = "Approved";
			_context.Orders.Update(order);
			_context.SaveChanges();
			return RedirectToAction("Index", "Home");
		}

		public async Task<IActionResult> Deny(int id)
		{
			var order = _context.Orders.FirstOrDefault(order => order.Id == id);
			order.Status = "Denied";
			_context.Orders.Update(order);
			_context.SaveChanges();
			return RedirectToAction("Index", "Home");
		}
	}
}