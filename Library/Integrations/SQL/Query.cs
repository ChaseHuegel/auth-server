using System.Collections.Generic;
using System.Linq;
using Swordfish.Library.Extensions;

namespace Swordfish.Library.Integrations.SQL
{
    public class Query
    {
        private List<string> Entries = new List<string>();

        public string Name { get; set; }
        public string Address { get; set; }
        public int Port { get; set; }
        public int Timeout { get; set; }

        private Query AddSimpleParameter(string value)
        {
            Entries.Add(value);
            return this;
        }

        private Query AppendParameter(string value)
        {
            Entries[Entries.Count-1] = Entries[Entries.Count-1] + value;
            return this;
        }

        public QueryResult Execute() => Database.Put(this);
        
        public QueryResult Result() => Database.Get(this);

        public bool HasResult() => Database.Get(this).Table.Rows.Count > 0;

        public Query Select(string value) => AddSimpleParameter($"SELECT {value}");

        public Query From(string value) => AddSimpleParameter($"FROM {value}");

        public Query Where(string value) => AddSimpleParameter($"WHERE {value}");

        public Query Equals(string value) => AppendParameter($"=\'{value}\'");

        public Query And(string value) => AddSimpleParameter($"AND {value}");

        public Query InsertInto(string value) => AddSimpleParameter($"INSERT INTO {value}");

        public Query Columns(params string[] values) => AddSimpleParameter($"({string.Join(',', values)})");

        public Query Values(params string[] values) => AddSimpleParameter($"VALUES ({string.Join(',', values.Select(x => x.Envelope("\'")))})");

        public Query End() => AppendParameter(";");

        public override string ToString()
        {
            return string.Join(" ", Entries);
        }
    }
}
