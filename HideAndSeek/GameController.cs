using System.Text.Json;
using System.Text.RegularExpressions;

namespace HideAndSeek
{
    public class GameController
    {
        /// <summary>
        /// The player's current location in the house
        /// </summary>
        public Location CurrentLocation { get; private set; }

        /// <summary>
        /// The number of moves the player has made
        /// </summary>
        public int MoveNumber { get; private set; } = 1;

        /// <summary>
        /// Private list of opponents the player needs to find
        /// </summary>
        public readonly IEnumerable<Opponent> Opponents = new List<Opponent>()
        {
             new Opponent("Joe"),
             new Opponent("Bob"),
             new Opponent("Ana"),
             new Opponent("Owen"),
             new Opponent("Jimmy"),
        };

        /// <summary>
        /// Private list of opponents the player has found so far
        /// </summary>
        private readonly List<Opponent> foundOpponents = new List<Opponent>();
        public List<Opponent> FoundOpponents { get { return foundOpponents; } }

        /// <summary>
        /// Returns true if the game is over
        /// </summary>
        public bool GameOver => Opponents.Count() == foundOpponents.Count();

        /// <summary>
        /// Returns the the current status to show to the player
        /// </summary>
        public string Status
        {
            get
            {
                string result = $"You are in the {CurrentLocation}. You see the following exits:";
                foreach (string part in CurrentLocation.ExitList)
                {
                    result += (Environment.NewLine + " - " + part);
                }
                if(CurrentLocation is LocationWithHidingPlace)
                {
                    var hideout = (LocationWithHidingPlace) CurrentLocation;
                    result += (Environment.NewLine + "Someone could hide " + hideout.HidingPlace);
                }
                string listOfFoundedOpponents = "";
                for(int i = 0; i < foundOpponents.Count; i++)
                {
                    listOfFoundedOpponents += (foundOpponents[i]);
                    if ((i + 1) != foundOpponents.Count)
                        listOfFoundedOpponents += (", ");
                }    
                result += (Environment.NewLine + $"You have found {foundOpponents.Count} of {Opponents.Count()} opponents: {listOfFoundedOpponents}");
                return result;
            }
        }
        /// <summary>
        /// A prompt to display to the player
        /// </summary>
        public string Prompt => $"{MoveNumber}: Which direction do you want to go (or type 'check'): ";
        public GameController()
        {
            House.ClearHidingPlaces();
            foreach (var opponent in Opponents)
                opponent.Hide();

            CurrentLocation = House.Entry;
        }
        /// <summary>
        /// Move to the location in a direction
        /// </summary>
        /// <param name="direction">The direction to move</param>
        /// <returns>True if the player can move in that direction, false oterwise</returns>
        public bool Move(Direction direction)
        {
            if (!CurrentLocation.Exits.ContainsKey(direction))
                return false;
            else
            {
                CurrentLocation = CurrentLocation.GetExit(direction);
                return true;
            }
        }
        /// <summary>
        /// Parses input from the player and updates the status
        /// </summary>
        /// <param name="input">Input to parse</param>
        /// <returns>The results of parsing the input</returns>
        public string ParseInput(string input)
        {
            if (Enum.TryParse(input, out Direction result))
            {
                if (CurrentLocation.Exits.ContainsKey(result))
                {
                    MoveNumber++;
                    Move(result);
                    return $"Moving {result}";
                }
                else
                    return "There's no exit in that direction";
            }
            else if(input.ToLower() == "check")
            {
                if(!(CurrentLocation is LocationWithHidingPlace))
                {
                    MoveNumber++;
                    return $"There is no hiding place in the {CurrentLocation}";
                }
                else
                {
                    MoveNumber++;
                    var hideout = CurrentLocation as LocationWithHidingPlace;
                    var list = hideout.CheckHidingPlace();
                    foundOpponents.AddRange(list);
                    if (list.Count > 0)
                        return $"You found {list.Count} opponent{s(list.Count)} hiding {hideout.HidingPlace}";
                    else
                        return $"Nobody was hiding {hideout.HidingPlace}";
                }
            }
            else if(input.StartsWith("save "))
            {
                Regex regex = new Regex(@"^[a-z0-9]+$");
                
                string file = input.Substring(5).ToLower().Trim();
                if (regex.IsMatch(file)) 
                {

                    file = input.Substring(5).ToLower().Trim() + ".txt";
                    SavedGame save = new SavedGame();
                    save.SaveAGame(this);
                    var jsonString = JsonSerializer.Serialize(save);

                    using (var sw = new StreamWriter(file))
                    {
                        sw.Write(jsonString);
                    }
                    return $"Saved current game to {file}";
                }
                else
                    return $"Wrong filename: {file}";
            }
            else if (input.StartsWith("load "))
            {
                Regex regex = new Regex(@"^[a-z0-9]+$");
                string file = input.Substring(5).ToLower().Trim();
                if ((regex.IsMatch(file)) && (File.Exists(input.Substring(5).ToLower().Trim() + ".txt")))
                {
                    file = input.Substring(5).ToLower().Trim() + ".txt";
                    var jsonString = "";
                    using (var sr = new StreamReader(file))
                    {
                        jsonString = sr.ReadLine();
                    }
                    SavedGame save = JsonSerializer.Deserialize<SavedGame>(jsonString);
                    LoadGame(save);
                    return $"Loaded game from {file}";
                }
                return $"Wrong file name: {file}";

            }
            else
                return "That's not a valid direction";
        }
        private string s(int number)
        {
            if (number == 1)
                return "";
            else
                return "s";
        }
        public void LoadGame(SavedGame save)
        {
            House.ClearHidingPlaces();
            MoveNumber = save.MoveNumber;
            CurrentLocation = House.GetLocationByName(save.CurrentLocation);
            foundOpponents.Clear();
            foreach(string name in save.FoundOpponents)
            {
                foundOpponents.Add(new Opponent(name));
            }
            foreach(string name in save.Opponents.Keys)
            {
                Opponent opponent = new Opponent(name);
                (House.GetLocationByName(save.Opponents[name]) as LocationWithHidingPlace).Hide(opponent);
            }
        }
    }
}
