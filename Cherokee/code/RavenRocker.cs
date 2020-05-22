using System;
using System.IO;
using System.Linq;

namespace Cherokee.code.verb_ending_patterns
{
    public class RavenRocker
    {
        public static void Main()
        {
            string[] rawLines = File.ReadAllLines("./code/ravenrock.txt");
            string[] hyphenLines = rawLines.Where(l => l == "-----").ToArray();
            string[] headerLines = rawLines.Where(l => l.Contains("] (")).ToArray();

            for (int i = 0; i < headerLines.Length - 1; i++)
            {
                string header = headerLines[i];
                string nextHeader = headerLines[i + 1];
                int headerIdx = GetFirstMatchingIndex(rawLines, header);
                int nextHeaderIdx = GetFirstMatchingIndex(rawLines, nextHeader);

                if (headerIdx == -1) throw new InvalidOperationException();
                if (nextHeaderIdx == -1) throw new InvalidOperationException();

                string[] currentDef = new string[nextHeaderIdx - headerIdx];
                for (int j = headerIdx; j < nextHeaderIdx; j++)
                {
                    currentDef[j - headerIdx] = rawLines[j];
                }
            }
        }

        private static int GetFirstMatchingIndex(string[] haystack, string needle)
        {
            for (int i = 0; i < haystack.Length; i++)
            {
                if (haystack[i] == needle) return i;
            }

            return -1;
        }
    }
}
