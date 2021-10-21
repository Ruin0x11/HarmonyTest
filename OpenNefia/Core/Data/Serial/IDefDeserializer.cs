using FluentResults;
using OpenNefia.Core.Data.Serial.CrossRefs;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;

namespace OpenNefia.Core.Data.Serial
{
    public interface IDefDeserializer
    {
        void PopulateAllFields(XElement element, object target, Type containingModType);
        void PopulateField(XElement childElement, object target, FieldInfo field, Type containingModType);
        void PopulateField(XAttribute childAttribute, object target, FieldInfo field, Type containingModType);
        void PopulateFieldByName(string name, XElement element, object target, Type containingModType);
        void PopulateFieldByElement(string name, XElement element, object target, Type containingModType);
        void PopulateFieldByAttribute(string name, XAttribute attribute, object target, Type containingModType);
        void AddCrossRef(IDefCrossRef crossRef);

        // TODO remove in order to support save-local serialized defs
        Result<object> DeserializeObject(XElement element, Type ty, Type containingModType, FieldInfo? info = null);
    }
}