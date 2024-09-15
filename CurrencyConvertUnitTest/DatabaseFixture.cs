using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyConvertUnitTest
{
    public class DatabaseFixture : IDisposable
    {
        public DatabaseFixture()
        {
            // Code to set up resources (e.g., open a database connection)
            Console.WriteLine("DatabaseFixture setup");
        }

        public void Dispose()
        {
            // Code to clean up resources (e.g., close the database connection)
            Console.WriteLine("DatabaseFixture cleanup");
        }
    }
}
