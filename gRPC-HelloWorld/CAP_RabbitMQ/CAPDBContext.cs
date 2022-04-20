using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAP_RabbitMQ
{
    public class CAPDBContext : DbContext
    {
        public CAPDBContext(DbContextOptions<CAPDBContext> options) : base(options)
        {
        }
    }
}
