namespace HideAndSeekTests
{
    using HideAndSeek;
    using System.Collections.Generic;
    using System.Linq;

    [TestClass]
    public class LocationTests
    {
        private Location center;
        /// <summary>
        /// Initializes each unit test by setting creating a new the center location
        /// and adding a room in each direction before the test
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            // You'll use this to create a bunch of locations before each test
            center = new Location("Center Room");
            Assert.AreSame("Center Room", center.ToString());
            Assert.AreEqual(0, center.ExitList.Count());

            center.AddExit(Direction.North, new Location("North Room"));
            center.AddExit(Direction.NorthEast, new Location("NorthEast Room"));
            center.AddExit(Direction.East, new Location("East Room"));
            center.AddExit(Direction.SouthEast, new Location("SouthEast Room"));
            center.AddExit(Direction.South, new Location("South Room"));
            center.AddExit(Direction.SouthWest, new Location("SouthWest Room"));
            center.AddExit(Direction.West, new Location("West Room"));
            center.AddExit(Direction.NorthWest, new Location("NorthWest Room"));
            center.AddExit(Direction.Up, new Location("Upper Room"));
            center.AddExit(Direction.Down, new Location("Down Room"));
            center.AddExit(Direction.In, new Location("Inside Room"));
            center.AddExit(Direction.Out, new Location("Outside Room"));

            Assert.AreEqual(12, center.ExitList.Count());
        }
        /// <summary>
        /// Make sure GetExit returns the location in a direction only if it exists
        /// </summary>
        [TestMethod]
        public void TestGetExit()
        {
            // This test will make sure the GetExit method works
            var eastRoom = center.GetExit(Direction.East);
            Assert.AreEqual("East Room", eastRoom.Name);
            Assert.AreSame(center, eastRoom.GetExit(Direction.West));
            Assert.AreSame(eastRoom, eastRoom.GetExit(Direction.Up));
        }
        /// <summary>
        /// Validates that the exit lists are working
        /// </summary>
        [TestMethod]
        public void TestExitList()
        {
            // This test will make sure the ExitList property works
            Assert.AreEqual(center.ExitList.Count(), 12);
            Assert.AreEqual(center.ExitList.First(), "the Outside Room is Out");
            Assert.AreEqual(center.ExitList.TakeLast(1).First(), "the Inside Room is In");
        }
        /// <summary>
        /// Validates that each room’s name and return exit is created correctly
        /// </summary>
        [TestMethod]
        public void TestReturnExits()
        {
            // This test will test navigating through the center Location
            var eastRoom = center.GetExit(Direction.East);
            var testRoom = eastRoom.GetExit(Direction.West);
            var northRoom = testRoom.GetExit(Direction.North);
            var test2Room = northRoom.GetExit(Direction.South);

            Assert.AreSame(eastRoom, testRoom.GetExit(Direction.East));
            Assert.AreEqual(testRoom.Exits.Count(), 12);
            Assert.AreEqual(eastRoom.Exits.Count(), 1);
            Assert.AreEqual(northRoom.Exits.Count(), 1);
            Assert.AreSame(test2Room, center);
        }
        /// <summary>
        /// Add a hall to one of the rooms and make sure the hall room’s names
        /// and return exits are created correctly
        /// </summary>
        [TestMethod]
        public void TestAddHall()
        {
            // This test will add a hallway with two locations and make sure they work
            var eastRoom = center.GetExit(Direction.East);
            eastRoom.AddExit(Direction.East, new Location("Hall"));
            var hall  = eastRoom.GetExit(Direction.East);
            hall.AddExit(Direction.East, new Location("Bath"));

            Assert.AreEqual(hall.Exits.Count(), 2);
            Assert.AreEqual(eastRoom.Exits.Count(), 2);
            Assert.AreEqual(hall.ExitList.First(), "the East Room is to the West");
            Assert.AreSame(hall.GetExit(Direction.Up), hall);
        }
    }
}