using OpenNefia.Core.Data.Serial;
using OpenNefia.Mod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OpenNefia.Core.Data
{
    public class Def : IComparable<Def>, IEquatable<Def>, IDefSerializable
    {
        public string Id { get; }

        public int? ElonaId { get; internal set; }

        public BaseMod? Mod { get; internal set; }

        public Def(string id)
        {
            this.Id = id;
        }

        public virtual void OnResolveReferences()
        {
        }

        public virtual void DeserializeDefField(IDefDeserializer deserializer, XmlNode node, Type containingModType)
        {
            deserializer.PopulateAllFields(node, this, containingModType);
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
