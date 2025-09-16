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
	using System;
	using System.Buffers.Text;
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

					await FetchSectionServicesAsync(db);

					await FetchFoodServicesAsync(db);
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "❌ Error while fetching  services");
				}

				_logger.LogInformation($"⏳ Sleeping {_intervalHours} Hours...");
				await Task.Delay(TimeSpan.FromHours(_intervalHours), stoppingToken);
			}
		}

		private async Task FetchFoodServicesAsync(AppDbContext db)
		{
			//donot use jwt token

			//string token = await GetJwtKey(_loginSetting, _httpClient);
			//if (string.IsNullOrEmpty(token)) return;

			var serviceType = await db.serviceTypes.FirstOrDefaultAsync(s =>
				s.ServiceTypeName=="food" || s.ServiceTypeName=="Food");

			if (serviceType == null || string.IsNullOrEmpty(serviceType.ServiceTypeAddress))
			{
				_logger.LogWarning("⚠️ ServiceType 'Food' not found or has no address");
				return;
			}

		    string urlWithQuery = $"{serviceType.ServiceTypeAddress}?username={_loginSetting.UserName}&password={_loginSetting.Password}";

			var request = new HttpRequestMessage(HttpMethod.Get, urlWithQuery);
			request.Headers.UserAgent.ParseAdd("Mozilla/5.0");

			//request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

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
				var existingServices = await db.services.Where(s=> s.Creator== "ServiceFood").ToListAsync();
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
							if (menu.Name.Contains("IPTV"))
							{
								services.Add(item.ToEntity(meal.MealName, menu.Name, serviceType.ServiceTypeID));

							}
						}
					}
				}

				db.services.AddRange(services);
				await db.SaveChangesAsync();
				_logger.LogInformation("✅ Food services added to database");
			}
		}
		private async Task FetchSectionServicesAsync(AppDbContext db)
		{ 

		

			string urlWithQuery = $"{_loginSetting.BaseUrl}/{_loginSetting.SectionUrl}";

			var request = new HttpRequestMessage(HttpMethod.Get, urlWithQuery);
			request.Headers.UserAgent.ParseAdd("Mozilla/5.0");


			var response = await _httpClient.SendAsync(request);
			if (!response.IsSuccessStatusCode)
			{
				_logger.LogWarning("⚠️ Request failed: {StatusCode}", response.StatusCode);
				return;
			}

			var json = await response.Content.ReadAsStringAsync();
			var sections = JsonConvert.DeserializeObject<List<SectionDto>>(json);

			if (sections != null)
			{
				var existingServiceTypes = await db.serviceTypes.Where(s => s.Creator=="SectionService").ToListAsync();
				if (existingServiceTypes.Any())
				{
					db.serviceTypes.RemoveRange(existingServiceTypes);
					await db.SaveChangesAsync();
					_logger.LogInformation("🗑️ Existing serviceTypes cleared from database");
				}
				var serviceTypes = new List<ServiceType>();
				foreach (var section in sections)
				{
					// فارسی
					if (!string.IsNullOrWhiteSpace(section.Name))
					{
						serviceTypes.Add(new ServiceType
						{
							ServiceTypeLanguageID = "fa",
							ServiceTypeName = section.Name.Trim(),
							ServiceTypeParams = section.Code.ToString(),
							ServiceTypeHasTime = "0",
							IsEnabled = 1,
							IsDeleted = section.Del == true || section.Deleted == true ? 1 : 0,
							Creator = "SectionService",
							Ctime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
							Mtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")

						});
					}

					// عربی
					if (!string.IsNullOrWhiteSpace(section.NameArabic))
					{
						serviceTypes.Add(new ServiceType
						{
							ServiceTypeLanguageID = "ar",
							ServiceTypeName = section.NameArabic.Trim(),
							ServiceTypeParams = section.Code.ToString(),

							ServiceTypeHasTime = "0",
							IsEnabled = 1,
							IsDeleted = section.Del == true || section.Deleted == true ? 1 : 0,
							Creator = "SectionService",
							Ctime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
							Mtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")

						});
					}

					// انگلیسی
					if (!string.IsNullOrWhiteSpace(section.NameEnglish))
					{
						serviceTypes.Add(new ServiceType
						{
							ServiceTypeLanguageID = "en",
							ServiceTypeName = section.NameEnglish.Trim(),
							ServiceTypeParams = section.Code.ToString(),

							ServiceTypeHasTime = "0",
							IsEnabled = 1,
							IsDeleted = section.Del == true || section.Deleted == true ? 1 : 0,
							Creator = "SectionService",
							Ctime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
							Mtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")

						});
					}
				}

				db.serviceTypes.AddRange(serviceTypes);
				await db.SaveChangesAsync();
				_logger.LogInformation("✅ Sections services added to database");
			}
		}


		private async Task<string> GetJwtKey(LoginSetting loginsetting, HttpClient client)
		{
			var loginUrl = $"{loginsetting.BaseUrl}?username={loginsetting.UserName}&password={loginsetting.Password}";
			var response = await client.GetAsync(loginUrl);

			if (!response.IsSuccessStatusCode) return "";

			var content = await response.Content.ReadAsStringAsync();
			var json = JObject.Parse(content);
			return json["token"]?.ToString() ?? "";
		}
	}

}
