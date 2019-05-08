using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sqldependency.Entity
{
    public class UUMS_Owners
    {
        public Guid OwnerGUID { get; set; }
        public string OwnerName { get; set; }
        public string OwnerEmail { get; set; }
        public int RegistrationType { get; set; }
        public DateTime? RegistrationTime { get; set; }
        public DateTime? CancellationDate { get; set; }
        public string UpdateMan { get; set; }
    }
}
