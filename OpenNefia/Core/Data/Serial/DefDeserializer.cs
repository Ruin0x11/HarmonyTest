using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

/*
namespace OpenNefia.Core.Data.Serial
{
    /// <summary>
    /// Mostly modeled on top of Newtonsoft.JSON, because I wouldn't 
    /// dare attempt to write my own deserializer completely from scratch.
    /// </summary>
    internal class DefDeserializer
    {
        private DeserializeContract GetContract(object target)
        {
            throw new NotImplementedException();
        }

        private DeserializeConverter GetConverter(DeserializeContract propertyContract,
            DeserializeObjectContract contract,
            DeserializeConverter? converter, 
            DeserializeProperty? targetProperty)
        {
            throw new NotImplementedException();
        }

        public Def DeserializeDef(Type defType, XmlNode node)
        {
            var id = node.Attributes?["Id"];

            if (id == null)
                throw new DefLoadException($"Def has no ID.");

            var defInstance = (Def)Activator.CreateInstance(defType, id.Value)!;

            var contract = GetContract(defInstance);

            PopulateObject(defInstance, defType, contract, node);

            return defInstance;
        }

        private void PopulateObject(XmlNode node, object target, Type targetType, DeserializeObjectContract contract, DeserializeProperty? targetProperty)
        {
            foreach (var child in node.ChildNodes)
            {
                var childNode = (XmlNode)child;

                var content = childNode.Value!;
                var memberName = childNode.Name;
                var hasChildren = childNode.ChildNodes.Count > 0;

                var property = contract.Properties.GetClosestMatchingProperty(memberName);

                if (property != null)
                {
                    if (property.PropertyContract == null)
                    {
                        property.PropertyContract = GetContract(property.PropertyType);
                    }

                    var propertyConverter = GetConverter(property.PropertyContract, property.Converter, contract, targetProperty);

                    SetPropertyValue(childNode, target, property, contract, targetProperty, converter);
                }
                else
                {
                    throw new DefLoadException($"Type '{type.FullName}' has no member named '{memberName}.");
                }
            }
        }

        private bool ShouldSkipSettingProperty(DeserializeProperty property, object contract)
        {
            return false;
        }

        private void SetPropertyValue(
            XmlNode childNode,
            object target,
            DeserializeProperty property, 
            DeserializeContainerContract containerContract, 
            DeserializeProperty? containerProperty, 
            DeserializeConverter? propertyConverter)
        {
            if (ShouldSkipSettingProperty(property))
            {
                return;
            }

            object? value;

            if (propertyConverter != null && propertyConverter.CanRead)
            {
                value = DeserializeConvertable(propertyConverter, property.PropertyType!, childNode);
            }
            else
            {
                value = CreateValue(childNode, property.PropertyType, property.PropertyContract, property, containerContract, containerProperty);
            }

            property.ValueProvider!.SetValue(target, value);
        }

        private object? DeserializeConvertable(DeserializeConverter? propertyConverter, Type propertyType, XmlNode childNode)
        {
            throw new NotImplementedException();
        }

        private object? CreateValue(XmlNode childNode, Type propertyType, 
            DeserializeContract propertyContract, 
            DeserializeProperty property, 
            DeserializeContainerContract containerContract, 
            DeserializeProperty? containerProperty)
        {
            throw new NotImplementedException();
        }
    }
}
*/