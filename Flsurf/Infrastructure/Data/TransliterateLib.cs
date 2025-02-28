namespace Flsurf.Infrastructure.Data
{
    public static class TransliterateLib
    {
        public static string Transliterate(string text)
        {
            Dictionary<char, string> translitTable = new Dictionary<char, string>
        {
            {'а', "a"}, {'б', "b"}, {'в', "v"}, {'г', "g"}, {'д', "d"}, {'е', "e"}, {'ё', "yo"}, {'ж', "zh"},
            {'з', "z"}, {'и', "i"}, {'й', "y"}, {'к', "k"}, {'л', "l"}, {'м', "m"}, {'н', "n"}, {'о', "o"},
            {'п', "p"}, {'р', "r"}, {'с', "s"}, {'т', "t"}, {'у', "u"}, {'ф', "f"}, {'х', "kh"}, {'ц', "ts"},
            {'ч', "ch"}, {'ш', "sh"}, {'щ', "shch"}, {'ъ', ""}, {'ы', "y"}, {'ь', ""}, {'э', "e"}, {'ю', "yu"},
            {'я', "ya"}
        };

            string result = "";

            foreach (char c in text)
            {
                char lowerC = char.ToLower(c);
                if (translitTable.ContainsKey(lowerC))
                {
                    string transChar = translitTable[lowerC];
                    result += char.IsUpper(c) ? char.ToUpper(transChar[0]) + transChar.Substring(1) : transChar;
                }
                else
                {
                    result += c;
                }
            }

            return result;
        }
    }
}
