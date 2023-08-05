using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HideAndSeek
{
    public class House
    {
        public static Location Entry { get; private set; }

        static House()
        {
            Entry = new Location("Entry");
            Entry.AddExit(Direction.Out, new LocationWithHidingPlace("Garage", "in a car trunk"));
            Entry.AddExit(Direction.East, new Location("Hallway"));

            var hallway = Entry.GetExit(Direction.East);
            hallway.AddExit(Direction.NorthWest, new LocationWithHidingPlace("Kitchen", "in the oven"));
            hallway.AddExit(Direction.North, new LocationWithHidingPlace("Bathroom", "in the toilet cistern"));
            hallway.AddExit(Direction.South, new LocationWithHidingPlace("Living Room", "on TV"));
            hallway.AddExit(Direction.Up, new Location("Landing"));

            var landing = hallway.GetExit(Direction.Up);
            landing.AddExit(Direction.Up, new LocationWithHidingPlace("Attic", "on the roof"));
            landing.AddExit(Direction.South, new LocationWithHidingPlace("Pantry", "in a glass jar"));
            landing.AddExit(Direction.SouthEast, new LocationWithHidingPlace("Kids Room", "in a sex doll"));
            landing.AddExit(Direction.SouthWest, new LocationWithHidingPlace("Nursery", "behind the window"));
            landing.AddExit(Direction.West, new LocationWithHidingPlace("Second Bathroom", "behind the door"));
            landing.AddExit(Direction.NorthWest, new LocationWithHidingPlace("Master Bedroom", "on the sex swing"));

            var masterBedroom = landing.GetExit(Direction.NorthWest);
            masterBedroom.AddExit(Direction.East, new LocationWithHidingPlace("Master Bath", "under the water"));

            FullfillListOfHouseLocations(Entry);
        }

        private static List<Location> allHouseLocations = new List<Location>();
        public static List<Location> AllHouseLocations
        {
            get { return allHouseLocations; }
        }
        public static Random Random = new Random();

        private static void FullfillListOfHouseLocations(Location location)
        {
            foreach(Location exit in location.Exits.Values)
            {
                if (!allHouseLocations.Contains(exit))
                {
                    allHouseLocations.Add(exit);
                    FullfillListOfHouseLocations(exit);
                }
            }
        }
        public static Location GetLocationByName(string name)
        {
            foreach (Location location in allHouseLocations)
            {
                if (location.Name == name)
                {
                    return location;
                }
            }
            return Entry;
        }
        public static Location RandomExit(Location location)
        {
            int random = House.Random.Next(location.Exits.Count);
            return location.Exits.ElementAt(random).Value;
        }

        public static void ClearHidingPlaces()
        {
            foreach (Location location in allHouseLocations)
            {
                if (location is LocationWithHidingPlace)
                {
                    LocationWithHidingPlace hideout = location as LocationWithHidingPlace;
                    hideout.CheckHidingPlace();
                }
            }
        }
    }
}
