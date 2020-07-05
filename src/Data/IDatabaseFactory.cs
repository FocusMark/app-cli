using LiteDB;

namespace FocusMark.App.Cli.Data
{
    public interface IDatabaseFactory
    {
        ILiteDatabase GetDatabase(string databasePath);
    }
}
