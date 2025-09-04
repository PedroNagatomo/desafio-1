using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hypesoft.Infrastructure.Configuration
{
    public class MongoDbSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
    }
}