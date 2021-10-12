using System;
using System.Reflection;
using System.Xml;

namespace OpenNefia.Core.Data.Serial
{
    public interface IDefDeserializer
    {
        void PopulateAllFields(XmlNode node, object target, Type containingModType);
        void PopulateField(XmlNode childNode, object target, PropertyInfo prop, Type containingModType);
        void PopulateFieldByName(string name, XmlNode node, object target, Type containingModType);
        void PopulateFieldByNode(string name, XmlNode node, object target, Type containingModType);
    }
}