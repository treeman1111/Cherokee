using Cherokee.Extensions;
using Cherokee.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Cherokee.Parsing
{
    public static class RavenRockParser
    {
        public static List<RavenRockEntry> ParseLinesToEntries(string[] lines)
        {
            if (lines == null) throw new ArgumentNullException(nameof(lines));
            if (lines.Length == 0) return new List<RavenRockEntry>(0);

            var startLineRegex = new Regex("\\] \\((?<name>[a-z]+)\\)", RegexOptions.Compiled);
            var foundStartLines = new List<StartLine>();
            for (int i = 0; i < lines.Length; i++)
            {
                var result = startLineRegex.Match(lines[i]);
                if (!result.Success) continue;

                var captured = result.Groups["name"].Value;
                bool parsedToEntryType = Enum.TryParse<EntryType>(captured.ToUpperInvariant(), out var entryType);
                if (!parsedToEntryType) throw new InvalidOperationException($"Unrecognized entry type `{captured}`");

                foundStartLines.Add(new StartLine(entryType, i));
            }

            var parsed = new List<RavenRockEntry>(foundStartLines.Count);
            for (int i = 0; i < foundStartLines.Count; i++)
            {
                StartLine startLine = foundStartLines[i];
                int beginLine = startLine.Line;
                int endLine = (i == foundStartLines.Count - 1) ? lines.Length : foundStartLines[i + 1].Line;
                string mashed = MashLines(lines[beginLine..endLine]);

                parsed.Add(startLine.EntryType switch
                {
                    EntryType.V => ParseToVerbEntry(mashed),
                    EntryType.N => ParseToNounEntry(mashed),
                    EntryType.ADJ => ParseToAdjectiveEntry(mashed),
                    EntryType.PT => new RavenRockNounEntry { English = "PT junk - TODO" }, // TODO
                    _ => throw new NotImplementedException(),
                });
            }


            return parsed;
        }

        private static string MashLines(string[] lines)
        {
            var bilder = new StringBuilder();

            foreach (var line in lines)
            {
                bilder.Append($"{line} ");
            }

            return bilder.ToString().Trim();
        }

        // TODO: add support for entries with two romanized / syllabary forms
        private static RavenRockVerbEntry ParseToVerbEntry(string mashed)
        {
            /*
             * some examples:
             * [
             *    ᎠᎢ [aɂi] (v) “He is walking.”
             *    ᎦᎢ [gaɂi] “I am…”
             *    -----
             *    ᎠᎢᏐᎢ [aɂiso³ɂi] “He often…”
             *    -----
             *    -----
             * ]
             * [
             *    ᎠᎦᏛᎲᏍᎦ [ạkdv³hvsga] (v) “He is investigating it.”
             *    ᎠᎩᎦᏛᎥᏍᎦ [a¹gịkdv³hvsga] “I am…”
             *    ᎤᎦᏛᏅᎢ [ukdv³hnv²³ɂi] “He did…”
             *    ᎠᎦᏛᎲᏍᎪᎢ [ạkdv³hvsgo³ɂi] “He often…”
             *    ᎯᎦᏛᎲᎦ [hịkdv³hvga] “Let you…”
             *    ᎤᎦᏛᏗ [ukdv³hdi] “For him…”
             * ]
             * things to note:
             *   - not all verbs are of the form `he is <doing> it.` some are `it is <doing>` &c
             *   - the different entries are terminated by a `”`
             *   - some verbs are missing entries (due to ignorance or to strange meaning if they were defined. all of these entries are marked by `-----`
             *   - parsing becomes a lot easier if we can separate each form to its own 'line'
             */
            const string missingVerbEntry = "-----";

            // split using the closing quote which marks the end of a verb line
            var splitByCloseQuotes = mashed.Split('”', StringSplitOptions.RemoveEmptyEntries);
            var forms = new List<string>();

            for (int i = 0; i < splitByCloseQuotes.Length; i++)
            {
                var str = splitByCloseQuotes[i];
                if (!str.Contains(missingVerbEntry)) // easy case - no missing verb entries rolled up
                {
                    forms.Add(str);
                    continue;
                }

                // unroll all the missing verb entries onto their own lines
                while (str.Contains(missingVerbEntry))
                {
                    forms.Add(missingVerbEntry);
                    str = str.ReplaceFirst(missingVerbEntry, string.Empty);
                }

                // push reduced line (of just one verb form) to list as well
                forms.Add(str.Trim());
            }

            static (string, string, string)? ProcessLine(string line)
            {
                if (line == missingVerbEntry) return null;

                var firstOpenQuote = line.IndexOf('“');
                var firstLeftBrack = line.IndexOf('[');
                var firstRightBrack = line.IndexOf(']');

                var english = line[(firstOpenQuote + 1)..];
                var romanized = line[(firstLeftBrack + 1)..firstRightBrack];
                var syllabary = line[..firstLeftBrack].TrimEnd();

                return (syllabary, romanized, english);
            }

            var thirdPresent = ProcessLine(forms[0]);
            var firstPresent = ProcessLine(forms[1]);
            var completive = ProcessLine(forms[2]);
            var incompletive = ProcessLine(forms[3]);
            var immediate = ProcessLine(forms[4]);
            var infinitive = ProcessLine(forms[5]);

            return new RavenRockVerbEntry
            {
                English = thirdPresent.HasValue ? thirdPresent.Value.Item3 : null,
                CompletiveRomanized = completive.HasValue ? completive.Value.Item2 : null,
                CompletiveSyllabary = completive.HasValue ? completive.Value.Item1 : null,
                FirstPresentRomanized = firstPresent.HasValue ? firstPresent.Value.Item2 : null,
                FirstPresentSyllabary = firstPresent.HasValue ? firstPresent.Value.Item1 : null,
                ImmediateRomanized = immediate.HasValue ? immediate.Value.Item2 : null,
                ImmediateSyllabary = immediate.HasValue ? immediate.Value.Item1 : null,
                IncompletiveRomanized = incompletive.HasValue ? incompletive.Value.Item2 : null,
                IncompletiveSyllabary = incompletive.HasValue ? incompletive.Value.Item1 : null,
                InfinitiveRomanized = infinitive.HasValue ? infinitive.Value.Item2 : null,
                InfinitiveSyllabary = infinitive.HasValue ? infinitive.Value.Item1 : null,
                ThirdPresentRomanized = thirdPresent.HasValue ? thirdPresent.Value.Item2 : null,
                ThirdPresentSyllabary = thirdPresent.HasValue ? thirdPresent.Value.Item1 : null
            };
        }

        private static RavenRockNounEntry ParseToNounEntry(string mashed)
        {
            /*
             * some examples:
             * ᎠᎦᏍᎬᏂᏓ [ạksgṿnida] (n) “① His left. ② Lefthandedness.” ᏥᎦᏍᎬᏂᏓ [tsịksgṿnida] “My …”
             * ᎠᎦᏖᎾ [ạgạtena] (n) “Lace.” 13
             * ᎠᎦᏙᎵ [ạktoli] (n) “His eye.” ᏗᎦᏙᎵ [dịktoli] “His … (more than one)” ᏥᎦᏙᎵ [tsịktoli] “My …” ᏗᏥᎦᏙᎵ [dịtsịktoli] “My … (more than one)” <example sentence>
             * these take a similar form to adjectives, but, particularly for body "parts" have additional information
             * regarding different "persons". that information as well as example sentences are ignored while parsing
             * <SYLLABARY> [ROMANIZED] (n) “ENGLISH” <currently extraneous information>
             *             1         2     3       4
             * ------------ ---------       -------
             * 1: first left brack
             * 2: first right brack
             * 3: first open quote
             * 4: first close quote
             */
            var firstOpenQuote = mashed.IndexOf('“');
            var firstCloseQuote = mashed.IndexOf('”');
            var firstLeftBrack = mashed.IndexOf('[');
            var firstRightBrack = mashed.IndexOf(']');

            var english = mashed[(firstOpenQuote + 1)..firstCloseQuote];
            var romanized = mashed[(firstLeftBrack + 1)..firstRightBrack];
            var syllabary = mashed[..firstLeftBrack].TrimEnd();

            return new RavenRockNounEntry
            {
                English = english,
                Romanized = romanized,
                Syllabary = syllabary
            };
        }

        private static RavenRockAdjectiveEntry ParseToAdjectiveEntry(string mashed)
        {
            /*
             * we assume that once these are mashed they take a form like:
             * ᎪᏍᏗᏳᎵ [gọsdiyuhli] (adj) “Blunt.” <possible example sentence>
             * ignore the example sentence part for now.
             * <SYLLABARY> [ROMANIZED] (adj) “ENGLISH”
             *             1         2       3       4
             * ------------ ---------         -------
             * 1: first left brack
             * 2: first right brack
             * 3: first open quote
             * 4: first close quote
             */
            var firstOpenQuote = mashed.IndexOf('“');
            var firstCloseQuote = mashed.IndexOf('”');
            var firstLeftBrack = mashed.IndexOf('[');
            var firstRightBrack = mashed.IndexOf(']');

            var english = mashed[(firstOpenQuote + 1)..firstCloseQuote];
            var romanized = mashed[(firstLeftBrack + 1)..firstRightBrack];
            var syllabary = mashed[..firstLeftBrack].TrimEnd();

            return new RavenRockAdjectiveEntry
            {
                English = english,
                Romanized = romanized,
                Syllabary = syllabary
            };
        }
    }

    struct StartLine
    {
        public StartLine(EntryType entryType, int line)
        {
            EntryType = entryType;
            Line = line;
        }

        public EntryType EntryType { get; }
        public int Line { get; }
    }

    enum EntryType
    {
        V,
        N,
        ADJ,
        PT,
    }
}
