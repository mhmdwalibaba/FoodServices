using FoodServices.Entities;
using FoodServices.Model;
using FoodServices.Setting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using MRKHServices.Persistence.Entites;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;


namespace FoodServices.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class FoodController : ControllerBase
	{
		private readonly AppDbContext _db;
		private readonly HttpClient _httpClient;
		private readonly LoginSetting _loginSetting;
		public FoodController(AppDbContext dbContext, HttpClient httpClient, IOptions<LoginSetting> loginSetting)
		{
			_db = dbContext;
			_httpClient = httpClient;
			_loginSetting = loginSetting.Value;
		}
		// GET: api/<FoodController>
		[HttpGet]
		public async Task<IActionResult> Get()
		{
			var address = "";
			int serviceTypeID = 0;
			var serviceType = await _db.serviceTypes.FirstOrDefaultAsync(s => s.ServiceTypeName == "Food" || s.ServiceTypeName == "food");
			if (serviceType == null)
			{
				return NoContent();
			}


			address = serviceType.ServiceTypeAddress;
			serviceTypeID = serviceType.ServiceTypeID;

			if (address == null || address == "")
			{
				return NoContent();
			}
			string token = await GetJwtKey(_loginSetting, _httpClient);
			token = token.Trim();
			var request = new HttpRequestMessage(HttpMethod.Get, address);
			request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

			Console.WriteLine("Request to: " + address);
			Console.WriteLine("Auth Header: " + request.Headers.Authorization);

			var response = await _httpClient.SendAsync(request);

			if (response.IsSuccessStatusCode)
			{
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
								if (menu.Name.Contains("IPTV"))
								{
									services.Add(item.ToEntity(meal.MealName, menu.Name, serviceTypeID));
								}
							}
						}
					}

					_db.services.AddRange(services);
					await _db.SaveChangesAsync();
				}
				return Ok(new { Message = "Services Food Added To Table Service in db OK" });
			}
			else
			{
				var errorContent = await response.Content.ReadAsStringAsync();
				return StatusCode((int)response.StatusCode, new { Message = "token is not valid", Detail = errorContent });
			}

		}



		
	
	    public async Task<string> GetJwtKey(LoginSetting loginsetting, HttpClient client)
		{
			var loginUrl = $"{loginsetting.BaseUrl}?username={loginsetting.UserName}&password={loginsetting.Password}";

			var response = await client.GetAsync(loginUrl);

			if (response.IsSuccessStatusCode)
			{
				var content = await response.Content.ReadAsStringAsync();

				
				var json = JObject.Parse(content);
				var token = json["token"].ToString();

				Console.WriteLine("✅ توکن دریافت شد:");
				Console.WriteLine(token);

				return token;
			}
			return "";
		}
	}
}
