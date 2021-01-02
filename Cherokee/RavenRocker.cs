using Cherokee.Parsing;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Threading.Tasks;

namespace Cherokee.code.RavenRockParsing
{
    public static class RavenRocker
    {
        public static async Task Main()
        {
            var entryLinesTask = File.ReadAllLinesAsync(@".\Sources\raven_rock_dictionary\ravenrock.txt");
            var parsed = RavenRockParser.ParseLinesToEntries(await entryLinesTask);
            var serialized = new JObject {{ "dictionary", JArray.FromObject(parsed) }};
            await File.WriteAllTextAsync("ravenrock.json", serialized.ToString());
        }
    }
}
