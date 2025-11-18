using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class AuditLog : BaseEntity
    {
        public int AppUserId { get; set; }
        public string Action { get; set; }
        public string TableName { get; set; }
        public LogType Type { get; set; }
        public string? RecordId { get; set; }
        public string OldValues { get; set; }
        public string NewValues { get; set; }
        public string? IpAdress { get; set; }



        public AppUser AppUser { get; set; }

    }
}
