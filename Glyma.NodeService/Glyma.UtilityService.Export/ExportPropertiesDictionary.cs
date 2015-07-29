using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace Glyma.UtilityService.Export
{
    public class ExportPropertiesDictionary : Dictionary<string, string>
    {
        private readonly string xmlSchema = @"<xs:schema id='dictionary' xmlns='' xmlns:xs='http://www.w3.org/2001/XMLSchema'>
                                                  <xs:element name='{0}'>
                                                    <xs:complexType>
                                                      <xs:choice minOccurs='0' maxOccurs='unbounded'>
                                                        <xs:element name='{1}'>
                                                          <xs:complexType>
                                                            <xs:attribute name='{2}' type='xs:string' />
                                                            <xs:attribute name='{3}' type='xs:string' />
                                                          </xs:complexType>
                                                        </xs:element>
                                                      </xs:choice>
                                                    </xs:complexType>
                                                  </xs:element>
                                                </xs:schema>";

        private const string _rootNodeName = "ExportProperties";
        private const string _itemNodeName = "Property";
        private const string _keyAttrName = "Key";
        private const string _valueAttrName = "Value";

        private ExportPropertiesDictionary()
        {
            xmlSchema = string.Format(xmlSchema, _rootNodeName, _itemNodeName, _keyAttrName, _valueAttrName);
        }

        public ExportPropertiesDictionary(IDictionary<string, string> dictionary) 
            : this()
        {
            if (dictionary != null)
            {
                foreach (string key in dictionary.Keys)
                {
                    Add(key, dictionary[key]);
                }
            }
        }

        public ExportPropertiesDictionary(string xml)
            : this()
        {
            if (!string.IsNullOrEmpty(xml)) 
            {
                ConvertFromXml(xml);
            }
        }

        public string ConvertToXml()
        {
            XElement xElement = new XElement(_rootNodeName, from key in this.Keys
                                                            select new XElement(_itemNodeName, new XAttribute(_keyAttrName, key), new XAttribute(_valueAttrName, this[key])));
            return xElement.ToString(SaveOptions.DisableFormatting);
        }

        private void ConvertFromXml(string xml)
        {
            Clear(); //clear the existing Dictionary

            XDocument xDocument = XDocument.Parse(xml);

            ValidateXml(xDocument); //validate that the XML is well formed to our schema

            var dictionaryItemQuery = from element in xDocument.Root.Elements()
                                      where element.Name == _itemNodeName &&
                                            element.Attributes().Count() == 2 &&
                                            element.FirstAttribute.Name == _keyAttrName &&
                                            element.LastAttribute.Name == _valueAttrName
                                      select element;
            foreach (XElement keyValuePair in dictionaryItemQuery)
            {
                Add(keyValuePair.Attribute(_keyAttrName).Value, keyValuePair.Attribute(_valueAttrName).Value);
            }
        }

        private void ValidateXml(XDocument xDocument)
        {
            bool isValid = true;
            string errorMessage = string.Empty;

            XmlSchemaSet schemas = new XmlSchemaSet();
            schemas.Add("", XmlReader.Create(new StringReader(xmlSchema)));

            xDocument.Validate(schemas, (sender, e) =>
                        {
                            errorMessage = string.Format("Validation error: {0}", e.Message);
                            isValid = false;
                        }, true);

            if (!isValid)
            {
                throw new XmlSchemaValidationException(errorMessage);
            }
        }
    }
}
