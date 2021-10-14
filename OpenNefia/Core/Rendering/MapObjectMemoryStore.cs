using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Rendering
{
    internal class MapObjectMemoryStore : IEnumerable<MapObjectMemory>
    {
        private InstancedMap Map;
        internal int CurrentIndex;
        internal Dictionary<int, MapObjectMemory> AllMemory;
        internal List<MapObjectMemory>?[] Positional;
        internal Stack<MapObjectMemory> Removed;

        public MapObjectMemoryStore(InstancedMap map)
        {
            this.Map = map;
            CurrentIndex = 0;
            this.AllMemory = new Dictionary<int, MapObjectMemory>();
            this.Positional = new List<MapObjectMemory>?[map.Width * map.Height];
            this.Removed = new Stack<MapObjectMemory>();
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

        public void RevealObjects(int index)
        {
            var at = Positional[index];

            if (at != null)
            {
                foreach (var memory in at)
                {
                    AllMemory.Remove(memory.Index);
                    Removed.Push(memory);
                }

                at.Clear();
            }

            var x = index % Map.Width;
            var y = index / Map.Height;

            int i = 0;
            foreach (var obj in Map._Pool.At(x, y))
            {
                if (at == null)
                {
                    at = new List<MapObjectMemory>();
                    Positional[index] = at;
                }

                var memory = GetOrCreateMemory();

                obj.ProduceMemory(ref memory);

                if (memory.IsVisible)
                {
                    this.AllMemory[memory.Index] = memory;
                    memory.TypeKey = obj.TypeKey;
                    memory.ZOrder = i;
                }

                i++;
            }
        }

        public MapObjectMemory GetOrCreateMemory()
        {
            MapObjectMemory memory;

            if (this.Removed.Count > 0)
            {
                memory = this.Removed.Pop();
            }
            else
            {
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
