using System;
using System.Linq;
using System.Xml.Linq;

namespace OpenNefia.Core.Extensions
{
    /// <summary>
    /// From https://stackoverflow.com/a/2493039.
    /// </summary>
    public static class XmlLinqExtensions
    {
        private static string relativeXPath(XElement e)
        {
            int index = e.IndexPosition();

            var currentNamespace = e.Name.Namespace;

            string name;
            if (currentNamespace == null)
            {
                name = e.Name.LocalName;
            }
            else
            {
                string namespacePrefix = e.GetPrefixOfNamespace(currentNamespace)!;
                name = namespacePrefix + ":" + e.Name.LocalName;
            }

            // If the element is the root, no index is required
            return (index == -1) ? "/" + name : string.Format
            (
                "/{0}[{1}]",
                name,
                index.ToString()
            );
        }

        private static bool IsDefElement(XElement e)
        {
            return e.Parent != null && e.Parent.Name == "Defs" && e.Parent.Parent == null && e.Attribute("Id") != null;
        }

        private static string relativeXPathDefAware(XElement e)
        {
            if (IsDefElement(e))
            {
                return $"/{e.Name.LocalName}[{e.Attribute("Id")}]";
            }
            else
            {
                return relativeXPath(e);
            }
        }

        /// <summary>
        /// Get the absolute XPath to a given XElement, including the namespace.
        /// (e.g. "/a:people/b:person[6]/c:name[1]/d:last[1]").
        /// </summary>
        public static string GetAbsoluteXPath(this XElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            var ancestors = from e in element.Ancestors()
                            select relativeXPath(e);

            return string.Concat(ancestors.Reverse().ToArray()) +
                   relativeXPath(element);
        }

        /// <summary>
        /// Get the absolute XPath to a given XElement, accounting for Def IDs.
        /// This is for programatically generating <see cref="PatchOperation"/>s 
        /// targeting a specific <see cref="XElement"/>.
        /// </summary>
        public static string GetAbsoluteXPathDefAware(this XElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            var ancestors = from e in element.Ancestors()
                            select relativeXPathDefAware(e);

            return string.Concat(ancestors.Reverse().ToArray()) +
                   relativeXPathDefAware(element);
        }

        /// <summary>
        /// Get the index of the given XElement relative to its
        /// siblings with identical names. If the given element is
        /// the root, -1 is returned.
        /// </summary>
        /// <param name="element">
        /// The element to get the index of.
        /// </param>
        public static int IndexPosition(this XElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            if (element.Parent == null)
            {
                return -1;
            }

            int i = 1; // Indexes for nodes start at 1, not 0

            foreach (var sibling in element.Parent.Elements(element.Name))
            {
                if (sibling == element)
                {
                    return i;
                }

                i++;
            }

            throw new InvalidOperationException
                ("element has been removed from its parent.");
        }
    }


    }
