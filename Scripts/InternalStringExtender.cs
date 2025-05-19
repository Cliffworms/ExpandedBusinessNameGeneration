using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

namespace Game.Mods.Helper
{
    public static class InternalStringExtender
    {
        const string keyString = "Key";
        const string valueString = "Value";

        static readonly char[] trimChars = { '\r', '\n' };

        public static void LoadCsvStrings(Mod mod, string filename)
        {
            LocalizedStringDatabase sd = LocalizationSettings.StringDatabase;
            var op = sd.GetTableAsync("Internal_Strings");
            op.WaitForCompletion();
            if (op.IsDone)
            {
                StringTable table = op.Result;

                if (!(table is StringTable stringTable))
                    return;

                var myCsvText = mod.GetAsset<TextAsset>(filename);
                var newData = ParseCsvRows(myCsvText.text);

                // Patch string table from patch data
                foreach(var kvp in newData)
                {
                    StringTableEntry entry = stringTable.GetEntry(kvp.Key);
                    if (entry != null)
                        entry.Value = kvp.Value;
                    else
                        stringTable.AddEntry(kvp.Key, kvp.Value);
                }
            }
        }

        /// <summary>
        /// Parse source CSV data into key/value pairs separated by comma character.
        /// Source CSV file must have only two columns for Key and Value.
        /// </summary>
        /// <param name="csvText">Source CSV data.</param>
        /// <returns>KeyValuePair for each row.</returns>
        private static List<KeyValuePair<string, string>> ParseCsvRows(string csvText)
        {
            // Regex pattern inspired by https://gist.github.com/awwsmm/886ac0ce0cef517ad7092915f708175f
            // but without the exponential behavior
            const string linePattern = "(?:\\n|^)([^\",\\n]*),((?:\"[^\"]*\")+|[^\",\\n]*)";

            // Split source CSV based on regex matches
            List<KeyValuePair<string, string>> rows = (from Match m in
                        Regex.Matches(csvText, linePattern)
                    select new KeyValuePair<string, string>(
                        m.Groups[1].Value.Trim(trimChars),
                        UnescapeCsVvalue(m.Groups[2].Value).Trim(trimChars)
                    )
                ).ToList();

            // Remove first row if it contains "Key" as key and "Value" as value
            // This is the expected header row but doesn't need to be present
            // First row will be accepted if any other key/value pair is present instead
            if (rows.Count > 0 && rows[0].Key == keyString && rows[0].Value == valueString)
                rows.RemoveAt(0);

            return rows;
        }

        private static string UnescapeCsVvalue(string value)
        {
            if (value.Length > 0 && value[0] == '"')
            {
                return value.Substring(1, value.Length - 2)
                    .Replace("\"\"", "\""); // unescape quote marks
            }
            return value;
        }
    }
}