using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HideAndSeek
{
    public class SavedGame
    {
        public string CurrentLocation { get; set; } = "";
        public Dictionary<string, string> Opponents { get; set; } = new Dictionary<string, string>();
        public List<string> FoundOpponents { get; set; }
        public int MoveNumber { get; set; }
        public void SaveAGame(GameController controller)
        { 
            CurrentLocation = controller.CurrentLocation.Name;
            MoveNumber = controller.MoveNumber;
            var discouvered =
                from enemy in controller.FoundOpponents
                select enemy.Name;
            FoundOpponents = discouvered.ToList();
            
            foreach(Location location in House.AllHouseLocations)
            {
                if(location is LocationWithHidingPlace)
                {
                    var list = (location as LocationWithHidingPlace).CheckHidingPlace();
                    if(list.Count > 0)
                    {
                        foreach(var item in list)
                        {
                            Opponents.Add(item.Name, location.Name);
                        }
                    }
                }
            }
        }
    }
}
