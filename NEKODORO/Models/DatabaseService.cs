using SQLite;
namespace NEKODORO.Models
{
    public class PomodoroSession
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int MinutesWorked { get; set; }
        public int CoinsEarned { get; set; }
    }

    public class DatabaseService
    {
        private SQLiteAsyncConnection _database;
        async Task Init()
        {
            if (_database is not null)
                return;

            var databasePath = Path.Combine(FileSystem.AppDataDirectory, "NekoData.db3");
            _database = new SQLiteAsyncConnection(databasePath);
            await _database.CreateTableAsync<PomodoroSession>();
        }


    
    

    public async Task AddSessionAsync(int minutes, int coins)
        {
            await Init();
            var session = new PomodoroSession
            {
                Date = DateTime.Now,
                MinutesWorked = minutes,
                CoinsEarned = coins
            };
            await _database.InsertAsync(session);

        }

        public async Task<List<PomodoroSession>> GetSessionsAsync()
        {
            await Init();
            return await _database.Table<PomodoroSession>().ToListAsync();
        }

    }
}
