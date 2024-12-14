using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSocketServer.Domain.Entities
{
    public class User
    {
        public string Uid { get; set; } = string.Empty;
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string Password { get; set; } // Use only for registration
    }

}
