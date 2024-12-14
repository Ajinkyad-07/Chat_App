using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSocketServer.Domain.Entities
{
    public class IncomingMessage
    {
        public string Text { get; set; }
        public string UserId { get; set; } // Target user's ID
        public string UserName { get; set; }
        public DateTime Timestamp { get; set; }
        public string Id { get; set; }
    }
}
