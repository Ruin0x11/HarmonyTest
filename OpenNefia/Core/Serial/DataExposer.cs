using fNbt;
using OpenNefia.Core.Data;
using OpenNefia.Core.Data.Serial;
using OpenNefia.Core.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Serial
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

        internal bool EnterCompound(NbtCompound? compound)
        {
            if (compound == null)
                return false;

            CurrentCompound = compound;
            CompoundStack.Push(CurrentCompound);

            return true;
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
                if (ty.IsEnum)
                {
                    CurrentCompound.Add(new NbtString(name, Enum.GetName(ty, data!)));
                }
                else
                {
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

                        case TypeCode.Object:
                            var comp = new NbtCompound(name);
                            EnterCompound(comp);

                            if (ty == typeof(Type))
                            {
                                var type = (data as Type)!;
                                var typeName = type.FullName;
                                ExposeValue(ref typeName, "TypeName");
                            }
                            else if (ty == typeof(Love.Quad))
                            {
                                var quad = (data as Love.Quad)!;
                                var viewport = quad.GetViewport();
                                var textureDimensions = quad.GetTextureDimensions();
                                ExposeValue(ref viewport, "Viewport");
                                ExposeValue(ref textureDimensions, "TextureDimensions");
                            }
                            else if (ty == typeof(Love.Point))
                            {
                                var point = (Love.Point)Convert.ChangeType(data, typeof(Love.Point))!;
                                ExposeValue(ref point.X, "X");
                                ExposeValue(ref point.Y, "Y");
                            }
                            else if (ty == typeof(Love.Vector2))
                            {
                                var vec2 = (Love.Vector2)Convert.ChangeType(data, typeof(Love.Vector2))!;
                                ExposeValue(ref vec2.X, "X");
                                ExposeValue(ref vec2.Y, "Y");
                            }
                            else if (ty == typeof(Love.RectangleF))
                            {
                                var rect = (Love.RectangleF)Convert.ChangeType(data, typeof(Love.RectangleF))!;
                                ExposeValue(ref rect.X, "X");
                                ExposeValue(ref rect.Y, "Y");
                                ExposeValue(ref rect.Width, "Width");
                                ExposeValue(ref rect.Height, "Height");
                            }
                            else if (ty == typeof(Love.Color))
                            {
                                var color = (Love.Color)Convert.ChangeType(data, typeof(Love.Color))!;
                                ExposeValue(ref color.r, "r");
                                ExposeValue(ref color.g, "g");
                                ExposeValue(ref color.b, "b");
                                ExposeValue(ref color.a, "a");
                            }
                            else
                            {
                                throw new Exception($"Cannot serialize value of type {ty.Name}");
                            }

                            ExitCompound();
                            CurrentCompound.Add(comp);

                            break;

                        case TypeCode.Decimal:
                        case TypeCode.DateTime:
                        case TypeCode.Empty:
                        case TypeCode.DBNull:
                            throw new Exception($"Cannot serialize value of type {ty.Name}");
                    }
                }
            }
            else if (this.Stage == SerialStage.LoadingDeep | this.Stage == SerialStage.ResolvingRefs)
            {
                var ty = typeof(T);

                if (ty.IsEnum)
                {
                    var stringTag = CurrentCompound.Get<NbtString>(name);
                    var value = stringTag.StringValue;
                    if (Enum.IsDefined(ty, value))
                    {
                        data = (T)Convert.ChangeType(Enum.Parse(ty, value), ty)!;
                    }
                    else
                    {
                        throw new Exception($"Enum '{ty}' does not have variant '{value}'");
                    }
                }
                else
                {
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

                        case TypeCode.Object:
                            var comp = CurrentCompound.Get<NbtCompound>(name)!;

                            if (EnterCompound(comp))
                            {
                                if (ty == typeof(Type))
                                {
                                    var typeName = string.Empty;
                                    ExposeValue(ref typeName!, "TypeName");
                                    var type = Type.GetType(typeName);
                                    if (type == null)
                                    {
                                        throw new Exception($"Cannot find type named {typeName} in any assembly.");
                                    }
                                    data = (T?)(object)type;
                                }
                                else if (ty == typeof(Love.Quad))
                                {
                                    var viewport = new Love.RectangleF(0, 0, 0, 0);
                                    var textureDimensions = new Love.Vector2(0, 0);
                                    ExposeValue(ref viewport, "Viewport");
                                    ExposeValue(ref textureDimensions, "TextureDimensions");
                                    var quad = Love.Graphics.NewQuad(viewport.X, viewport.Y, viewport.Width, viewport.Height, textureDimensions.X, textureDimensions.Y);
                                    data = (T)Convert.ChangeType(quad, ty);
                                }
                                else if (ty == typeof(Love.Point))
                                {
                                    var point = (Love.Point)Convert.ChangeType(data, typeof(Love.Point))!;
                                    ExposeValue(ref point.X, "X");
                                    ExposeValue(ref point.Y, "Y");
                                    data = (T)Convert.ChangeType(point, ty);
                                }
                                else if (ty == typeof(Love.Vector2))
                                {
                                    var vec2 = (Love.Vector2)Convert.ChangeType(data, typeof(Love.Vector2))!;
                                    ExposeValue(ref vec2.X, "X");
                                    ExposeValue(ref vec2.Y, "Y");
                                    data = (T)Convert.ChangeType(vec2, ty);
                                }
                                else if (ty == typeof(Love.RectangleF))
                                {
                                    var rect = (Love.RectangleF)Convert.ChangeType(data, typeof(Love.RectangleF))!;
                                    ExposeValue(ref rect.X, "X");
                                    ExposeValue(ref rect.Y, "Y");
                                    ExposeValue(ref rect.Width, "Width");
                                    ExposeValue(ref rect.Height, "Height");
                                    data = (T)Convert.ChangeType(rect, ty);
                                }
                                else if (ty == typeof(Love.Color))
                                {
                                    var color = (Love.Color)Convert.ChangeType(data, typeof(Love.Color))!;
                                    ExposeValue(ref color.r, "r");
                                    ExposeValue(ref color.g, "g");
                                    ExposeValue(ref color.b, "b");
                                    ExposeValue(ref color.a, "a");
                                    data = (T)Convert.ChangeType(color, ty);
                                }
                                else
                                {
                                    throw new Exception($"Cannot deserialize value of type {ty.Name}");
                                }

                                ExitCompound();
                            }

                            break;

                        case TypeCode.Decimal:
                        case TypeCode.DateTime:
                        case TypeCode.Empty:
                        case TypeCode.DBNull:
                        default:
                            throw new Exception($"Cannot deserialize value of type {ty.Name}");
                    }
                }
            }
            else if (this.Stage == SerialStage.Invalid)
            {
                throw new Exception("Cannot use an invalid serializer.");
            }
        }

        public void ExposeDeep<T>(ref T data, string name, params object[] ctorParams)
        {
            var ty = typeof(T);

            if (this.Stage == SerialStage.Saving)
            {
                if (typeof(IDataExposable).IsAssignableFrom(ty))
                {
                    var comp = new NbtCompound(name);
                    EnterCompound(comp);

                    if (data!.GetType() != ty || ty.IsGenericTypeDefinition)
                    {
                        comp.Add(new NbtString("@Class", data.GetType().FullName));
                    }

                    ((IDataExposable)data).Expose(this);

                    if (typeof(IDataReferenceable).IsAssignableFrom(ty))
                    {
                        var index = ((IDataReferenceable)data).GetUniqueIndex();
                        if (FoundRefs.ContainsKey(index))
                            Errors.Add($"Overwriting existing deep saved compound '{index}' (of type '{ty.Name}')");
                        FoundRefs[index] = data;
                    }

                    ExitCompound();
                    CurrentCompound.Add(comp);
                }
                else
                {
                    ExposeValue<T>(ref data!, name);
                }
            }
            else if (this.Stage == SerialStage.LoadingDeep | this.Stage == SerialStage.ResolvingRefs)
            {
                if (typeof(IDataExposable).IsAssignableFrom(ty))
                {
                    var compound = CurrentCompound.Get<NbtCompound>(name)!;
                    if (EnterCompound(compound))
                    {
                        if (data == null)
                        {
                            var instantiateType = ty;
                            var classTag = compound.Get<NbtString>("@Class");
                            if (classTag != null)
                            {
                                var typeName = classTag.StringValue;
                                var type = FindTypeWithName(typeName);
                                if (type == null)
                                {
                                    throw new Exception($"Cannot find type with name {typeName} in all loaded assemblies");
                                }
                                instantiateType = type;
                            }
                            data = (T)ReflectionUtils.CreateFromPublicOrPrivateCtor(instantiateType, ctorParams)!;
                        }

                        ((IDataExposable)data).Expose(this);

                        if (this.Stage == SerialStage.LoadingDeep && typeof(IDataReferenceable).IsAssignableFrom(ty))
                        {
                            var index = ((IDataReferenceable)data).GetUniqueIndex();
                            FoundRefs[index] = data;
                        }

                        ExitCompound();
                    }
                }
                else
                {
                    ExposeValue<T>(ref data!, name);
                }
            }
            else if (this.Stage == SerialStage.Invalid)
            {
                throw new Exception("Cannot use an invalid serializer.");
            }
        }

        private Type? FindTypeWithName(string typeName)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = assembly.GetType(typeName, throwOnError: false, ignoreCase: true);
                if (type != null)
                {
                    return type;
                }
            }
            return Type.GetType(typeName, throwOnError: false, ignoreCase: true);
        }

        // TODO needs ExposeWeakNullable for T?, instead of calling like ExposeWeak(ref Object!, nameof(Object));
        public void ExposeWeak<T>(ref T? data, string tagName, T? defaultValue = default(T))
        {
            var ty = typeof(T);

            if (this.Stage == SerialStage.Saving)
            {
                if (typeof(IDataReferenceable).IsAssignableFrom(ty))
                {
                    if (data != null)
                    {
                        var index = ((IDataReferenceable)data).GetUniqueIndex();
                        CurrentCompound.Add(new NbtString(tagName, index));
                    }
                }
                else
                {
                    throw new Exception($"{ty} does not implement IDataReferenceable");
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

        public void ExposeDef<T>(ref T data, string tagName)
        {
            NbtCompound defCompound;

            var defTypeObj = DefLoader.GetDirectDefType(typeof(T));

            if (defTypeObj == null)
            {
                throw new Exception($"{typeof(T)} is not a subclass of Def");
            }

            string defId;

            if (this.Stage == SerialStage.Saving)
            {
                defCompound = new NbtCompound(tagName);
                defId = (data as Def)!.Id;
            }
            else
            {
                defCompound = CurrentCompound.Get<NbtCompound>(tagName)!;
                defId = string.Empty;
            }

            EnterCompound(defCompound);
            ExposeValue(ref defId!, "Id");
            ExitCompound();

            if (Stage == SerialStage.Saving)
            {
                CurrentCompound.Add(defCompound);
            }
            else if (Stage == SerialStage.LoadingDeep)
            {
                var def = DefLoader.GetDef(defTypeObj, defId);
                if (def == null)
                {
                    throw new Exception($"Def {defTypeObj.Name}.{defId} does not exist.");
                }
                data = (T)Convert.ChangeType(def, typeof(T));
            }
        }

        public void ExposeCollection<K, V>(ref Dictionary<K, V> data, string tagName, ExposeMode keyMode = ExposeMode.Default, ExposeMode valueMode = ExposeMode.Default) 
            where K: notnull
        {
            NbtCompound dictCompound;

            if (this.Stage == SerialStage.Saving)
            {
                dictCompound = new NbtCompound(tagName);
            }
            else
            {
                if (this.Stage == SerialStage.LoadingDeep)
                {
                    data = new Dictionary<K, V>();
                }
                dictCompound = CurrentCompound.Get<NbtCompound>(tagName)!;
            }

            var keyList = data.Keys.ToList();
            var valueList = data.Values.ToList();

            EnterCompound(dictCompound);
            ExposeCollection(ref keyList, "@Keys", keyMode);
            ExposeCollection(ref valueList, "@Values", valueMode);
            ExitCompound();

            if (Stage == SerialStage.Saving)
            {
                CurrentCompound.Add(dictCompound);
            }
            else if (Stage == SerialStage.LoadingDeep)
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
            var list = data.ToList();

            ExposeCollection(ref list, tagName, mode);

            if (Stage == SerialStage.LoadingDeep)
            {
                data = new T[list.Count];
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
                if (typeof(Def).IsAssignableFrom(ty))
                {
                    mode = ExposeMode.Def;
                }
                else
                {
                    mode = ExposeMode.Deep;
                }
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

                var count = data.Count;
                ExposeValue(ref count, "@Count");

                if (mode == ExposeMode.Deep)
                {
                    if (Stage == SerialStage.LoadingDeep)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            T entry = default(T)!;
                            this.ExposeDeep(ref entry!, i.ToString());
                            data.Add(entry);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < count; i++)
                        {
                            T entry = data[i];
                            this.ExposeDeep(ref entry!, i.ToString());
                        }
                    }
                }
                else if (mode == ExposeMode.Reference)
                {
                    if (Stage == SerialStage.LoadingDeep)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            T entry = default(T)!;
                            this.ExposeWeak(ref entry!, i.ToString());
                            data.Add(entry);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < count; i++)
                        {
                            T entry = data[i];
                            this.ExposeWeak(ref entry!, i.ToString());
                        }
                    }
                }
                else if (mode == ExposeMode.Def)
                {
                    if (Stage == SerialStage.LoadingDeep)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            Def def = null!;
                            this.ExposeDef(ref def!, i.ToString());
                            data.Add((T)Convert.ChangeType(def, typeof(T)));
                        }
                    }
                    else
                    {
                        for (int i = 0; i < count; i++)
                        {
                            T entry = data[i];
                            this.ExposeDef(ref entry!, i.ToString());
                        }
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
