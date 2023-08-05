using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HideAndSeek
{
    public class Location
    {
        /// <summary>
        /// The name of this location
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The exits out of this location
        /// </summary>
        public IDictionary<Direction, Location> Exits { get; private set; }
        = new Dictionary<Direction, Location>();

        /// <summary>
        /// The constructor sets the location name
        /// </summary>
        /// <param name="name">Name of the location</param>
        public Location(string name) => Name = name;

        public override string ToString() => Name;

        /// <summary>
        /// Returns a sequence of descriptions of the exits, sorted by direction
        /// </summary>
        public IEnumerable<string> ExitList
        {
            get
            {
                var result =
                    from location in Exits
                    orderby location.Key
                    select ($"the {location.Value} is {DescribeDirection(location.Key)}");
                return result;
            }

        }

        /// <summary>
        /// Adds an exit to this location
        /// </summary>
        /// <param name="direction">Direction of the connecting location</param>
        /// <param name="connectingLocation">Connecting location to add</param>
        public void AddExit(Direction direction, Location connectingLocation)
        {
            if (!Exits.ContainsKey(direction))
            {
                Exits.Add(direction, connectingLocation);
                connectingLocation.AddReturnExit(direction, this);
            }
            else
                throw new NotImplementedException($"Location in {direction} direction is already exist");
        }

        /// <summary>
        /// Gets the exit location in a direction
        /// </summary>
        /// <param name="direction">Direciton of the exit location</param>
        /// <returns>The exit location, or this if there is no exit in that direction</returns>
        public Location GetExit(Direction direction)
        {
            if (Exits.ContainsKey(direction))
            {
                Location location = Exits[direction];
                if (!location.Exits.ContainsKey((Direction)(-(int)direction)))
                    location.AddReturnExit(direction, this);
                return location;
            }
            else
                return this;
        }  

        /// <summary>
        /// Describes a direction (e.g. "in" vs. "to the North")
        /// </summary>
        /// <param name="d">Direction to describe</param>
        /// <returns>string describing the direction</returns>
        private string DescribeDirection(Direction d) => d switch
        {
            Direction.Up => "Up",
            Direction.Down => "Down",
            Direction.In => "In",
            Direction.Out => "Out",
            _ => $"to the {d}",
        };
        private void AddReturnExit(Direction direction, Location connectingLocation) =>
            Exits.Add((Direction)(-(int)direction), connectingLocation);

    }
}
