using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Canon.Data.Import
{
    public class ImportErrorMessage
    {
        private List<string> _params = new List<string>(10);
        public string Id { get; set; }
        public List<string> Parameters
        { get { return _params; } }

        public ImportErrorMessage()
        {
        }

        public ImportErrorMessage(string id)
        {
            this.Id = id;
        }

        public ImportErrorMessage(string id, List<string> parameters)
        {
            this.Id = id;
            foreach (string p in parameters)
                this.Parameters.Add(p);
        }

        public ImportErrorMessage(string id, string[] parameters)
        {
            this.Id = id;
            foreach (string p in parameters)
                this.Parameters.Add(p);
        }
    }
}
