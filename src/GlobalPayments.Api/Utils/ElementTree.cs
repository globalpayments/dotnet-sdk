using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace GlobalPayments.Api.Utils {
    internal class ElementTree {
        private XmlDocument doc;
        private Dictionary<string, string> namespaces;

        public ElementTree() {
            doc = new XmlDocument();
            namespaces = new Dictionary<string, string>();
            namespaces.Add("soap", "http://schemas.xmlsoap.org/soap/envelope/");
        }
        public ElementTree(string xml) : this() {
            var settings = new XmlReaderSettings {
                DtdProcessing = DtdProcessing.Prohibit,
            };

            var buffer = Encoding.UTF8.GetBytes(xml);
            var stream = new MemoryStream(buffer);
            var reader = XmlReader.Create(stream, settings);
            doc.Load(reader);
        }

        public Element Element(string tagName) {
            var element = CreateElement(tagName);
            return new Element(doc, element);
        }

        public Element SubElement(Element parent, string tagName) {
            var child = CreateElement(tagName);
            parent.GetElement().AppendChild(child);
            return new Element(doc, child);
        }
        public Element SubElement<T>(Element parent, string tagName, T value) {
            if (EqualityComparer<T>.Default.Equals(value, default(T)))
                return null;
            return SubElement(parent, tagName).Text(value.ToString());
        }
        //public Element SubElement(Element parent, string tagName, string value) {
        //    if (!string.IsNullOrEmpty(value)) {
        //        return SubElement(parent, tagName).Text(value);
        //    }
        //    return null;
        //}
        //public Element SubElement(Element parent, string tagName, int value) {
        //    if (value != default(int)) {
        //        return SubElement(parent, tagName).Text(value.ToString());
        //    }
        //    return null;
        //}

        public string ToString(Element root) {
            doc.AppendChild(root.GetElement());
            try {
                return doc.InnerXml;
            }
            finally {
                doc.RemoveChild(root.GetElement());
            }
        }

        public string ToJsonString(Element root) {
            doc.AppendChild(root.GetElement());
            try {
                var sb = new StringBuilder("{");
                foreach (XmlNode child in root.GetElement().ChildNodes) {
                    sb.Append(string.Format("\"{0}\": \"{1}\", ", child.Name, child.InnerText));
                }
                var rvalue = sb.ToString().TrimEnd(',', ' ') + "}";
                return rvalue;
            }
            finally {
                doc.RemoveChild(root.GetElement());
            }
        }

        public Element Get(string tagName) {
            try {
                var node = doc.GetElementsByTagName(tagName)[0];
                if(node != null)
                    return Utils.Element.FromNode(doc, node);
                return null;
            }
            catch (IndexOutOfRangeException) {
                return null;
            }
        }

        public static ElementTree Parse(string xml) {
            return new ElementTree(xml);
        }

        public static ElementTree Parse(byte[] buffer) {
            var xmlString = string.Empty;
            foreach (var b in buffer)
                xmlString += (char)b;

            return Parse(xmlString);
        }

        private XmlElement CreateElement(string tagName) {
            XmlElement element;
            if (tagName.Contains(":")) {
                string[] data = tagName.Split(':');
                element = doc.CreateElement(data[0], data[1], namespaces[data[0]]);
            }
            else element = doc.CreateElement(tagName);
            return element;
        }

        public void AddNamespace(string prefix, string uri)
        {
            namespaces.Add(prefix, uri);
        }
    }

    internal class Element {
        private XmlDocument doc;
        private XmlElement element;

        public XmlElement GetElement() {
            return element;
        }

        public Element(XmlDocument doc, XmlElement element) {
            this.doc = doc;
            this.element = element;
        }

        public Element FirstChild() {
            return FromNode(doc, element.FirstChild);
        }

        public Element Prefix(string prefix) {
            element.Prefix = prefix;
            return this;
        }

        public Element Remove(string tagName) {
            var child = Get(tagName);
            if (child != null)
                element.RemoveChild(child.GetElement());
            return this;
        }

        public Element Set(string name, string value) {
            element.SetAttribute(name, value);
            return this;
        }

        public Element Text(string text) {
            element.InnerText = text;
            return this;
        }

        public Element Append(Element child) {
            doc.ImportNode(child.GetElement(), true);
            element.AppendChild(child.GetElement());
            return this;
        }

        public string Tag() { return element.Name; }

        public bool Has(string tagName) {
            return element.GetElementsByTagName(tagName).Count > 0;
        }

        public Element Get(string tagName) {
            return Element.FromNode(doc, element.GetElementsByTagName(tagName)[0]);
        }

        public Element[] GetAll() {
            var nodes = element.ChildNodes;
            Element[] elements = new Element[nodes.Count];
            for (int i = 0; i < elements.Length; i++) {
                elements[i] = Element.FromNode(doc, nodes[i]);
            }
            return elements;
        }

        public Element[] GetAll(string tagName) {
            var nodes = element.GetElementsByTagName(tagName);
            Element[] elements = new Element[nodes.Count];
            for (int i = 0; i < elements.Length; i++) {
                elements[i] = Element.FromNode(doc, nodes[i]);
            }
            return elements;
        }

        public T GetValue<T>(params string[] tagNames) {
            try {
                foreach (var tagName in tagNames) {
                    var node = element.GetElementsByTagName(tagName)[0];
                    if (node != null)
                        return (T)Convert.ChangeType(node.InnerText, typeof(T));
                }
                return default(T);
            }
            catch (IndexOutOfRangeException) {
                return default(T);
            }
        }

        public T GetAttribute<T>(string attributeName) {
            return (T)Convert.ChangeType(element.GetAttribute(attributeName), typeof(T));
        }

        public static Element FromNode(XmlDocument doc, XmlNode node) {
            return new Element(doc, (XmlElement)node);
        }
    }
}
