using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace FISE_Browser.Helper
{
    public class opf
    {
       
    }

    [XmlRoot(ElementName = "title", Namespace = "http://purl.org/dc/elements/1.1/")]
    public class Title
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "creator", Namespace = "http://purl.org/dc/elements/1.1/")]
    public class Creator
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "file-as")]
        public string Fileas { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "identifier", Namespace = "http://purl.org/dc/elements/1.1/")]
    public class Identifier
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "meta", Namespace = "http://www.idpf.org/2007/opf")]
    public class Meta
    {
        [XmlAttribute(AttributeName = "property")]
        public string Property { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "metadata", Namespace = "http://www.idpf.org/2007/opf")]
    public class Metadata
    {
        [XmlElement(ElementName = "title", Namespace = "http://purl.org/dc/elements/1.1/")]
        public Title Title { get; set; }
        [XmlElement(ElementName = "creator", Namespace = "http://purl.org/dc/elements/1.1/")]
        public List<Creator> Creator { get; set; }
        [XmlElement(ElementName = "identifier", Namespace = "http://purl.org/dc/elements/1.1/")]
        public Identifier Identifier { get; set; }
        [XmlElement(ElementName = "language", Namespace = "http://purl.org/dc/elements/1.1/")]
        public string Language { get; set; }
        [XmlElement(ElementName = "description", Namespace = "http://purl.org/dc/elements/1.1/")]
        public string Description { get; set; }
        [XmlElement(ElementName = "publisher", Namespace = "http://purl.org/dc/elements/1.1/")]
        public string Publisher { get; set; }
        [XmlElement(ElementName = "meta", Namespace = "http://www.idpf.org/2007/opf")]
        public List<Meta> Meta { get; set; }
        [XmlAttribute(AttributeName = "dc", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Dc { get; set; }
    }

    [XmlRoot(ElementName = "item")]
    public class Item
    {
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "href")]
        public string Href { get; set; }
        [XmlAttribute(AttributeName = "media-type")]
        public string Mediatype { get; set; }
    }

    [XmlRoot(ElementName = "item", Namespace = "http://www.idpf.org/2007/opf")]
    public class Item2
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "href")]
        public string Href { get; set; }
        [XmlAttribute(AttributeName = "media-type")]
        public string Mediatype { get; set; }
        [XmlAttribute(AttributeName = "properties")]
        public string Properties { get; set; }
    }

    [XmlRoot(ElementName = "manifest", Namespace = "http://www.idpf.org/2007/opf")]
    public class Manifest
    {
        [XmlElement(ElementName = "item")]
        public List<Item> Item { get; set; }
        [XmlElement(ElementName = "item", Namespace = "http://www.idpf.org/2007/opf")]
        public List<Item2> Item2 { get; set; }
    }

    [XmlRoot(ElementName = "itemref")]
    public class Itemref
    {
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
        [XmlAttribute(AttributeName = "linear")]
        public string Linear { get; set; }
        [XmlAttribute(AttributeName = "idref")]
        public string Idref { get; set; }
        [XmlAttribute(AttributeName = "height2")]
        public string Height2 { get; set; }
        [XmlAttribute(AttributeName = "height1")]
        public string Height1 { get; set; }
    }

    [XmlRoot(ElementName = "spine", Namespace = "http://www.idpf.org/2007/opf")]
    public class Spine
    {
        [XmlElement(ElementName = "itemref")]
        public List<Itemref> Itemref { get; set; }
    }

    [XmlRoot(ElementName = "package", Namespace = "http://www.idpf.org/2007/opf")]
    public class Package
    {
        [XmlElement(ElementName = "metadata", Namespace = "http://www.idpf.org/2007/opf")]
        public Metadata Metadata { get; set; }
        [XmlElement(ElementName = "manifest", Namespace = "http://www.idpf.org/2007/opf")]
        public Manifest Manifest { get; set; }
        [XmlElement(ElementName = "spine", Namespace = "http://www.idpf.org/2007/opf")]
        public Spine Spine { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
        [XmlAttribute(AttributeName = "version")]
        public string Version { get; set; }
        [XmlAttribute(AttributeName = "lang", Namespace = "http://www.w3.org/XML/1998/namespace")]
        public string Lang { get; set; }
        [XmlAttribute(AttributeName = "unique-identifier")]
        public string Uniqueidentifier { get; set; }
        [XmlAttribute(AttributeName = "prefix")]
        public string Prefix { get; set; }
    }
}