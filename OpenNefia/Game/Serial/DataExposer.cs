using fNbt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Game.Serial
{
    public class DataExposer
    {
        private string Filepath { get; }
        public SerialStage Stage { get; internal set; }
        internal NbtFile File;
        private NbtCompound RootCompound;
        private NbtCompound CurrentCompound;
        private Dictionary<string, object> FoundRefs;
        private Stack<NbtCompound> CompoundStack;
        internal List<string> Errors { get; }

        internal DataExposer(string filepath, SerialStage stage)
        {
            Filepath = filepath;
            Stage = stage;
            CompoundStack = new Stack<NbtCompound>();
            FoundRefs = new Dictionary<string, object>();
            Errors = new List<string>();

            if (stage == SerialStage.LoadingDeep)
            {
                File = new NbtFile();
                File.LoadFromFile(filepath);
                RootCompound = File.RootTag;
            }
            else
            {
                RootCompound = new NbtCompound("Root");
                CurrentCompound = RootCompound;
                File = new NbtFile(RootCompound);
            }

            CurrentCompound = RootCompound;
            EnterCompound(RootCompound);
        }

        internal void EnterCompound(NbtCompound compound)
        {
            CurrentCompound = compound;
            CompoundStack.Push(CurrentCompound);
        }

        internal void ExitCompound()
        {
            CompoundStack.Pop();
            CurrentCompound = CompoundStack.Peek();
        }

        public void ExposeValue<T>(ref T data, string name)
        {
            if (this.Stage == SerialStage.Saving)
            {
                var ty = typeof(T);
                if (ty == typeof(int))
                {
                    CurrentCompound.Add(new NbtInt(name, (int)Convert.ChangeType(data, typeof(int))!));
                }
                else if (ty == typeof(string))
                {
                    CurrentCompound.Add(new NbtString(name, (string)Convert.ChangeType(data, typeof(string))!));
                }
                else
                {
                    Errors.Add($"Cannot serialize value of type {ty.Name}");
                }
            }
            else if (this.Stage == SerialStage.LoadingDeep | this.Stage == SerialStage.ResolvingRefs)
            {
                var ty = typeof(T);
                if (ty == typeof(int))
                {
                    var tag = CurrentCompound.Get<NbtInt>(name)!;
                    data = (T)Convert.ChangeType(tag.IntValue, ty);
                }
                else if (ty == typeof(string))
                {
                    var tag = CurrentCompound.Get<NbtString>(name)!;
                    data = (T)Convert.ChangeType(tag.StringValue, ty);
                }
                else
                {
                    Errors.Add($"Cannot deserialize value of type {ty.Name}");
                }
            }
            else if (this.Stage == SerialStage.Invalid)
            {
                throw new Exception("Cannot use an invalid serializer.");
            }
        }

        public void ExposeDeep<T>(ref T? data, string name) where T: IDataExposable, new()
        {
            if (this.Stage == SerialStage.Saving)
            {
                var ty = typeof(T);
                if (data != null)
                {
                    var comp = new NbtCompound(name);
                    EnterCompound(comp);
                    data.Expose(this);
                    ExitCompound();
                    CurrentCompound.Add(comp);

                    if (typeof(IDataReferenceable).IsAssignableFrom(ty))
                    {
                        var index = ((IDataReferenceable)data).GetUniqueIndex();
                        if (FoundRefs.ContainsKey(index))
                            Errors.Add($"Overwriting existing deep saved compound '{index}' (of type '{ty.Name}')");
                        FoundRefs[index] = data;
                    }
                }
            }
            else if (this.Stage == SerialStage.LoadingDeep | this.Stage == SerialStage.ResolvingRefs)
            {
                var ty = typeof(T);
                var compound = CurrentCompound.Get<NbtCompound>(name)!;
                if (compound != null)
                {
                    if (data == null)
                    {
                        data = (T)Activator.CreateInstance(ty)!;
                    }

                    EnterCompound(compound);
                    data.Expose(this);
                    ExitCompound();

                    if (this.Stage == SerialStage.LoadingDeep && typeof(IDataReferenceable).IsAssignableFrom(ty))
                    {
                        var index = ((IDataReferenceable)data).GetUniqueIndex();
                        FoundRefs[index] = data;
                    }
                }
            }
            else if (this.Stage == SerialStage.Invalid)
            {
                throw new Exception("Cannot use an invalid serializer.");
            }
        }

        internal void ExposeWeak<T>(ref T? data, string tagName) where T: class, IDataReferenceable
        {
            if (this.Stage == SerialStage.Saving)
            {
                if (data != null)
                {
                    var index = data.GetUniqueIndex();
                    CurrentCompound.Add(new NbtString(tagName, index));
                }
            }
            else if (this.Stage == SerialStage.ResolvingRefs)
            {
                var refTag = CurrentCompound.Get<NbtString>(tagName)!;
                if (refTag != null)
                {
                    if (FoundRefs.TryGetValue(refTag.StringValue, out var reference))
                    {
                        data = (T)reference;
                    }
                    else
                    {
                        data = null;
                    }
                }
            }
            else if (this.Stage == SerialStage.Invalid)
            {
                throw new Exception("Cannot use an invalid serializer.");
            }
        }

        internal void Save()
        {
            File.SaveToFile(Filepath, NbtCompression.ZLib);
        }
    }
}
