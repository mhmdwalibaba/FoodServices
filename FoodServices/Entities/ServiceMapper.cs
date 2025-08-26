

using MRKHServices.Persistence.Entites;

namespace FoodServices.Entities
{
	public static class ServiceMapper
	{
		public static Service ToEntity(this ServiceDto dto, string mealName, string menuName,int serviceTypeID)
		{
			return new Service
			{
				ServiceTypeID = serviceTypeID, 
				ServiceName = dto.Description,   
				ServiceTitle = menuName,         
				ServiceDesc = mealName, 
				location =mealName,
				ServicePrice = dto.Price,
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
