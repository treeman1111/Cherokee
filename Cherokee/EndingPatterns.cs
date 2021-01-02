using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Cherokee.code.verb_ending_patterns
{
    public class EndingPatterns
    {
        private static readonly Dictionary<char, string> lookup = new Dictionary<char, string>
        {
            { 'Ꭰ', "a" },
            { 'Ꭱ', "e" },
            { 'Ꭲ', "i" },
            { 'Ꭳ', "o" },
            { 'Ꭴ', "u" },
            { 'Ꭵ', "v" },

            { 'Ꭶ', "ga" },
            { 'Ꭸ', "ge" },
            { 'Ꭹ', "gi" },
            { 'Ꭺ', "go" },
            { 'Ꭻ', "gu" },
            { 'Ꭼ', "gv" },

            { 'Ꮝ', "s" },
            { 'Ꮜ', "sa" },
            { 'Ꮞ', "se" },
            { 'Ꮟ', "si" },
            { 'Ꮠ', "so" },
            { 'Ꮡ', "su" },
            { 'Ꮢ', "sv" },

            { 'Ꮉ', "ma" },
            { 'Ꮊ', "me" },
            { 'Ꮋ', "mi" },
            { 'Ꮌ', "mo" },
            { 'Ꮍ', "mu" },

            { 'Ꮏ', "hna" },
            { 'Ꮎ', "na" },
            { 'Ꮑ', "ne" },
            { 'Ꮒ', "ni" },
            { 'Ꮓ', "no" },
            { 'Ꮔ', "nu" },
            { 'Ꮕ', "nv" },

            { 'Ꮣ', "da" },
            { 'Ꮥ', "de" },
            { 'Ꮧ', "di" },
            { 'Ꮩ', "do" },
            { 'Ꮪ', "du" },
            { 'Ꮫ', "dv" },

            { 'Ꮃ', "la" },
            { 'Ꮄ', "le" },
            { 'Ꮅ', "li" },
            { 'Ꮆ', "lo" },
            { 'Ꮇ', "lu" },
            { 'Ꮈ', "lv" },

            { 'Ꮹ', "wa" },
            { 'Ꮺ', "we" },
            { 'Ꮻ', "wi" },
            { 'Ꮼ', "wo" },
            { 'Ꮽ', "wu" },
            { 'Ꮾ', "wv" },

            { 'Ꮖ', "gwa" },
            { 'Ꮗ', "gwe" },
            { 'Ꮘ', "gwi" },
            { 'Ꮙ', "gwo" },
            { 'Ꮚ', "gwu" },
            { 'Ꮛ', "gwv" },

            { 'Ꮤ', "ta" },
            { 'Ꮦ', "te" },
            { 'Ꮨ', "ti" },

            { 'Ꮿ', "ya" },
            { 'Ᏸ', "ye" },
            { 'Ᏹ', "yi" },
            { 'Ᏺ', "yo" },
            { 'Ᏻ', "yu" },
            { 'Ᏼ', "yv" },

            { 'Ꭽ', "ha" },
            { 'Ꭾ', "he" },
            { 'Ꭿ', "hi" },
            { 'Ꮀ', "ho" },
            { 'Ꮁ', "hu" },
            { 'Ꮂ', "hv" },

            { 'Ꮳ', "tsa" },
            { 'Ꮴ', "tse" },
            { 'Ꮵ', "tsi" },
            { 'Ꮶ', "tso" },
            { 'Ꮷ', "tsu" },
            { 'Ꮸ', "tsv" },

            { 'Ꮭ', "tla" },
            { 'Ꮮ', "tle" },
            { 'Ꮯ', "tli" },
            { 'Ꮰ', "tlo" },
            { 'Ꮱ', "tlu" },
            { 'Ꮲ', "tlv" },
            { 'Ꮬ', "dla" },

            { 'Ꭷ', "ka" }
        };

        public static async Task Main(string[] args)
        {
            var entries = await GetDictionaryEntries();

            // filter out vi and vt entries
            var verbs = new List<JObject>();
            foreach (var entry in entries)
            {
                var typeToken = entry["type"];
                if (typeToken == null)
                {
                    Console.WriteLine($"Found an object without a type: {entry.ToString(Newtonsoft.Json.Formatting.Indented)}");
                    Console.WriteLine();
                    continue;
                }

                // get a list of the type(s) associated with the entry
                var entryTypes = new List<string>(2);
                if (typeToken.Type == JTokenType.String)
                {
                    entryTypes.Add((string)typeToken);
                }
                else if (typeToken.Type == JTokenType.Array)
                {
                    var typeArray = (JArray)typeToken;
                    foreach (var token in typeArray)
                    {
                        entryTypes.Add((string)token);
                    }
                }

                // check to see if the type is a verb
                if (entryTypes.Contains("vi") || entryTypes.Contains("vt"))
                {
                    verbs.Add(entry);
                }
            }

            // map verbs
            var mappedVerbs = new List<RomanizedVerb>(verbs.Count);
            foreach (var verb in verbs)
            {
                if (TryParseToRomanizedVerb(verb, out var romanizedVerb))
                {
                    mappedVerbs.Add(romanizedVerb);
                }
                else
                {
                    Console.WriteLine($"Failed to map: {verb.ToString(Newtonsoft.Json.Formatting.Indented)}");
                }
            }

            // find forms where the completive and present of one verb appear in another's
            var pairs = new HashSet<(RomanizedVerb, RomanizedVerb)>();
            foreach (var verb in mappedVerbs)
            {
                if (verb.ThirdCompletive == null) continue;
                var strippedCompletive = verb.ThirdCompletive.Substring(1);

                // strip the Ꭰ or the Ꭶ off the front of the third present
                string strippedPresent = "";
                if (verb.ThirdPresent.StartsWith("a"))
                {
                    strippedPresent = verb.ThirdPresent.Substring(1);
                }

                if (verb.ThirdPresent.StartsWith("ga"))
                {
                    strippedPresent = verb.ThirdPresent.Substring(2);
                }

                foreach (var test in mappedVerbs)
                {
                    if (test.ThirdCompletive == null || test.ThirdPresent == verb.ThirdPresent) continue;
                    if (test.ThirdCompletive.Contains(strippedCompletive) && test.ThirdPresent.Contains(strippedPresent))
                    {
                        pairs.Add((verb, test));
                    }
                }
            }

            foreach (var pair in pairs)
            {
                Console.WriteLine(pair);
            }
        }

        private static bool TryParseToRomanizedVerb(JObject verbEntry, out RomanizedVerb verb)
        {
            verb = new RomanizedVerb();

            var thirdPersonToken = verbEntry["third_present_syllabary"];
            var firstPersonToken = verbEntry["first_present_syllabary"];
            var thirdCompletiveToken = verbEntry["completive_past_syallabary"]; // TODO: typo in entries...
            var thirdIncompletiveToken = verbEntry["continuous_syllabary"];
            var secondImmediateToken = verbEntry["immediate_syllabary"];
            var thirdInfinitiveToken = verbEntry["infinitive_syllabary"];
            var translationToken = verbEntry["third_present_en"];

            if (thirdPersonToken == null || translationToken == null) return false;

            if (thirdPersonToken.Type == JTokenType.String)
            {
                verb.ThirdPresent = (string)thirdPersonToken;
            }
            else if (thirdPersonToken.Type == JTokenType.Array)
            {
                var array = (JArray)thirdPersonToken;
                verb.ThirdPresent = (string)array[0];
            }

            if (translationToken.Type == JTokenType.String)
            {
                verb.Translation = (string)translationToken;
            }
            else if (translationToken.Type == JTokenType.Array)
            {
                var array = (JArray)translationToken;
                verb.Translation = (string)array[0];
            }

            if (firstPersonToken != null)
            {
                if (firstPersonToken.Type == JTokenType.String)
                {
                    verb.FirstPresent = (string)firstPersonToken;
                }
                else if (firstPersonToken.Type == JTokenType.Array)
                {
                    var array = (JArray)firstPersonToken;
                    verb.FirstPresent = (string)array[0];
                }
            }

            if (thirdCompletiveToken != null)
            {
                if (thirdCompletiveToken.Type == JTokenType.String)
                {
                    verb.ThirdCompletive = (string)thirdCompletiveToken;
                }
                else if (thirdCompletiveToken.Type == JTokenType.Array)
                {
                    var array = (JArray)thirdCompletiveToken;
                    verb.ThirdCompletive = (string)array[0];
                }
            }

            if (thirdIncompletiveToken != null)
            {
                if (thirdIncompletiveToken.Type == JTokenType.String)
                {
                    verb.ThirdIncompletive = (string)thirdIncompletiveToken;
                }
                else if (thirdIncompletiveToken.Type == JTokenType.Array)
                {
                    var array = (JArray)thirdIncompletiveToken;
                    verb.ThirdIncompletive = (string)array[0];
                }
            }

            if (secondImmediateToken != null)
            {
                if (secondImmediateToken.Type == JTokenType.String)
                {
                    verb.SecondImmediate = (string)secondImmediateToken;
                }
                else if (secondImmediateToken.Type == JTokenType.Array)
                {
                    var array = (JArray)secondImmediateToken;
                    verb.SecondImmediate = (string)array[0];
                }
            }

            if (thirdInfinitiveToken != null)
            {
                if (thirdInfinitiveToken.Type == JTokenType.String)
                {
                    verb.ThirdInfinitive = (string)thirdInfinitiveToken;
                }
                else if (thirdInfinitiveToken.Type == JTokenType.Array)
                {
                    var array = (JArray)thirdInfinitiveToken;
                    verb.ThirdInfinitive = (string)array[0];
                }
            }

            if (verb.ThirdPresent != null)
            {
                bool romanized = TryRomanize(verb.ThirdPresent, out string romThirdPres);
                if (romanized)
                {
                    verb.ThirdPresent = romThirdPres;
                }
                else
                {
                    verb.HadInvalidChars = true;
                }
            }

            if (verb.FirstPresent != null)
            {
                if (TryRomanize(verb.FirstPresent, out string romFirstPres))
                {
                    verb.FirstPresent = romFirstPres;
                }
                else
                {
                    verb.HadInvalidChars = true;
                }
            }

            if (verb.ThirdCompletive != null)
            {
                if (TryRomanize(verb.ThirdCompletive, out string romThirdComp))
                {
                    verb.ThirdCompletive = romThirdComp;
                }
                else
                {
                    verb.HadInvalidChars = true;
                }
            }

            if (verb.ThirdIncompletive != null)
            {
                if (TryRomanize(verb.ThirdIncompletive, out string romThirdIncomp))
                {
                    verb.ThirdIncompletive = romThirdIncomp;
                }
                else
                {
                    verb.HadInvalidChars = true;
                }
            }

            if (verb.SecondImmediate != null)
            {
                if (TryRomanize(verb.SecondImmediate, out string romSecondImm))
                {
                    verb.SecondImmediate = romSecondImm;
                }
                else
                {
                    verb.HadInvalidChars = true;
                }
            }

            if (verb.ThirdInfinitive != null)
            {
                if (TryRomanize(verb.ThirdInfinitive, out string thirdInf))
                {
                    verb.ThirdInfinitive = thirdInf;
                }
                else
                {
                    verb.HadInvalidChars = true;
                }
            }

            return true;
        }

        private static bool TryRomanize(string syllabary, out string romanized)
        {
            var builder = new StringBuilder();

            foreach (char c in syllabary)
            {
                if (!lookup.TryGetValue(c, out string roman))
                {
                    romanized = String.Empty;
                    return false;
                }

                builder.Append(roman);
            }

            romanized = builder.ToString();
            return true;
        }

        // for now we just ignore the second entries where they exist
        // on the assumption that endings will be the same between the two
        private class RomanizedVerb
        {
            public string ThirdPresent { get; set; }
            public string FirstPresent { get; set; }
            public string ThirdCompletive { get; set; }
            public string ThirdIncompletive { get; set; }
            public string SecondImmediate { get; set; }
            public string ThirdInfinitive { get; set; }
            public string Translation { get; set; }
            public bool HadInvalidChars { get; set; }

            public override string ToString()
            {
                return $"{ThirdPresent} ({Translation})";
            }
        }

        private static async Task<List<JObject>> GetDictionaryEntries()
        {
            var pages = Directory.GetFiles("cherokee_english_dictionary", "*.json");
            var list = new List<JObject>();

            // read in all of the pages and:
            // 1) convert them to JObjects
            // 2) get the dictionary array from each of them
            // 3) return all the child JObjects within the array
            var jobjectifyTasks = new List<Task<List<JObject>>>(pages.Length);
            foreach (string path in pages)
            {
                jobjectifyTasks.Add(Task.Run(() =>
                {
                    var content = File.ReadAllText(path);
                    var page = JObject.Parse(content);
                    var dictionaryArray = (JArray)page["dictionary"];
                    var dictionaryEntries = new List<JObject>();

                    foreach (var entry in dictionaryArray)
                    {
                        dictionaryEntries.Add((JObject)entry);
                    }

                    return dictionaryEntries;
                }));
            }
            await Task.WhenAll(jobjectifyTasks);

            foreach (var task in jobjectifyTasks)
            {
                foreach (var dictionaryEntry in task.Result)
                {
                    list.Add(dictionaryEntry);
                }
            }

            return list;
        }
    }
}
