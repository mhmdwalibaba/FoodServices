

using FoodServices.Services;
using MRKHServices.Persistence.Entites;

namespace FoodServices.Entities
{
	public static class ServiceMapper
	{
		public static Service ToEntity(this ServiceDto dto, string mealName, string menuName,int serviceTypeID)
		{
			var parts = menuName.SplitMenuName();
			return new Service
			{
				ServiceTypeID = serviceTypeID, 
				ServiceName = dto.Description,   
				ServiceTitle = parts.ServiceTitle,         
				ServiceDesc = parts.ServiceDesc, 
				location =mealName,
				ServicePrice = dto.Price/10,
				ServiceImg = dto.ImageURL,

				Creator = "system",
				Ctime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
				Mtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
				IsEnabled = 1,
				IsDeleted = 0
			};
		}
	}
}
