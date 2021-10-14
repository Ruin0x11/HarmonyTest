﻿using fNbt;
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

        public void ExposeValue<T>(ref T? data, string name, T? defaultValue = default(T))
        {
            if (this.Stage == SerialStage.Saving)
            {
                var ty = typeof(T);

                switch (Type.GetTypeCode(ty))
                {
                    case TypeCode.Boolean:
                    case TypeCode.Char:
                    case TypeCode.Byte:
                    case TypeCode.SByte:
                        CurrentCompound.Add(new NbtByte(name, (byte)Convert.ChangeType(data, typeof(byte))!));
                        break;
                    case TypeCode.Int16:
                    case TypeCode.UInt16:
                        CurrentCompound.Add(new NbtShort(name, (short)Convert.ChangeType(data, typeof(short))!));
                        break;
                    case TypeCode.Int32:
                    case TypeCode.UInt32:
                        CurrentCompound.Add(new NbtInt(name, (int)Convert.ChangeType(data, typeof(int))!));
                        break;
                    case TypeCode.Int64:
                    case TypeCode.UInt64:
                        CurrentCompound.Add(new NbtLong(name, (long)Convert.ChangeType(data, typeof(long))!));
                        break;
                    case TypeCode.Single:
                        CurrentCompound.Add(new NbtFloat(name, (float)Convert.ChangeType(data, typeof(float))!));
                        break;
                    case TypeCode.Double:
                        CurrentCompound.Add(new NbtDouble(name, (double)Convert.ChangeType(data, typeof(double))!));
                        break;
                    case TypeCode.String:
                        CurrentCompound.Add(new NbtString(name, (string)Convert.ChangeType(data, typeof(string))!));
                        break;

                    case TypeCode.Decimal:
                    case TypeCode.DateTime:
                    case TypeCode.Object:
                    case TypeCode.Empty:
                    case TypeCode.DBNull:
                        throw new Exception($"Cannot serialize value of type {ty.Name}");
                }
            }
            else if (this.Stage == SerialStage.LoadingDeep | this.Stage == SerialStage.ResolvingRefs)
            {
                var ty = typeof(T);

                switch (Type.GetTypeCode(ty))
                {
                    case TypeCode.Boolean:
                    case TypeCode.Char:
                    case TypeCode.Byte:
                    case TypeCode.SByte:
                        var byteTag = CurrentCompound.Get<NbtByte>(name);
                        if (byteTag != null)
                            data = (T)Convert.ChangeType(byteTag.ByteValue, ty);
                        else
                            data = defaultValue;
                        break;
                    case TypeCode.Int16:
                    case TypeCode.UInt16:
                        var shortTag = CurrentCompound.Get<NbtShort>(name);
                        if (shortTag != null)
                            data = (T)Convert.ChangeType(shortTag.ShortValue, ty);
                        else
                            data = defaultValue;
                        break;
                    case TypeCode.Int32:
                    case TypeCode.UInt32:
                        var intTag = CurrentCompound.Get<NbtInt>(name);
                        if (intTag != null)
                            data = (T)Convert.ChangeType(intTag.IntValue, ty);
                        else
                            data = defaultValue;
                        break;
                    case TypeCode.Int64:
                    case TypeCode.UInt64:
                        var longTag = CurrentCompound.Get<NbtLong>(name);
                        if (longTag != null)
                            data = (T)Convert.ChangeType(longTag.LongValue, ty);
                        else
                            data = defaultValue;
                        break;
                    case TypeCode.Single:
                        var floatTag = CurrentCompound.Get<NbtFloat>(name);
                        if (floatTag != null)
                            data = (T)Convert.ChangeType(floatTag.FloatValue, ty);
                        else
                            data = defaultValue; 
                        break;
                    case TypeCode.Double:
                        var doubleTag = CurrentCompound.Get<NbtDouble>(name);
                        if (doubleTag != null)
                            data = (T)Convert.ChangeType(doubleTag.DoubleValue, ty);
                        else
                            data = defaultValue; 
                        break;
                    case TypeCode.String:
                        var stringTag = CurrentCompound.Get<NbtString>(name)!;
                        if (stringTag != null)
                            data = (T)Convert.ChangeType(stringTag.StringValue, ty);
                        else
                            data = defaultValue;
                        break;

                    case TypeCode.Decimal:
                    case TypeCode.DateTime:
                    case TypeCode.Object:
                    case TypeCode.Empty:
                    case TypeCode.DBNull:
                        throw new Exception($"Cannot deserialize value of type {ty.Name}");
                }
            }
            else if (this.Stage == SerialStage.Invalid)
            {
                throw new Exception("Cannot use an invalid serializer.");
            }
        }

        public void ExposeDeep<T>(ref T data, string name)
        {
            var ty = typeof(T);
            if (this.Stage == SerialStage.Saving)
            {
                if (data != null)
                {
                    if (typeof(IDataExposable).IsAssignableFrom(ty))
                    {
                        var comp = new NbtCompound(name);
                        EnterCompound(comp);
                        ((IDataExposable)data).Expose(this);
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
                    else
                    {
                        throw new Exception($"Cannot serialize type {ty} as deep");
                    }
                }
            }
            else if (this.Stage == SerialStage.LoadingDeep | this.Stage == SerialStage.ResolvingRefs)
            {
                if (typeof(IDataExposable).IsAssignableFrom(ty))
                {
                    var compound = CurrentCompound.Get<NbtCompound>(name)!;
                    if (compound != null)
                    {

                        EnterCompound(compound);
                        ((IDataExposable)data!).Expose(this);
                        ExitCompound();

                        if (this.Stage == SerialStage.LoadingDeep && typeof(IDataReferenceable).IsAssignableFrom(ty))
                        {
                            var index = ((IDataReferenceable)data).GetUniqueIndex();
                            FoundRefs[index] = data;
                        }
                    }
                }
                else
                {
                    throw new Exception($"Cannot deserialize type {ty} as deep");
                }
            }
            else if (this.Stage == SerialStage.Invalid)
            {
                throw new Exception("Cannot use an invalid serializer.");
            }
        }

        public void ExposeWeak<T>(ref T? data, string tagName, T? defaultValue = default(T))
        {
            var ty = typeof(T);
            if (this.Stage == SerialStage.Saving)
            {
                if (data != null && typeof(IDataReferenceable).IsAssignableFrom(ty))
                {
                    var index = ((IDataReferenceable)data).GetUniqueIndex();
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
                        data = defaultValue;
                    }
                }
            }
            else if (this.Stage == SerialStage.Invalid)
            {
                throw new Exception("Cannot use an invalid serializer.");
            }
        }

        public void ExposeCollection<K, V>(ref Dictionary<K, V> data, string tagName, ExposeMode keyMode = ExposeMode.Default, ExposeMode valueMode = ExposeMode.Default) where K: notnull
        {
            if (keyMode == ExposeMode.Default)
            {
                keyMode = ExposeMode.Deep;
            }
            if (valueMode == ExposeMode.Default)
            {
                valueMode = ExposeMode.Deep;
            }

            NbtCompound dictCompound;

            if (this.Stage == SerialStage.Saving)
            {
                dictCompound = new NbtCompound(tagName);
            }
            else
            {
                dictCompound = CurrentCompound.Get<NbtCompound>(tagName)!;
            }

            EnterCompound(dictCompound);
            var keyList = data.Keys.ToList();
            var valueList = data.Values.ToList();
            ExitCompound();

            ExposeCollection(ref keyList, "Keys", keyMode);
            ExposeCollection(ref valueList, "Values", valueMode);

            if (Stage == SerialStage.LoadingDeep)
            {
                data = new Dictionary<K, V>();
                for (var i = 0; i < keyList.Count; i++)
                {
                    data.Add(keyList[i], valueList[i]);
                }
            }
        }

        public void ExposeCollection<T>(ref T[] data, string tagName, ExposeMode mode = ExposeMode.Default)
        {
            if (mode == ExposeMode.Default)
            {
                mode = ExposeMode.Deep;
            }

            var list = data.ToList();

            ExposeCollection(ref list, tagName, mode);

            if (Stage == SerialStage.LoadingDeep)
            {
                for (var i = 0; i < data.Length; i++)
                {
                    data[i] = list[i];
                }
            }
        }

        public void ExposeCollection<T>(ref List<T> data, string tagName, ExposeMode mode = ExposeMode.Default)
        {
            var ty = typeof(T);

            if (mode == ExposeMode.Default)
            {
                mode = ExposeMode.Deep;
            }

            if (ty == typeof(byte))
            {
                if (this.Stage == SerialStage.Saving)
                {
                    List<byte> list = (List<byte>)Convert.ChangeType(data, typeof(List<byte>))!;
                    var arrayCompound = new NbtByteArray(tagName, list.ToArray());
                    CurrentCompound.Add(arrayCompound);
                }
                else if (this.Stage == SerialStage.LoadingDeep)
                {
                    var arrayCompound = CurrentCompound.Get<NbtByteArray>(tagName)!;
                    data = new List<T>();
                    for (int i = 0; i < arrayCompound.Value.Length; i++)
                    {
                        data.Add((T)Convert.ChangeType(arrayCompound[i], ty));
                    }
                }
            }
            else if (ty == typeof(int) || ty == typeof(short))
            {
                if (this.Stage == SerialStage.Saving)
                {
                    List<int> list = new List<int>();
                    foreach (var i in data)
                    {
                        list.Add((int)Convert.ChangeType(i, typeof(int))!);
                    }
                    var arrayCompound = new NbtIntArray(tagName, list.ToArray());
                    CurrentCompound.Add(arrayCompound);
                }
                else if (this.Stage == SerialStage.LoadingDeep)
                {
                    var arrayCompound = CurrentCompound.Get<NbtIntArray>(tagName)!;
                    data = new List<T>();
                    for (int i = 0; i < arrayCompound.Value.Length; i++)
                    {
                        data.Add((T)Convert.ChangeType(arrayCompound[i], ty));
                    }
                }
            }
            else
            {
                NbtCompound listCompound;

                if (this.Stage == SerialStage.Saving)
                {
                    listCompound = new NbtCompound(tagName);
                }
                else
                {
                    listCompound = CurrentCompound.Get<NbtCompound>(tagName)!;
                }

                this.EnterCompound(listCompound);

                if (mode == ExposeMode.Deep)
                {
                    for (int i = 0; i < data.Count; i++)
                    {
                        T entry = data[i];
                        this.ExposeDeep(ref entry!, i.ToString());
                    }
                }
                else if (mode == ExposeMode.Reference)
                {
                    for (int i = 0; i < data.Count; i++)
                    {
                        T entry = data[i];
                        this.ExposeWeak(ref entry!, i.ToString());
                    }
                }

                this.ExitCompound();

                if (this.Stage == SerialStage.Saving)
                {
                    CurrentCompound.Add(listCompound);
                }
            }
        }

        internal void Save()
        {
            File.SaveToFile(Filepath, NbtCompression.ZLib);
        }
    }
}
