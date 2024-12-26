namespace Autorepair.Services
{
	public class StringUtils
	{
		private static readonly Dictionary<char, char> LatinToCyrillicMap = new Dictionary<char, char>
	{
		{'a', 'а'}, {'A', 'А'},
		{'b', 'б'}, {'B', 'Б'},
		{'e', 'е'}, {'E', 'Е'},
		{'K', 'К'}, {'k', 'к'},
		{'M', 'М'}, {'m', 'м'},
		{'H', 'Н'}, {'h', 'н'},
		{'o', 'о'}, {'O', 'О'},
		{'p', 'р'}, {'P', 'Р'},
		{'c', 'с'}, {'C', 'С'},
		{'y', 'у'}, {'Y', 'У'},
		{'x', 'х'}, {'X', 'Х'}
	};

		private static readonly Dictionary<char, char> CyrillicToLatinMap = LatinToCyrillicMap
			.ToDictionary(pair => pair.Value, pair => pair.Key);

		public static string ConvertToCyrillic(string input)
		{
			return new string(input.Select(c =>
				LatinToCyrillicMap.ContainsKey(c) ? LatinToCyrillicMap[c] : c).ToArray());
		}

		public static string ConvertToLatin(string input)
		{
			return new string(input.Select(c =>
				CyrillicToLatinMap.ContainsKey(c) ? CyrillicToLatinMap[c] : c).ToArray());
		}
	}
}
