namespace FoodServices.Services
{
	using FoodServices.Entities;
	using FoodServices.Model;
	using FoodServices.Setting;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.Extensions.Hosting;
	using Microsoft.Extensions.Logging;
	using Microsoft.Extensions.Options;
	using MRKHServices.Persistence.Entites;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;
	using System.Net.Http.Headers;

	public class FoodWorker : BackgroundService
	{
		private readonly ILogger<FoodWorker> _logger;
		private readonly IServiceProvider _services;
		private readonly HttpClient _httpClient;
		private readonly LoginSetting _loginSetting;
		private readonly int _intervalHours;

		public FoodWorker(
			ILogger<FoodWorker> logger,
			IServiceProvider services,
			HttpClient httpClient,
			IOptions<LoginSetting> loginSetting,
			IOptions<ServiceConfig> config)
		{
			_logger = logger;
			_services = services;
			_httpClient = httpClient;
			_loginSetting = loginSetting.Value;
			_intervalHours = config.Value.IntervalHours;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			_logger.LogInformation("🚀 FoodWorker Started");

			while (!stoppingToken.IsCancellationRequested)
			{
				try
				{
					using var scope = _services.CreateScope();
					var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

					await FetchFoodServicesAsync(db);
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "❌ Error while fetching food services");
				}

				_logger.LogInformation($"⏳ Sleeping {_intervalHours} Hours...");
				await Task.Delay(TimeSpan.FromHours(_intervalHours), stoppingToken);
			}
		}

		private async Task FetchFoodServicesAsync(AppDbContext db)
		{
			string token = await GetJwtKey(_loginSetting, _httpClient);
			if (string.IsNullOrEmpty(token)) return;

			var serviceType = await db.serviceTypes.FirstOrDefaultAsync(s =>
				s.ServiceTypeName=="food" || s.ServiceTypeName=="Food");

			if (serviceType == null || string.IsNullOrEmpty(serviceType.ServiceTypeAddress))
			{
				_logger.LogWarning("⚠️ ServiceType 'Food' not found or has no address");
				return;
			}

			var request = new HttpRequestMessage(HttpMethod.Get, serviceType.ServiceTypeAddress);
			request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

			var response = await _httpClient.SendAsync(request);
			if (!response.IsSuccessStatusCode)
			{
				_logger.LogWarning("⚠️ Request failed: {StatusCode}", response.StatusCode);
				return;
			}

			var json = await response.Content.ReadAsStringAsync();
			var apiResponse = JsonConvert.DeserializeObject<ApiResponseDto>(json);

			if (apiResponse?.Result != null)
			{
				var existingServices = await db.services.ToListAsync();
				if (existingServices.Any())
				{
					db.services.RemoveRange(existingServices);
					await db.SaveChangesAsync();
					_logger.LogInformation("🗑️ Existing services cleared from database");
				}
				var services = new List<Service>();
				foreach (var meal in apiResponse.Result)
				{
					foreach (var menu in meal.MenuList)
					{
						foreach (var item in menu.MealItems)
						{
							services.Add(item.ToEntity(meal.MealName, menu.Name, serviceType.ServiceTypeID));
						}
					}
				}

				db.services.AddRange(services);
				await db.SaveChangesAsync();
				_logger.LogInformation("✅ Food services added to database");
			}
		}

		private async Task<string> GetJwtKey(LoginSetting loginsetting, HttpClient client)
		{
			var loginUrl = $"{loginsetting.Url}?username={loginsetting.UserName}&password={loginsetting.Password}";
			var response = await client.GetAsync(loginUrl);

			if (!response.IsSuccessStatusCode) return "";

			var content = await response.Content.ReadAsStringAsync();
			var json = JObject.Parse(content);
			return json["token"]?.ToString() ?? "";
		}
	}

}
