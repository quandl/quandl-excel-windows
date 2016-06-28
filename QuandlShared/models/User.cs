using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quandl.Shared.models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string ApiKey { get; set; }
        public string ApiVersion { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ShouldConfirm { get; set; }

        public string FullName => $"{FirstName} {LastName}";
    }
}
