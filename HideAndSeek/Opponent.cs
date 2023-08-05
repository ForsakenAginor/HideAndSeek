using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HideAndSeek
{
    public class Opponent
    {
        public readonly string Name;
        public Opponent(string name) => Name = name;
        public override string ToString() => Name;
        public void Hide()
        {
            var currentRoom = House.Entry;
            int steps = House.Random.Next(10, 50);
            while (steps > 0)
            {
                currentRoom = House.RandomExit(currentRoom);
                System.Diagnostics.Debug.WriteLine($"I'm going to the {currentRoom}");
                steps--;
            }
            while (true)
            {
                if (currentRoom is LocationWithHidingPlace)
                {
                    var hideout = currentRoom as LocationWithHidingPlace;
                    hideout.Hide(this);
                    break;
                }
                else
                    currentRoom = House.RandomExit(currentRoom);
            }
            System.Diagnostics.Debug.WriteLine($"{Name} is hiding " + $"{(currentRoom as LocationWithHidingPlace).HidingPlace} in the {currentRoom.Name}");
        }
    }
}
