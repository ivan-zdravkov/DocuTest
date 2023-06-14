using DocuTest.Application.Interfaces;
using DocuTest.Application.Services;
using DocuTest.Data.Main.DAL.Interfaces;
using DocuTest.Shared.Models;
using Moq;
using NUnit.Framework;
using System.Data;

namespace DocuTest.Tests.Unit.Services
{
    [TestFixture]
    public class DocumentServiceTests
    {
        private readonly Guid userId = new Guid("d9aefc6d-1a69-4548-82c2-39c087e7c739");
        private readonly Guid typeId = new Guid("0d66cfc7-4ac7-473e-8256-63e20f7444cd");

        private readonly Guid d1Id = new Guid("c1eef796-af0c-41ce-8c31-4ecd7b1ca3f4");
        private readonly Guid d2Id = new Guid("b4d1fb42-82b0-4fd1-8066-14b6ff8acc43");
        private readonly Guid d1f1Id = new Guid("027ccb5d-d5fe-44fb-8038-17a2bb537cf4");
        private readonly Guid d1f2Id = new Guid("6b259824-14cf-41ad-a481-008a1ac50b2c");
        private readonly Guid d2f1Id = new Guid("d50b2e9b-d017-4719-b929-de89f347c69f");

        private Metadata d1f1m1;
        private Metadata d1f1m2;
        private Metadata d1f2m1;
        private Metadata d1f2m2;
        private Metadata d2f1m1;

        private Shared.Models.File d1f1;
        private Shared.Models.File d1f2;
        private Shared.Models.File d2f1;
        private Document d1;
        private Document d2;

        private Mock<IDbConnection> connectionMock = new Mock<IDbConnection>();
        private Mock<IDbTransaction> transactionMock = new Mock<IDbTransaction>();

        private Mock<IDocumentReadStrategy> documentReadStrategyMock = new Mock<IDocumentReadStrategy>();
        private Mock<IDocumentWriteStrategy> documentWriteStrategyMock = new Mock<IDocumentWriteStrategy>();

        private Mock<IDbConnectionFactory> connectionFactoryMock = new Mock<IDbConnectionFactory>();
        private Mock<IDocumentRepository> documentRepositoryMock = new Mock<IDocumentRepository>();
        private Mock<IFileRepository> fileRepositoryMock = new Mock<IFileRepository>();
        private Mock<IMetadataRepository> metadataRepositoryMock = new Mock<IMetadataRepository>();

        [SetUp]
        public void Setup()
        {
            d1f1m1 = new Metadata() { FileId = d1f1Id, Key = "1", Value = "A" };
            d1f1m2 = new Metadata() { FileId = d1f1Id, Key = "2", Value = "B" };
            d1f2m1 = new Metadata() { FileId = d1f2Id, Key = "1", Value = "C" };
            d1f2m2 = new Metadata() { FileId = d1f2Id, Key = "2", Value = "D" };
            d2f1m1 = new Metadata() { FileId = d2f1Id, Key = "1", Value = "A" };

            d1f1 = new Shared.Models.File()
            {
                Id = d1f1Id,
                DocumentId = d1Id,
                Name = "Document 1, File 1",
                Extension = "txt",
                Content = new byte[] { },
                Metadata = new List<Metadata>() { d1f1m1, d1f1m2 }
            };

            d1f2 = new Shared.Models.File()
            {
                Id = d1f2Id,
                DocumentId = d1Id,
                Name = "Document 1, File 2",
                Extension = "txt",
                Content = new byte[] { },
                Metadata = new List<Metadata>() { d1f2m1, d1f2m2 }
            };

            d2f1 = new Shared.Models.File()
            {
                Id = d2f1Id,
                DocumentId = d2Id,
                Name = "Document 2, File 1",
                Extension = "txt",
                Content = new byte[] { },
                Metadata = new List<Metadata>() { d2f1m1 }
            };

            d1 = new Document()
            {
                Id = d1Id,
                UserId = userId,
                DocumentTypeId = typeId,
                Name = "Document 1",
                Files = new List<Shared.Models.File>() { d1f1, d1f2 }
            };

            d2 = new Document()
            {
                Id = d2Id,
                UserId = userId,
                DocumentTypeId = typeId,
                Name = "Document 2",
                Files = new List<Shared.Models.File>() { d2f1 }
            };

            this.connectionMock = new Mock<IDbConnection>();
            this.transactionMock = new Mock<IDbTransaction>();

            this.connectionFactoryMock = new Mock<IDbConnectionFactory>();
            this.documentRepositoryMock = new Mock<IDocumentRepository>();
            this.fileRepositoryMock = new Mock<IFileRepository>();
            this.metadataRepositoryMock = new Mock<IMetadataRepository>();

            this.connectionMock.Setup(x => x.BeginTransaction()).Returns(this.transactionMock.Object);
            this.connectionMock.Setup(x => x.Open()).Verifiable();
            this.connectionMock.Setup(x => x.Close()).Verifiable();

            this.transactionMock.Setup(x => x.Connection).Returns(this.connectionMock.Object);
            this.transactionMock.Setup(x => x.Commit()).Verifiable();
            this.transactionMock.Setup(x => x.Rollback()).Verifiable();

            this.connectionFactoryMock.Setup(x => x.Create()).Returns(this.connectionMock.Object);
        }

        [Test]
        public async Task GetSingle_ReturnsFullDocument()
        {
            //Arrange
            this.documentRepositoryMock
                .Setup(x => x.Get(this.connectionMock.Object, d1.Id, this.documentReadStrategyMock.Object, CancellationToken.None))
                .ReturnsAsync(d1);

            this.fileRepositoryMock
                .Setup(x => x.Get(this.connectionMock.Object, new Guid[] { d1.Id }, CancellationToken.None))
                .ReturnsAsync(d1.Files.ToArray());

            this.metadataRepositoryMock
                .Setup(x => x.Get(this.connectionMock.Object, d1.Files.Select(f => f.Id).ToArray(), CancellationToken.None))
                .ReturnsAsync(d1.Files.SelectMany(f => f.Metadata).ToArray());

            DocumentService documentServiceSUT = new DocumentService(
                this.documentReadStrategyMock.Object,
                this.documentWriteStrategyMock.Object,
                this.documentRepositoryMock.Object,
                this.fileRepositoryMock.Object,
                this.metadataRepositoryMock.Object,
                this.connectionFactoryMock.Object
            );

            //Act
            Document result = await documentServiceSUT.Get(d1.Id, CancellationToken.None);

            //Assert
            Assert.That(result.Equals(d1));
        }

        [Test]
        public async Task GetSingle_NotExisting_ThrowsException()
        {
            //Arrange
            DocumentService documentServiceSUT = new DocumentService(
                this.documentReadStrategyMock.Object,
                this.documentWriteStrategyMock.Object,
                this.documentRepositoryMock.Object,
                this.fileRepositoryMock.Object,
                this.metadataRepositoryMock.Object,
                this.connectionFactoryMock.Object
            );

            //Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await documentServiceSUT.Get(d1.Id, CancellationToken.None));
        }

        [Test]
        public async Task GetMultiple_ReturnsFullDocuments()
        {
            //Arrange
            this.documentRepositoryMock
                .Setup(x => x.Get(this.connectionMock.Object, new Guid[] { d1.Id, d2.Id }, this.documentReadStrategyMock.Object, CancellationToken.None))
                .ReturnsAsync(new Document[] { d1, d2 });

            this.fileRepositoryMock
                .Setup(x => x.Get(this.connectionMock.Object, new Guid[] { d1.Id, d2.Id }, CancellationToken.None))
                .ReturnsAsync(d1.Files.Union(d2.Files).ToArray());

            this.metadataRepositoryMock
                .Setup(x => x.Get(this.connectionMock.Object, d1.Files.Union(d2.Files).Select(f => f.Id).ToArray(), CancellationToken.None))
                .ReturnsAsync(d1.Files.Union(d2.Files).SelectMany(f => f.Metadata).ToArray());

            DocumentService documentServiceSUT = new DocumentService(
                this.documentReadStrategyMock.Object,
                this.documentWriteStrategyMock.Object,
                this.documentRepositoryMock.Object,
                this.fileRepositoryMock.Object,
                this.metadataRepositoryMock.Object,
                this.connectionFactoryMock.Object
            );

            //Act
            IEnumerable<Document> result = await documentServiceSUT.Get(new Guid[] { d1.Id, d2.Id }, CancellationToken.None);

            //Assert
            CollectionAssert.AreEquivalent(new Document[] { d1, d2 }, result);
        }

        [Test]
        public async Task GetByMetadata_GetsAllDocuments()
        {
            //Arrange
            this.documentRepositoryMock
                .Setup(x => x.Get(this.connectionMock.Object, new Guid[] { d1.Id, d2.Id }, this.documentReadStrategyMock.Object, CancellationToken.None))
                .ReturnsAsync(new Document[] { d1, d2 });

            this.fileRepositoryMock
                .Setup(x => x.Get(this.connectionMock.Object, new Guid[] { d1.Id, d2.Id }, CancellationToken.None))
                .ReturnsAsync(d1.Files.Union(d2.Files).ToArray());

            this.fileRepositoryMock
                .Setup(x => x.GetDocumentIds(this.connectionMock.Object, new Guid[] { d1f1.Id, d2f1.Id }, CancellationToken.None))
                .ReturnsAsync(new Guid[] { d1.Id, d2.Id });

            this.metadataRepositoryMock
                .Setup(x => x.GetFileIds(this.connectionMock.Object, "1", "A", CancellationToken.None))
                .ReturnsAsync(new Guid[] { d1f1.Id, d2f1.Id });

            this.metadataRepositoryMock
                .Setup(x => x.Get(this.connectionMock.Object, d1.Files.Union(d2.Files).Select(f => f.Id).ToArray(), CancellationToken.None))
                .ReturnsAsync(d1.Files.Union(d2.Files).SelectMany(f => f.Metadata).ToArray());

            DocumentService documentServiceSUT = new DocumentService(
                this.documentReadStrategyMock.Object,
                this.documentWriteStrategyMock.Object,
                this.documentRepositoryMock.Object,
                this.fileRepositoryMock.Object,
                this.metadataRepositoryMock.Object,
                this.connectionFactoryMock.Object
            );

            //Act
            IEnumerable<Document> result = await documentServiceSUT.GetByMetadata("1", "A", CancellationToken.None);

            //Assert
            CollectionAssert.AreEquivalent(new Document[] { d1, d2 }, result);
        }

        [Test]
        public async Task InsertDocument_CommitsTransaction_ReturnsDocumentId()
        {
            //Arrange
            this.documentRepositoryMock
                .Setup(x => x.Insert(this.transactionMock.Object, d1, this.documentWriteStrategyMock.Object, CancellationToken.None))
                .ReturnsAsync(d1.Id);

            this.fileRepositoryMock
                .Setup(x => x.Insert(this.transactionMock.Object, d1f1, CancellationToken.None))
                .ReturnsAsync(d1f1.Id);

            this.fileRepositoryMock
                .Setup(x => x.Insert(this.transactionMock.Object, d1f2, CancellationToken.None))
                .ReturnsAsync(d1f2.Id);

            this.metadataRepositoryMock
                .Setup(x => x.Insert(this.transactionMock.Object, d1.Files.SelectMany(f => f.Metadata).ToArray(), CancellationToken.None))
                .Verifiable();

            DocumentService documentServiceSUT = new DocumentService(
                this.documentReadStrategyMock.Object,
                this.documentWriteStrategyMock.Object,
                this.documentRepositoryMock.Object,
                this.fileRepositoryMock.Object,
                this.metadataRepositoryMock.Object,
                this.connectionFactoryMock.Object
            );

            //Act
            Guid result = await documentServiceSUT.Insert(d1, CancellationToken.None);

            //Assert
            this.documentRepositoryMock.Verify(x => x.Insert(this.transactionMock.Object, It.IsAny<Document>(), this.documentWriteStrategyMock.Object, CancellationToken.None), Times.Once);
            this.fileRepositoryMock.Verify(x => x.Insert(this.transactionMock.Object, It.IsAny<Shared.Models.File>(), CancellationToken.None), Times.Exactly(2));
            this.metadataRepositoryMock.Verify(x => x.Insert(this.transactionMock.Object, It.Is<IEnumerable<Metadata>>(x => x.Count() == 2), CancellationToken.None), Times.Exactly(2));

            this.transactionMock.Verify(x => x.Commit(), Times.Once);

            Assert.That(result.Equals(d1.Id));
        }

        [Test]
        public async Task InsertDocument_ThrowsException_RollbacksTransaction()
        {
            //Arrange
            this.documentRepositoryMock
                .Setup(x => x.Insert(this.transactionMock.Object, d1, this.documentWriteStrategyMock.Object, CancellationToken.None))
                .Throws<ArgumentException>();

            DocumentService documentServiceSUT = new DocumentService(
                this.documentReadStrategyMock.Object,
                this.documentWriteStrategyMock.Object,
                this.documentRepositoryMock.Object,
                this.fileRepositoryMock.Object,
                this.metadataRepositoryMock.Object,
                this.connectionFactoryMock.Object
            );

            //Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await documentServiceSUT.Insert(d1, CancellationToken.None));

            transactionMock.Verify(x => x.Rollback(), Times.Once);
        }

        [Test]
        public async Task UpdateDocument_CommitsTransaction()
        {
            //Arrange
            this.documentRepositoryMock
                .Setup(x => x.Get(this.connectionMock.Object, d1.Id, this.documentReadStrategyMock.Object, CancellationToken.None))
                .ReturnsAsync(d1);

            this.fileRepositoryMock
                .Setup(x => x.Get(this.connectionMock.Object, new Guid[] { d1.Id }, CancellationToken.None))
                .ReturnsAsync(d1.Files.ToArray());

            this.metadataRepositoryMock
                .Setup(x => x.Get(this.connectionMock.Object, d1.Files.Select(f => f.Id).ToArray(), CancellationToken.None))
                .ReturnsAsync(d1.Files.SelectMany(f => f.Metadata).ToArray());

            this.documentRepositoryMock
                .Setup(x => x.Update(this.transactionMock.Object, d1, this.documentWriteStrategyMock.Object, CancellationToken.None))
                .Verifiable();

            DocumentService documentServiceSUT = new DocumentService(
                this.documentReadStrategyMock.Object,
                this.documentWriteStrategyMock.Object,
                this.documentRepositoryMock.Object,
                this.fileRepositoryMock.Object,
                this.metadataRepositoryMock.Object,
                this.connectionFactoryMock.Object
            );

            //Act
            await documentServiceSUT.Update(d1, CancellationToken.None);

            //Assert
            this.documentRepositoryMock.Verify(x => x.Update(this.transactionMock.Object, It.Is<Document>(x => x == d1), this.documentWriteStrategyMock.Object, CancellationToken.None), Times.Once);

            this.transactionMock.Verify(x => x.Commit(), Times.Once);
        }

        [Test]
        public void UpdateDocument_NotExisting_ThrowsException()
        {
            //Arrange
            DocumentService documentServiceSUT = new DocumentService(
                this.documentReadStrategyMock.Object,
                this.documentWriteStrategyMock.Object,
                this.documentRepositoryMock.Object,
                this.fileRepositoryMock.Object,
                this.metadataRepositoryMock.Object,
                this.connectionFactoryMock.Object
            );

            //Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await documentServiceSUT.Update(d1, CancellationToken.None));
        }

        [Test]
        public async Task DeleteDocument_CommitsTransaction()
        {
            //Arrange
            this.fileRepositoryMock
                .Setup(x => x.GetFileIds(this.connectionMock.Object, d1.Id, CancellationToken.None))
                .ReturnsAsync(d1.Files.Select(f => f.Id));

            DocumentService documentServiceSUT = new DocumentService(
                this.documentReadStrategyMock.Object,
                this.documentWriteStrategyMock.Object,
                this.documentRepositoryMock.Object,
                this.fileRepositoryMock.Object,
                this.metadataRepositoryMock.Object,
                this.connectionFactoryMock.Object
            );

            //Act
            await documentServiceSUT.Delete(d1.Id, CancellationToken.None);

            //Assert
            this.metadataRepositoryMock.Verify(x => x.Delete(this.transactionMock.Object, It.Is<IEnumerable<Guid>>(x => x.SequenceEqual(d1.Files.Select(f => f.Id))), CancellationToken.None), Times.Once);
            this.fileRepositoryMock.Verify(x => x.Delete(this.transactionMock.Object, It.Is<IEnumerable<Guid>>(x => x.SequenceEqual(d1.Files.Select(f => f.Id))), CancellationToken.None), Times.Once);
            this.documentRepositoryMock.Verify(x => x.Delete(this.transactionMock.Object, It.Is<Guid>(x => x == d1.Id), this.documentWriteStrategyMock.Object, CancellationToken.None), Times.Once);

            this.transactionMock.Verify(x => x.Commit(), Times.Once);
        }

        [Test]
        public void DeleteDocument_ThrowsException_RollbacksTransaction()
        {
            //Arrange
            this.metadataRepositoryMock
                .Setup(x => x.Delete(this.transactionMock.Object, It.IsAny<IEnumerable<Guid>>(), CancellationToken.None))
                .Throws<ArgumentException>();

            DocumentService documentServiceSUT = new DocumentService(
                this.documentReadStrategyMock.Object,
                this.documentWriteStrategyMock.Object,
                this.documentRepositoryMock.Object,
                this.fileRepositoryMock.Object,
                this.metadataRepositoryMock.Object,
                this.connectionFactoryMock.Object
            );

            //Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await documentServiceSUT.Delete(d1.Id, CancellationToken.None));

            transactionMock.Verify(x => x.Rollback(), Times.Once);
        }
    }
}
