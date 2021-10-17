using System;

namespace OpenNefia.Core.Data
{
    public struct DefIdentifier
    {
        public readonly Type DefType;
        public readonly string DefId;

        public DefIdentifier(Type defType, string defId)
        {
            DefType = defType;
            DefId = defId;
        }

        public override int GetHashCode()
        {
            return (DefType?.GetHashCode() ?? 0) ^ (DefId?.GetHashCode() ?? 0);
        }

        public override bool Equals(object? obj)
        {
            if (!(obj is DefIdentifier key))
            {
                return false;
            }

            return Equals(key);
        }

        public bool Equals(DefIdentifier other)
        {
            return (Equals(DefType, other.DefType) && Equals(DefId, other.DefId));
        }

        public override string ToString() => $"{DefType.Name}:{DefId}";

        public void Deconstruct(out Type defType, out string defId)
        {
            defType = this.DefType;
            defId = this.DefId;
        }
    }
}
