using System;
using System.Net;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using NUnit.Framework;
using Perspective.Common;

namespace Perspective.EventStore.IntegrationTests
{
    [TestFixture]
    public class EventStoreRepositoryIntegrationTests
    {
        private static readonly IPEndPoint IntegrationTestTcpEndPoint = new IPEndPoint(IPAddress.Loopback, 1113);

        private static Guid SaveTestAggregateWithoutCustomHeaders(IRepository repository, int numberOfEvents)
        {
            var aggregateToSave = new TestAggregate(Guid.NewGuid());
            aggregateToSave.ProduceEvents(numberOfEvents);
            repository.SaveAsync(aggregateToSave).Wait();
            return aggregateToSave.Id;
        }

        private IEventStoreConnection _connection;
        private EventStoreRepository _sut;

        [SetUp]
        public void SetUp()
        {
            _connection = EventStoreConnection.Create(IntegrationTestTcpEndPoint);
            _connection.ConnectAsync().Wait();
            _sut = new EventStoreRepository(_connection);
        }

        [TearDown]
        public void TearDown()
        {
            _connection.Close();
        }

        [Test]
        public void CanGetLatestVersionById()
        {
            var savedId = SaveTestAggregateWithoutCustomHeaders(_sut, 3000 /* excludes TestAggregateCreated */);

            var retrieved = _sut.GetByIdAsync<TestAggregate>(savedId).Result;
            Assert.AreEqual(3000, retrieved.AppliedEventCount);
        }

        [Test]
        public void CanGetSpecificVersionFromFirstPageById()
        {
            var savedId = SaveTestAggregateWithoutCustomHeaders(_sut, 100 /* excludes TestAggregateCreated */);

            var retrieved = _sut.GetByIdAsync<TestAggregate>(savedId, 65).Result;
            Assert.AreEqual(64, retrieved.AppliedEventCount);
        }

        [Test]
        public void CanGetSpecificVersionFromSubsequentPageById()
        {
            var savedId = SaveTestAggregateWithoutCustomHeaders(_sut, 500 /* excludes TestAggregateCreated */);

            var retrieved = _sut.GetByIdAsync<TestAggregate>(savedId, 126).Result;
            Assert.AreEqual(125, retrieved.AppliedEventCount);
        }

        [Test]
        public void CanHandleLargeNumberOfEventsInOneTransaction()
        {
            const int numberOfEvents = 50000;

            var aggregateId = SaveTestAggregateWithoutCustomHeaders(_sut, numberOfEvents);

            var saved = _sut.GetByIdAsync<TestAggregate>(aggregateId).Result;
            Assert.AreEqual(numberOfEvents, saved.AppliedEventCount);
        }

        [Test]
        public void CanSaveExistingAggregate()
        {
            var savedId = SaveTestAggregateWithoutCustomHeaders(_sut, 100 /* excludes TestAggregateCreated */);

            var firstSaved = _sut.GetByIdAsync<TestAggregate>(savedId).Result;
            firstSaved.ProduceEvents(50);
            _sut.SaveAsync(firstSaved).Wait();

            var secondSaved = _sut.GetByIdAsync<TestAggregate>(savedId).Result;
            Assert.AreEqual(150, secondSaved.AppliedEventCount);
        }

        [Test]
        public void CanSaveMultiplesOfWritePageSize()
        {
            var savedId = SaveTestAggregateWithoutCustomHeaders(_sut, 1500 /* excludes TestAggregateCreated */);
            var saved = _sut.GetByIdAsync<TestAggregate>(savedId).Result;

            Assert.AreEqual(1500, saved.AppliedEventCount);
        }

        [Test]
        public void ClearsEventsFromAggregateOnceCommitted()
        {
            var aggregateToSave = new TestAggregate(Guid.NewGuid());
            aggregateToSave.ProduceEvents(10);
            _sut.SaveAsync(aggregateToSave).Wait();

            Assert.AreEqual(0, ((IAggregate)aggregateToSave).UncommittedEvents.Count);
        }

        [Test]
        public void ThrowsOnRequestingSpecificVersionHigherThanExists()
        {
            var aggregateId = SaveTestAggregateWithoutCustomHeaders(_sut, 10);

            AsyncThrows<AggregateVersionException>(() => _sut.GetByIdAsync<TestAggregate>(aggregateId, 50));
        }

        [Test]
        public void GetsEventsFromCorrectStreams()
        {
            var aggregate1Id = SaveTestAggregateWithoutCustomHeaders(_sut, 100);
            var aggregate2Id = SaveTestAggregateWithoutCustomHeaders(_sut, 50);

            var firstSaved = _sut.GetByIdAsync<TestAggregate>(aggregate1Id).Result;
            Assert.AreEqual(100, firstSaved.AppliedEventCount);

            var secondSaved = _sut.GetByIdAsync<TestAggregate>(aggregate2Id).Result;
            Assert.AreEqual(50, secondSaved.AppliedEventCount);
        }

        [Test]
        public void ThrowsOnGetNonExistentAggregate()
        {
            AsyncThrows<AggregateNotFoundException>(() => _sut.GetByIdAsync<TestAggregate>(Guid.NewGuid()));
        }

        [Test]
        public void ThrowsOnGetDeletedAggregate()
        {
            var aggregateId = SaveTestAggregateWithoutCustomHeaders(_sut, 10);

            var streamName = string.Format("testAggregate-{0}", aggregateId.ToString("N"));
            _connection.DeleteStreamAsync(streamName, 10, true).Wait();

            AsyncThrows<AggregateDeletedException>(() => _sut.GetByIdAsync<TestAggregate>(aggregateId));
        }

        private void AsyncThrows<T>(Func<Task> action) where T : Exception
        {
            Assert.Throws<T>(() =>
            {
                try
                {
                    action().Wait();
                }
                catch (AggregateException e)
                {
                    foreach (var innerException in e.InnerExceptions)
                    {
                        throw innerException;
                    }
                }
            });
        }
    }
}