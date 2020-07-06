using FocusMark.App.Cli.Models;
using LiteDB;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FocusMark.App.Cli.Data
{
    [TestClass]
    public class JwtTokenRepositoryTests
    {
        [TestMethod]
        [TestCategory("FocusMark.App.Cli.Data")]
        [TestCategory("FocusMark.App.Cli.Data.JwtTokenRepository")]
        [Owner("Johnathon Sullinger")]
        public async Task SaveTokens_InsertsIntoCollection()
        {
            // Arrange
            ILogger<JwtTokenRepository> logger = Mock.Of<ILogger<JwtTokenRepository>>();
            Mock<ILiteCollection<JwtTokens>> tokenCollectionMock = new Mock<ILiteCollection<JwtTokens>>();

            Mock<ILiteDatabase> databaseMock = new Mock<ILiteDatabase>();
            databaseMock.Setup(instance => instance.GetCollection<JwtTokens>(It.IsAny<string>(), BsonAutoId.ObjectId))
                .Returns(tokenCollectionMock.Object);

            IDatabaseFactory databaseFactory = Mock.Of<IDatabaseFactory>(instance => instance.GetDatabase(It.IsAny<string>()) == databaseMock.Object);
            JwtTokenRepository repo = new JwtTokenRepository(databaseFactory, new EphemeralDataProtectionProvider(), logger);
            JwtTokens tokens = new JwtTokens { AccessToken = "access", IdToken = "id", RefreshToken = "refresh" };

            // Act
            await repo.SaveTokens(tokens);

            // Assert
            tokenCollectionMock.Verify(instance => instance.Insert(It.IsAny<JwtTokens>()), Times.Exactly(1));
        }

        [TestMethod]
        [TestCategory("FocusMark.App.Cli.Data")]
        [TestCategory("FocusMark.App.Cli.Data.JwtTokenRepository")]
        [Owner("Johnathon Sullinger")]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task SaveTokens_ThrowsWhenTokenIsNull()
        {
            // Arrange
            ILogger<JwtTokenRepository> logger = Mock.Of<ILogger<JwtTokenRepository>>();
            ILiteCollection<JwtTokens> tokenCollection = Mock.Of<ILiteCollection<JwtTokens>>();
            ILiteDatabase database = Mock.Of<ILiteDatabase>(instance => instance.GetCollection(It.IsAny<string>(), BsonAutoId.ObjectId) == tokenCollection);
            IDatabaseFactory databaseFactory = Mock.Of<IDatabaseFactory>(instance => instance.GetDatabase(It.IsAny<string>()) == database);
            JwtTokenRepository repo = new JwtTokenRepository(databaseFactory, new EphemeralDataProtectionProvider(), logger);

            // Act
            await repo.SaveTokens(null);

            // Assert
            Assert.Fail();
        }

        [TestMethod]
        [TestCategory("FocusMark.App.Cli.Data")]
        [TestCategory("FocusMark.App.Cli.Data.JwtTokenRepository")]
        [Owner("Johnathon Sullinger")]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task SaveTokens_ThrowsWhenAccessTokenIsNull()
        {
            // Arrange
            ILogger<JwtTokenRepository> logger = Mock.Of<ILogger<JwtTokenRepository>>();
            ILiteCollection<JwtTokens> tokenCollection = Mock.Of<ILiteCollection<JwtTokens>>();
            ILiteDatabase database = Mock.Of<ILiteDatabase>(instance => instance.GetCollection(It.IsAny<string>(), BsonAutoId.ObjectId) == tokenCollection);
            IDatabaseFactory databaseFactory = Mock.Of<IDatabaseFactory>(instance => instance.GetDatabase(It.IsAny<string>()) == database);
            JwtTokenRepository repo = new JwtTokenRepository(databaseFactory, new EphemeralDataProtectionProvider(), logger);
            JwtTokens tokens = new JwtTokens { RefreshToken = "refresh", IdToken = "id" };

            // Act
            await repo.SaveTokens(tokens);

            // Assert
            Assert.Fail();
        }

        [TestMethod]
        [TestCategory("FocusMark.App.Cli.Data")]
        [TestCategory("FocusMark.App.Cli.Data.JwtTokenRepository")]
        [Owner("Johnathon Sullinger")]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task SaveTokens_ThrowsWhenIdTokenIsNull()
        {
            // Arrange
            ILogger<JwtTokenRepository> logger = Mock.Of<ILogger<JwtTokenRepository>>();
            ILiteCollection<JwtTokens> tokenCollection = Mock.Of<ILiteCollection<JwtTokens>>();
            ILiteDatabase database = Mock.Of<ILiteDatabase>(instance => instance.GetCollection(It.IsAny<string>(), BsonAutoId.ObjectId) == tokenCollection);
            IDatabaseFactory databaseFactory = Mock.Of<IDatabaseFactory>(instance => instance.GetDatabase(It.IsAny<string>()) == database);
            JwtTokenRepository repo = new JwtTokenRepository(databaseFactory, new EphemeralDataProtectionProvider(), logger);
            JwtTokens tokens = new JwtTokens { RefreshToken = "refresh", AccessToken = "access" };

            // Act
            await repo.SaveTokens(tokens);

            // Assert
            Assert.Fail();
        }

        [TestMethod]
        [TestCategory("FocusMark.App.Cli.Data")]
        [TestCategory("FocusMark.App.Cli.Data.JwtTokenRepository")]
        [Owner("Johnathon Sullinger")]
        [ExpectedException(typeof(ArgumentNullException))]
        public async Task SaveTokens_ThrowsWhenRefreshTokenIsNull()
        {
            // Arrange
            ILogger<JwtTokenRepository> logger = Mock.Of<ILogger<JwtTokenRepository>>();
            ILiteCollection<JwtTokens> tokenCollection = Mock.Of<ILiteCollection<JwtTokens>>();
            ILiteDatabase database = Mock.Of<ILiteDatabase>(instance => instance.GetCollection(It.IsAny<string>(), BsonAutoId.ObjectId) == tokenCollection);
            IDatabaseFactory databaseFactory = Mock.Of<IDatabaseFactory>(instance => instance.GetDatabase(It.IsAny<string>()) == database);
            JwtTokenRepository repo = new JwtTokenRepository(databaseFactory, new EphemeralDataProtectionProvider(), logger);
            JwtTokens tokens = new JwtTokens { IdToken = "id", AccessToken = "access" };

            // Act
            await repo.SaveTokens(tokens);

            // Assert
            Assert.Fail();
        }
    }
}
