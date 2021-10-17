using OpenNefia.Core.Data.Serial;
using OpenNefia.Mod;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace OpenNefia.Core.Data
{
    public abstract class Def : IComparable<Def>, IEquatable<Def>, IDefSerializable
    {
        public string Id { get; }

        public int? ElonaId { get; internal set; }

        public ModInfo? Mod { get; internal set; }

        public uint HotReloadId { get; internal set; }

        public XElement? OriginalXml { get; internal set; }

        public DefIdentifier Identifier { get; }

        internal Def(string id)
        {
            this.Id = id;
            this.Identifier = new DefIdentifier(GetDirectDefType(), this.Id);
        }

        public Type GetDirectDefType()
        {
            var type = this.GetType();

            while (type.BaseType != null && type.BaseType != typeof(object))
            {
                if (type.BaseType == typeof(Def))
                {
                    return type;
                }

                type = type.BaseType;
            }

            throw new Exception($"Def type {type} is not a subclass of Def!");
        }

        public virtual void OnResolveReferences()
        {
        }

        public virtual bool IsThemable => false;

        public virtual void OnMerge()
        {

        }

        public virtual void DeserializeDefField(IDefDeserializer deserializer, XElement element, Type containingModType)
        {
            deserializer.PopulateAllFields(element, this, containingModType);
        }

        public virtual void ValidateDefField(List<string> errors)
        {
        }

        public override string ToString() => $"<Def {this.GetType().Name}.{this.Id}>";

        public int CompareTo(Def? other)
        {
            if (this.Mod == other?.Mod)
                return this.Id.CompareTo(other?.Id);
            return -1;
        }

        public bool Equals(Def? other)
        {
            return this.Mod == other?.Mod && this.Id == other?.Id;
        }
    }
}
