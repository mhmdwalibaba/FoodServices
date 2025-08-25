using FoodServices.Entities;
using FoodServices.Model;
using MRKHServices.Persistence.Entites;
using Newtonsoft.Json;
using System;

namespace FoodServices.Services
{
	public class FoodService
	{
		private readonly HttpClient _httpClient;
		private readonly AppDbContext _db;

		public FoodService(HttpClient httpClient, AppDbContext db)
		{
			_httpClient = httpClient;
			_db = db;
		}

		public async Task FetchAndSaveServicesAsync()
		{
			var response = await _httpClient.GetAsync("http://localhost:5000/api/menu");
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
		}
	}

}
