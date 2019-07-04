using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExamenNet.Validators
{
    public class ErrorsCollection
    {
        public string Username;
        public int Id;

        public ErrorsCollection()
        {
            ErrorMessages = new List<string>();
        }
        public string Entity { get; set; }
        public List<string> ErrorMessages { get; set; }

    }
}
