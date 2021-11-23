using ParserLes.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserLes.DataFolder
{
    class DataBaseContext: DbContext
    {

        public DbSet<JSONData> JSONData { get; set; }
        public DbSet<ParserSettings> parserSettings { get; set; }
        

        public DataBaseContext() : base(@"Server=localhost\SQLEXPRESS;Initial Catalog=Parser;Trusted_Connection=True;") { }
        
    }
}
