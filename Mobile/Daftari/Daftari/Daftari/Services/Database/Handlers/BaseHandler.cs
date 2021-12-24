using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Daftari.Models;
using SQLite;
using Environment = System.Environment;
namespace Daftari.Services.Database
{
    public class BaseHandler
    {
        public static readonly string DATABASE_NAME = "daftari.db3";
        public static string DATABASE_PATH = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), DATABASE_NAME);
        readonly SQLiteAsyncConnection _database;

        public BaseHandler()
        {
            _database = new SQLiteAsyncConnection(DATABASE_PATH);
            _database.CreateTableAsync<User>().Wait();
            _database.CreateTableAsync<Customer>().Wait();
        }

        protected SQLiteAsyncConnection Connection
        {
            get
            {
                return _database;
            }
        }
    }
}
