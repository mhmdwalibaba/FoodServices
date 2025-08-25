using FoodServices.Entities;
using FoodServices.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Identity.Client;
using MRKHServices.Persistence.Entites;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FoodServices.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class FoodController : ControllerBase
	{
		private readonly AppDbContext _db;
		private readonly HttpClient _httpClient;
		public FoodController(AppDbContext dbContext,HttpClient httpClient)
		{
			_db=dbContext;
			_httpClient = httpClient;
		}
		// GET: api/<FoodController>
		[HttpGet]
		public async Task<IActionResult> Get()
		{
			var response = await _httpClient.GetAsync("http://localhost:5287/api/food");
			response.EnsureSuccessStatusCode();

			var json = await response.Content.ReadAsStringAsync();

			var apiResponse = JsonConvert.DeserializeObject<ApiResponseDto>(json);

			if (apiResponse?.Result != null)
			{
				var services = new List<Service>();

				foreach (var meal in apiResponse.Result)
				{
					foreach (var menu in meal.MenuList)
					{
						foreach (var item in menu.MealItems)
						{
							services.Add(item.ToEntity(meal.MealName, menu.Name));
						}
					}
				}

				_db.services.AddRange(services);
				await _db.SaveChangesAsync();
			}
			return Ok();
		}

		
	}
}
