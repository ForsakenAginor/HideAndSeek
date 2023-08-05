using HideAndSeek;
using System;

namespace HideAndSeekTests
{
    [TestClass]
    public class GameControllerTests
    {
        GameController gameController;
        [TestInitialize]
        public void Initialize()
        {
            gameController = new GameController();
        }
        [TestMethod]
        public void TestMovement()
        {
            Assert.AreEqual("Entry", gameController.CurrentLocation.Name);
            Assert.IsFalse(gameController.Move(Direction.Up));
            Assert.AreEqual("Entry", gameController.CurrentLocation.Name);
            Assert.IsTrue(gameController.Move(Direction.East));
            Assert.AreEqual("Hallway", gameController.CurrentLocation.Name);
            Assert.IsTrue(gameController.Move(Direction.Up));
            Assert.AreEqual("Landing", gameController.CurrentLocation.Name);
            // Add more movement tests to the TestMovement test method
        }
        [TestMethod]
        public void TestParseInput()
        {
            var initialStatus = gameController.Status;
            Assert.AreEqual("That's not a valid direction", gameController.ParseInput("X"));
            Assert.AreEqual(initialStatus, gameController.Status);
            Assert.AreEqual("There's no exit in that direction",
            gameController.ParseInput("Up"));
            Assert.AreEqual(initialStatus, gameController.Status);
            Assert.AreEqual("Moving East", gameController.ParseInput("East"));
           
            Assert.AreEqual("You are in the Hallway. You see the following exits:" +
            Environment.NewLine + " - the Kitchen is to the NorthWest" +
            Environment.NewLine + " - the Entry is to the West" +
            Environment.NewLine + " - the Living Room is to the South" +
            Environment.NewLine + " - the Bathroom is to the North" +
            Environment.NewLine + " - the Landing is Up" +
            Environment.NewLine + "You have found 0 of 5 opponents: ", gameController.Status);

            Assert.AreEqual("Moving South", gameController.ParseInput("South"));
            Assert.AreEqual("You are in the Living Room. You see the following exits:" +
            Environment.NewLine + " - the Hallway is to the North" +
            Environment.NewLine + "Someone could hide on TV" +
            Environment.NewLine + "You have found 0 of 5 opponents: ", gameController.Status);
            // Can you add more input parsing tests to the TestParseInput test method?
        }

        [TestMethod]
        public void TestParseCheck()
        {
            Assert.IsFalse(gameController.GameOver);
            // Clear the hiding places and hide the opponents in specific rooms
            House.ClearHidingPlaces();
            var joe = gameController.Opponents.ToList()[0];
            (House.GetLocationByName("Garage") as LocationWithHidingPlace).Hide(joe);
            var bob = gameController.Opponents.ToList()[1];
            (House.GetLocationByName("Kitchen") as LocationWithHidingPlace).Hide(bob);
            var ana = gameController.Opponents.ToList()[2];
            (House.GetLocationByName("Attic") as LocationWithHidingPlace).Hide(ana);
            var owen = gameController.Opponents.ToList()[3];
            (House.GetLocationByName("Attic") as LocationWithHidingPlace).Hide(owen);
            var jimmy = gameController.Opponents.ToList()[4];
            (House.GetLocationByName("Kitchen") as LocationWithHidingPlace).Hide(jimmy);
            // Check the Entry -- there are no players hiding there
            Assert.AreEqual(1, gameController.MoveNumber);
            Assert.AreEqual("There is no hiding place in the Entry",
            gameController.ParseInput("Check"));
            Assert.AreEqual(2, gameController.MoveNumber);
            // Move to the Garage
            gameController.ParseInput("Out");
            Assert.AreEqual(3, gameController.MoveNumber);
            // We hid Joe in the Garage, so validate ParseInput's return value and the properties
            Assert.AreEqual("You found 1 opponent hiding in a car trunk",
            gameController.ParseInput("check"));

            
            Assert.AreEqual("You are in the Garage. You see the following exits:" +
            Environment.NewLine + " - the Entry is In" +
            Environment.NewLine + "Someone could hide in a car trunk" +
            Environment.NewLine + "You have found 1 of 5 opponents: Joe",
            gameController.Status);
            
            Assert.AreEqual("4: Which direction do you want to go (or type 'check'): ",
            gameController.Prompt);
            Assert.AreEqual(4, gameController.MoveNumber);
            
            // Move to the bathroom, where nobody is hiding
            gameController.ParseInput("In");
            gameController.ParseInput("East");
            gameController.ParseInput("North");
            // Check the Bathroom to make sure nobody is hiding there
            Assert.AreEqual("Nobody was hiding in the toilet cistern", gameController.ParseInput("check"));
            Assert.AreEqual(8, gameController.MoveNumber);
            
            gameController.ParseInput("South");
            gameController.ParseInput("NorthWest");
            Assert.AreEqual("You found 2 opponents hiding in the oven",
            gameController.ParseInput("check"));
            Assert.AreEqual("You are in the Kitchen. You see the following exits:" +
            Environment.NewLine + " - the Hallway is to the SouthEast" +
            Environment.NewLine + "Someone could hide in the oven" +
            Environment.NewLine + "You have found 3 of 5 opponents: Joe, Bob, Jimmy",
            gameController.Status);
            Assert.AreEqual("11: Which direction do you want to go (or type 'check'): ",
            gameController.Prompt);
            Assert.AreEqual(11, gameController.MoveNumber);
            Assert.IsFalse(gameController.GameOver);
            
            // Head up to the Landing, then check the Pantry (nobody's hiding there)
            gameController.ParseInput("SouthEast");
            gameController.ParseInput("Up");
            Assert.AreEqual(13, gameController.MoveNumber);
            gameController.ParseInput("South");
            Assert.AreEqual("Nobody was hiding in a glass jar",
            gameController.ParseInput("check"));
            Assert.AreEqual(15, gameController.MoveNumber);
            // Check the Attic to find the last two opponents, make sure the game is over
            gameController.ParseInput("North");
            gameController.ParseInput("Up");
            Assert.AreEqual(17, gameController.MoveNumber);
            Assert.AreEqual("You found 2 opponents hiding on the roof",
            gameController.ParseInput("check"));
            Assert.AreEqual("You are in the Attic. You see the following exits:" +
            Environment.NewLine + " - the Landing is Down" +
            Environment.NewLine + "Someone could hide on the roof" +
            Environment.NewLine +
            "You have found 5 of 5 opponents: Joe, Bob, Jimmy, Ana, Owen",
            gameController.Status);
            Assert.AreEqual("18: Which direction do you want to go (or type 'check'): ",
            gameController.Prompt);
            Assert.AreEqual(18, gameController.MoveNumber);
            Assert.IsTrue(gameController.GameOver);
            
        }
        [TestMethod]
        public void GameSaveLoadTest()
        {
            House.ClearHidingPlaces();
            var joe = gameController.Opponents.ToList()[0];
            (House.GetLocationByName("Garage") as LocationWithHidingPlace).Hide(joe);
            var bob = gameController.Opponents.ToList()[1];
            (House.GetLocationByName("Kitchen") as LocationWithHidingPlace).Hide(bob);
            var ana = gameController.Opponents.ToList()[2];
            (House.GetLocationByName("Attic") as LocationWithHidingPlace).Hide(ana);
            var owen = gameController.Opponents.ToList()[3];
            (House.GetLocationByName("Attic") as LocationWithHidingPlace).Hide(owen);
            var jimmy = gameController.Opponents.ToList()[4];
            (House.GetLocationByName("Kitchen") as LocationWithHidingPlace).Hide(jimmy);
            // Check the Entry -- there are no players hiding there
            gameController.ParseInput("Check");
            // Move to the Garage
            gameController.ParseInput("Out");
            // We hid Joe in the Garage
            gameController.ParseInput("check");

            Assert.AreEqual("That's not a valid direction", gameController.ParseInput("save333"));
            Assert.AreEqual("Wrong filename: 3d 33", gameController.ParseInput("save 3d 33"));
            Assert.AreEqual("Wrong filename: 3d\n\\\"33", gameController.ParseInput("save 3d\n\\\"33"));
            Assert.AreEqual("Wrong filename: testsave.gg", gameController.ParseInput("save testSave.gg"));

            //Save a game
            Assert.AreEqual("Saved current game to testsave.txt", gameController.ParseInput("save testSave"));

            Assert.AreEqual("Wrong file name: test save", gameController.ParseInput("load test Save"));

            //Load a game 
            Assert.AreEqual("Loaded game from testsave.txt", gameController.ParseInput("load testSave"));
            gameController.ParseInput("In");
            gameController.ParseInput("load testSave");
            Assert.AreEqual(gameController.CurrentLocation.Name, "Garage");
        }
    }
}
