using FoodServices.Entities;

namespace FoodServices.Services
{
	public static class StringExtensions
	{
		public static ServiceParts SplitMenuName(this string menuName, string keyword = "روم", string removeWord = "IPTV")
		{
			if (string.IsNullOrWhiteSpace(menuName))
				return new ServiceParts { ServiceDesc = "", ServiceTitle = "" };

			int idx = menuName.IndexOf(keyword, StringComparison.OrdinalIgnoreCase);

			string desc = "";
			string title = "";

			if (idx >= 0)
			{
				desc = menuName.Substring(0, idx).Trim();
				title = menuName.Substring(idx + keyword.Length).Trim();
			}
			else
			{
				desc = menuName;
			}

			if (!string.IsNullOrWhiteSpace(removeWord))
				title = title.Replace(removeWord, "", StringComparison.OrdinalIgnoreCase).Trim();

			return new ServiceParts
			{
				ServiceDesc = desc,
				ServiceTitle = title
			};
		}
	}
}
