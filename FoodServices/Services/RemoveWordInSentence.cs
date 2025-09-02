using System.Runtime.CompilerServices;

namespace FoodServices.Services
{
	public static class RemoveWordInSentence
	{
		// Extension method on string
		public static string RemoveWord(this string sentence, string word)
		{
			if (string.IsNullOrWhiteSpace(sentence) || string.IsNullOrWhiteSpace(word))
				return sentence;

			string[] words = sentence.Split(' ', StringSplitOptions.RemoveEmptyEntries);

			string result = string.Join(" ",
				words.Where(w => !w.Equals(word, StringComparison.OrdinalIgnoreCase)));

			return result;
		}
	}
}
