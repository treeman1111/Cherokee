using System.Collections.Generic;

namespace Cherokee.code.RavenRockParsing.Models
{
    internal sealed class LookupEntry
    {
        public string English { get; set; }

        public List<LookupReference> References { get; set; }
    }
}
