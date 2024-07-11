using GymBackend.Core.Contracts.Workouts;
using GymBackend.Core.Domains.Workouts;
using GymBackend.Service.Workouts;
using NSubstitute;
using NSubstitute.Core.Arguments;
using NSubstitute.Routing;
using System.Diagnostics.CodeAnalysis;

namespace GymBackend.Service.Test.Workouts
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    public class WorkoutTests
    {
        private IWorkoutsStorage storage;
        private WorkoutsService service;
        [SetUp]
        public void SetUp()
        {
            storage = Substitute.For<IWorkoutsStorage>();
            service = new WorkoutsService(storage);
        }

        [Test]
        public async Task GetRoutine_RandomUserId_ReturnsNull()
        {
            //Arrange
            Guid userId = Guid.NewGuid();

            //Act
            var result = await service.GetRoutineAsync(userId);

            //Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetRoutine_ExistingUserId_ReturnsRoutine()
        {
            //Arrange
            Guid userId = Guid.NewGuid();
            DateTime date = DateTime.Now;

            //Act
            storage.GetRoutineAsync(Arg.Is<Guid>(userId), Arg.Any<DateTime>(), Arg.Any<DateTime>()).Returns(new Routine { Date = date });
            storage.GetSetExerciseIdOrderByRoutineIdAsync(Arg.Any<Guid>()).Returns(new List<Set> { new Set { Name = "Test exercise" } });
            var result = await service.GetRoutineAsync(userId);

            //Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ExerciseSets, Has.Count.GreaterThan(0));
            Assert.That(result.ExerciseSets.First().Name, Is.EqualTo("Test exercise"));
        }

        [Test]
        public void AddRoutine_EmptySetList_ThrowsException()
        {
            //Arrange
            Guid userId = Guid.NewGuid();
            var exerciseSets = new List<ExerciseSets>();

            //Act
            var ex = Assert.ThrowsAsync<Exception>(async () => await service.AddRoutineAsync(userId, exerciseSets));

            //Assert
            Assert.That(ex.Message, Is.EqualTo("No exercises to add"));
        }

        [Test]
        public async Task AddRoutine_SetList_StorageCalled()
        {
            //Arrange
            Guid userId = Guid.NewGuid();
            Guid routineId = Guid.NewGuid();
            var exerciseSets = new List<ExerciseSets> 
            { 
                new(Guid.NewGuid(), Guid.NewGuid(), "exercise", 0, new List<SetArray> { new() { Id = 0, SetId = Guid.NewGuid(), Weight = 1, Sets = 1, Reps = 1, Order = 0 } }) 
            };

            //Act
            storage.AddRoutineAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<DateTime>()).Returns(new Routine { Id = routineId });
            var result = await service.AddRoutineAsync(userId, exerciseSets);

            //Assert
            await storage.Received(1).GetRoutineAsync(Arg.Any<Guid>(), Arg.Any<DateTime>(), Arg.Any<DateTime>());
            await storage.Received(1).AddRoutineAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<DateTime>());
            await storage.Received(1).AddExercisesToSetAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<int>());
            await storage.Received(1).AddExerciseSetFromArrayAsync(Arg.Any<Guid>(), Arg.Any<float>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<int>());
            Assert.That(result, Is.EqualTo(routineId));
        }
    }
}
