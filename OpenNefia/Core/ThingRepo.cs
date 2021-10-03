using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia
{
    public class ThingRepo
    {
        public static ThingRepo Instance = new ThingRepo();

        private Dictionary<string, IThingData> Storage;
        
        public ThingRepo()
        {
            Storage = new Dictionary<string, IThingData>();
        }

        public void Register(IThingData thing)
        {
            // TODO readable modName.ID
            // var modName = "ModName";

            if (Storage.ContainsKey(thing.ID))
            {
                throw new Exception($"Thing with ID {thing.ID} already exists.");
            }
            Storage.Add(thing.ID, thing);
        }

        public IEnumerable<KeyValuePair<string, IThingData>> Iter()
        {
            // TODO ordering
            return Storage.AsEnumerable();
        }
    }
}
