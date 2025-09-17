namespace FoodServices.Entities
{
	public class ItemDto
	{
		public string? Title { get; set; }
		public int? SectionID { get; set; }
		public string? SectionName { get; set; }	
		public int? StandardTime { get; set; }
		public int? DeadLine { get; set; }
		public string? NameArabic { get; set; }
		public string? NameEnglish { get; set; }
		public bool? Del { get; set; }	
	}
}
