using Cherokee.code.RavenRockParsing.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Cherokee.code.verb_ending_patterns
{
    public class RavenRocker
    {
        public static void Main()
        {
            var rawLookupLines = ReadLookup();
            var headerlessLookupLines =  RemoveHeaderLines(rawLookupLines);
            var parsedLookupLines = ParseLookupLines(headerlessLookupLines);

            var rawRavenRockLines = ReadRavenRock();
        }

        private static string[] ReadLookup()
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string lookupLocation = Path.Combine(currentDirectory, "code", "RavenRockParsing", "lookup.txt");

            return File.ReadAllLines(lookupLocation);
        }

        private static string[] ReadRavenRock()
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string lookupLocation = Path.Combine(currentDirectory, "code", "RavenRockParsing", "ravenrock.txt");

            return File.ReadAllLines(lookupLocation);
        }

        private static string[] RemoveHeaderLines(string[] lines)
        {
            Regex entryLineFilter = new Regex(".*:.*|.*\\).*");

            var possibleHeaders = new List<(int, string)>();
            for (int i = 0; i < lines.Length; i++)
            {
                string currentLine = lines[i];

                // HACK (but it works): the only non-head line found by this is one that starts with summer - some long description
                // which isnt actually a header
                if (!entryLineFilter.IsMatch(currentLine) && !currentLine.StartsWith("Summer"))
                {
                    possibleHeaders.Add((i, currentLine));
                }
            }

            var headerIndices = possibleHeaders.Select(x => x.Item1).ToHashSet();
            return lines.Where((line, idx) => !headerIndices.Contains(idx)).ToArray();
        }

        private static LookupEntry[] ParseLookupLines(string[] headerlessLines)
        {
            // get lines where each parse will finish (all of them end with `xxx)`)
            var endLineRegex = new Regex("[0-9]+\\)$");
            var rawEndLineIndices = new HashSet<int>();
            for (int i = 0; i < headerlessLines.Length; i++)
            {
                if (endLineRegex.IsMatch(headerlessLines[i])) rawEndLineIndices.Add(i);
            }
            var endLineIndices = rawEndLineIndices.ToArray();

            int parsedInsertIdx = 0;
            var parsed = new LookupEntry[endLineIndices.Length]; // number of end lines tells us how many entries we have to parse

            int startLine = 0;

            for (int i = 0; i < endLineIndices.Length; i++)
            {
                int endLine = endLineIndices[i];
                var stringBuilder = new StringBuilder();

                for (int line = startLine; line <= endLine; line++)
                {
                    stringBuilder.Append(headerlessLines[line]);
                    stringBuilder.Append(" ");
                }

                var rawEntry = stringBuilder.ToString().Trim();

                string[] colonSplit = rawEntry.Split(':', StringSplitOptions.RemoveEmptyEntries);
                if (colonSplit.Length != 2)
                {
                    Console.Error.WriteLine($"String with raw entry `{rawEntry}` has unusual : count={colonSplit.Length}");
                    startLine = endLine + 1;
                    continue;
                }

                string english = colonSplit[0];

                // references come after the `:`
                string[] rawReferences = colonSplit[1].Split(',', StringSplitOptions.RemoveEmptyEntries);
                var lookupReferences = new List<LookupReference>(rawReferences.Length);

                foreach (string rawReference in rawReferences)
                {
                    int leftBracketIdx = rawReference.IndexOf('[');
                    int rightBracketIdx = rawReference.IndexOf(']');

                    string syllabary = rawReference.Substring(0, leftBracketIdx).Trim();
                    string romanized = rawReference.Substring(leftBracketIdx + 1, rightBracketIdx - leftBracketIdx).Trim();
                    int page = int.Parse(rawReference
                        .Substring(rightBracketIdx + 1)
                        .Trim(trimChars: new[] { '(', ')' })
                        .Split(' ', StringSplitOptions.RemoveEmptyEntries)[1]
                   );

                    lookupReferences.Add(new LookupReference
                    {
                        Page = page,
                        Romanized = romanized, 
                        Syllabary = syllabary
                    });
                }

                parsed[parsedInsertIdx++] = new LookupEntry
                {
                    English = english,
                    References = lookupReferences
                };

                startLine = endLine + 1; // next start is one after current end
            }

            Console.WriteLine($"Finished parsing lookup lines. Parsed {parsed.Length} of them.");
            return parsed;
        }
    }
}
