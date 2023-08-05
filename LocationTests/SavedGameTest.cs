using HideAndSeek;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HideAndSeekTests
{
    [TestClass]
    public class SavedGameTest
    {
        GameController gameController;
        [TestInitialize]
        public void Initialize()
        {
            gameController = new GameController();
        }
        [TestMethod]
        public void SaveAGameTest()
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

            SavedGame save = new SavedGame();
            save.SaveAGame(gameController);
            Assert.AreEqual(save.CurrentLocation, "Attic");
            Assert.AreEqual(save.MoveNumber, 17);
            CollectionAssert.AreEqual(new List<string>() {"Joe", "Bob", "Jimmy" }, save.FoundOpponents);
            CollectionAssert.AreEqual(new Dictionary<string, string>() { { "Ana", "Attic" }, { "Owen", "Attic" } },
                save.Opponents);
        }
        [TestMethod]
        public void SerializeTest()
        {
            SavedGame save = new SavedGame();
            save.SaveAGame(gameController);

            var jsonString = JsonSerializer.Serialize(save);
            SavedGame loadedSave = new SavedGame();
            loadedSave = JsonSerializer.Deserialize<SavedGame>(jsonString);

            Assert.AreEqual(save.CurrentLocation, loadedSave.CurrentLocation);
            CollectionAssert.AreEqual(save.Opponents, loadedSave.Opponents);
        }
    }
}
