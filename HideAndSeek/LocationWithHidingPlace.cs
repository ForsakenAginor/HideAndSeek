using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HideAndSeek
{
    public class LocationWithHidingPlace : Location
    {
        public LocationWithHidingPlace(string name, string hidingPlace) : base(name)
        {
            HidingPlace = hidingPlace;
        }
        public string HidingPlace { get; private set; }
        private List<Opponent> opponents = new List<Opponent>();

        public void Hide(Opponent opponent)
        {
            opponents.Add(opponent);
        }

        public List<Opponent> CheckHidingPlace()
        {
            Opponent[] result = new Opponent[opponents.Count];
            opponents.CopyTo(result);
            opponents.Clear();
            return result.ToList();
        }
    }
}
