namespace FoodServices.Entities
{
	public class ApiResponseDto
	{
		public string Error { get; set; }
		public string Message { get; set; }
		public List<MealDto> Result { get; set; }
	}

	public class MealDto
	{
		public string MealID { get; set; }
		public string MealName { get; set; }
		public List<MenuDto> MenuList { get; set; }
	}

	public class MenuDto
	{
		public string Code { get; set; }
		public string ID { get; set; }
		public string Name { get; set; }
		public List<ServiceDto> MealItems { get; set; }
	}

	public class ServiceDto
	{
		public string Code { get; set; }
		public string Description { get; set; }
		public string ID { get; set; }
		public string? ImageURL { get; set; }
		public int Price { get; set; }
		public int Priority { get; set; }
		public int Remain { get; set; }
	}


}
