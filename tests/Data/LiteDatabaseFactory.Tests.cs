using LiteDB;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;

namespace FocusMark.App.Cli.Data
{
    [TestClass]
    public class LiteDatabaseFactoryTests
    {
        private const string databasePrefix = "FocusMark.App.Cli.Data.Tests.Database";

        [TestCleanup]
        public void Cleanup()
        {
            string[] files = Directory.GetFiles(Directory.GetCurrentDirectory())
                .Where(file => file.Contains(databasePrefix))
                .ToArray();
            
            foreach (string file in files)
            {
                File.Delete(file);
            }
        }

        [TestMethod]
        [TestCategory("FocusMark.App.Cli")]
        [TestCategory("FocusMark.App.Cli")]
        [Owner("Johnathon Sullinger")]
        public void GetDatabase_CreatesDatabaseFile()
        {
            // Arrange
            IDatabaseFactory databaseFactory = new LiteDatabaseFactory();
            string databaseName = $"{databasePrefix}-{Guid.NewGuid()}";

            // Act
            using (databaseFactory.GetDatabase(databaseName)) { }
            bool fileExists = File.Exists($"{Directory.GetCurrentDirectory()}\\{databaseName}");

            // Assert
            Assert.IsTrue(fileExists, "Database file was not created.");
        }
    }
}
