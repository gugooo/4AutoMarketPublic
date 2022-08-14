using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _4AutoMarket.Models
{
    public class LiveChatSession
    {
        public LiveChatSession()
        {
            this.messages = new HashSet<LiveChatMessage>();
        }
        public int Id { get; set; }
        public string UserName { get; set; }
        public string OperatorName { get; set; }
        public DateTime Added { get; set; }
        public virtual ICollection<LiveChatMessage> messages { get; set; }
    }
    public class LiveChatMessage
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public bool IsOperator { get; set; }
        public bool IsNew { get; set; }
        public DateTime Sended { get; set; }
    }
}
