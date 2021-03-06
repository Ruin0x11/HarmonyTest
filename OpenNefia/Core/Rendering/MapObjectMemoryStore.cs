using OpenNefia.Core.Map;
using OpenNefia.Serial;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Rendering
{
    internal class MapObjectMemoryStore : IEnumerable<MapObjectMemory>, IDataExposable
    {
        internal InstancedMap Map;
        internal int CurrentIndex;
        internal Dictionary<int, MapObjectMemory> AllMemory;
        internal List<MapObjectMemory>?[] Positional;
        internal HashSet<MapObjectMemory> Added;
        internal Stack<MapObjectMemory> Removed;

        public MapObjectMemoryStore(InstancedMap map)
        {
            this.Map = map;
            CurrentIndex = 0;
            this.AllMemory = new Dictionary<int, MapObjectMemory>();
            this.Positional = new List<MapObjectMemory>?[map.Width * map.Height];
            this.Added = new HashSet<MapObjectMemory>();
            this.Removed = new Stack<MapObjectMemory>();
        }

        public void Expose(DataExposer data)
        {
            data.ExposeValue(ref CurrentIndex, nameof(CurrentIndex));
            data.ExposeCollection(ref AllMemory, nameof(AllMemory), ExposeMode.Deep, ExposeMode.Deep);

            if (data.Stage == SerialStage.ResolvingRefs)
            {
                Added.Clear();
                Removed.Clear();
                Positional = new List<MapObjectMemory>?[Map.Width * Map.Height];

                foreach (var memory in AllMemory.Values)
                {
                    var index = memory.TileX + Map.Width * memory.TileY;
                    var at = Positional[index];

                    if (at == null)
                    {
                        at = new List<MapObjectMemory>();
                        Positional[index] = at;
                    }

                    at.Add(memory);
                }
            }
        }

        public IEnumerator<MapObjectMemory> GetEnumerator() => AllMemory.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => AllMemory.Values.GetEnumerator();

        public void ForgetObjects(int index)
        {
            var at = Positional[index];

            if (at != null)
            {
                foreach (var memory in at)
                {
                    AllMemory.Remove(memory.Index);
                    Removed.Push(memory);
                }

                Positional[index] = null;
            }
        }

        internal void Flush()
        {
            foreach (var added in this.Added)
            {
                added.State = MemoryState.InUse;
            }
            this.Map._MapObjectMemory.Added.Clear();
            this.Map._MapObjectMemory.Removed.Clear();
        }

        public void RedrawAll()
        {
            this.Map._MapObjectMemory.Added.Clear();
            this.Map._MapObjectMemory.Removed.Clear();
            foreach (var memory in this.AllMemory.Values)
            {
                memory.State = MemoryState.Added;
                this.Added.Add(memory);
            }
        }

        public void RevealObjects(int index)
        {
            var at = Positional[index];

            var x = index % Map.Width;
            var y = index / Map.Width;

            if (at != null)
            {
                foreach (var memory in at)
                {
                    AllMemory.Remove(memory.Index);
                    Removed.Push(memory);
                }

                at.Clear();
            }

            int i = 0;
            foreach (var obj in Map.AtPos(x, y).GetMapObjects())
            {
                if (at == null)
                {
                    at = new List<MapObjectMemory>();
                    Positional[index] = at;
                }

                var memory = GetOrCreateMemory();

                obj.ProduceMemory(memory);

                if (memory.IsVisible)
                {
                    this.AllMemory[memory.Index] = memory;
                    this.Added.Add(memory);
                    memory.ObjectUid = obj.Uid;
                    memory.TileX = x;
                    memory.TileY = y;
                    memory.ZOrder = i;
                    memory.ObjectType = obj.GetType();
                    at.Add(memory);
                }

                i++;
            }
        }

        public MapObjectMemory GetOrCreateMemory()
        {
            MapObjectMemory memory;

            if (this.Removed.Count > 0)
            {
                // Index is not changed, to support reuse.
                memory = this.Removed.Pop();
            }
            else
            {
                // Allocate a new memory entry and increment the index.
                var index = CurrentIndex;
                CurrentIndex += 1;

                memory = new MapObjectMemory()
                {
                    Index = index
                };
            }

            memory.State = MemoryState.Added;

            return memory;
        }
    }
}
