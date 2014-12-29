#region Copyright

// (c) 2013 ELVEES NeoTek CJSC. All rights reserved.
// Closed source software. Actual software is delivered under the license agreement and (or) non-disclosure agreement.
// All software is copyrighted by ELVEES NeoTek CJSC (Russia) and may not be copying, publicly transmitted, modifying or distributed without prior written authorization from the copyright holder.

#endregion

using System;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using JetBrains.Annotations;

namespace Alnet.Common
{
   /// <summary>
   /// A set of utility methods for work with XML.
   /// </summary>
   public static class XmlUtils
   {
      #region Public methods

      /// <summary>
      /// Checks that XML object is not null.
      /// </summary>
      /// <param name="xmlObject">XML object (XML element or attribute).</param>
      /// <exception cref="XmlException">Thrown when XML object is null.</exception>
      public static void CheckNotNull([CanBeNull] this XObject xmlObject)
      {
         if (xmlObject == null)
         {
            throw new XmlException("Xml element must not be null");
         }
      }

      /// <summary>
      /// Gets a value of first tag with name <paramref name="xTagName"/> from <paramref name="xElement"/>.
      /// </summary>
      /// <typeparam name="T">Type of <paramref name="xTagName"/> value.</typeparam>
      /// <param name="xElement">XML that contains  <paramref name="xTagName"/> tag.</param>
      /// <param name="xTagName">XML tag name.</param>
      /// <returns>A value of <paramref name="xTagName"/>.</returns>
      /// <exception cref="ArgumentNullException">If either <paramref name="xElement"/> or <paramref name="xTagName"/> are null.</exception>
      /// <exception cref="NullReferenceException">If there is no tag with <paramref name="xTagName"/> name.</exception>
      /// <exception cref="NotSupportedException">If <paramref name="xTagName"/> value conversion to type <typeparamref name="T"/> was failed.</exception>
      public static T GetFirstTagValue<T>(this XElement xElement, XName xTagName)
      {
         Guard.VerifyArgumentNotNull(xElement, "xElement");
         Guard.VerifyArgumentNotNull(xTagName, "xTagName");

         XElement xTag = xElement.Element(xTagName);
         Guard.VerifyNotNull(xTag, "There is no tag with '{0}' name here: {1}.", xTagName, xElement);
         string value = xTag.Value;

         return readFromString<T>(value);
      }

      /// <summary>
      /// Gets a value of first tag <paramref name="xTagName"/> from <paramref name="xElement"/>.
      /// Returns <paramref name="default"/> if this tag is not defined.
      /// </summary>
      /// <typeparam name="T">Type of <paramref name="xTagName"/> value.</typeparam>
      /// <param name="xElement">XML that contains  <paramref name="xTagName"/> tag.</param>
      /// <param name="xTagName">XML tag name.</param>
      /// <param name="default">Default value.</param>
      /// <returns>A value of <paramref name="xTagName"/>.</returns>
      /// <exception cref="ArgumentNullException">If <paramref name="xTagName"/> are null.</exception>
      /// <exception cref="NotSupportedException">If <paramref name="xTagName"/> value conversion to type <typeparamref name="T"/> was failed.</exception>
      public static T GetElementValue<T>(this XElement xElement, XName xTagName, T @default)
      {
         Guard.VerifyArgumentNotNull(xTagName, "xTagName");
         if (xElement == null)
         {
            return @default;
         }
         
         var xTag = xElement.Element(xTagName);

         if (xTag == null)
         {
            return @default;
         }

         string value = xTag.Value;

         return readFromString<T>(value);
      }

      /// <summary>
      /// Gets the xml attribute value.
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="xElement">The xml element.</param>
      /// <param name="attributeName">Name of the attribute.</param>
      /// <param name="isRequiredAttribute">if set to <c>true</c> [is required attribute].</param>
      /// <returns></returns>
      /// <exception cref="System.Xml.XmlException">
      /// </exception>
      /// <exception cref="System.InvalidCastException">
      /// </exception>
      public static T? GetXmlAttributeValue<T>(XElement xElement, string attributeName, bool isRequiredAttribute) where T : struct
      {
         T attributeValue;
         XAttribute attribute = xElement.Attribute(attributeName);

         if (attribute == null || attribute.Value == "")
         {
            if (isRequiredAttribute)
            {
               throw new XmlException(string.Format("Required attribute '{0}' wasn't found or has no value.", attributeName));
            }
            return null;
         }

         try
         {
            if (typeof(T).IsEnum)
            {
               if (!Enum.TryParse(attribute.Value, out attributeValue) || !Enum.IsDefined(typeof(T), attributeValue))
               {
                  throw new InvalidCastException();
               }
            }
            else
            {
               var converter = TypeDescriptor.GetConverter(typeof(T));
               var convertFrom = converter.ConvertFrom(attribute.Value);
               if (convertFrom != null)
               {
                  attributeValue = (T)convertFrom;
               }
               else
               {
                  throw new InvalidCastException();
               }
            }
         }
         catch (InvalidCastException e)
         {
            throw new XmlException(String.Format("Attribute '{0}' isn't properly configured.", attributeName), e);
         }

         return attributeValue;
      }

      /// <summary>
      /// Gets the required child xml element.
      /// </summary>
      /// <param name="xElement">The xml element.</param>
      /// <param name="elementName">Child xml element name.</param>
      /// <returns>Child xml element.</returns>
      /// <exception cref="XmlException"></exception>
      public static XElement GetRequiredElement(this XElement xElement, string elementName)
      {
         XElement xElementChild = xElement.Element(elementName);
         if (xElementChild == null)
         {
            throw new XmlException(string.Format("Required element \"{0}\" wasn't found.", elementName));
         }

         return xElementChild;
      }

      /// <summary>
      /// Gets the reqired xml attribute value.
      /// </summary>
      /// <param name="xElement">The xml element.</param>
      /// <param name="attributeName">Name of the attribute.</param>
      /// <returns>Attribute value.</returns>
      /// <exception cref="XmlException"></exception>
      /// <exception cref="InvalidCastException"></exception>
      public static string GetRequiredAttributeValue(this XElement xElement, string attributeName)
      {
         XAttribute attribute = xElement.Attribute(attributeName);
         if (attribute == null || attribute.Value == null)
         {
            throw new XmlException(string.Format("Required attribute '{0}' wasn't found or has no value.", attributeName));
         }

         return attribute.Value;
      }

      /// <summary>
      /// Gets the reqired xml attribute value.
      /// </summary>
      /// <typeparam name="T">Type of attribute value.</typeparam>
      /// <param name="xElement">The xml element.</param>
      /// <param name="attributeName">Name of the attribute.</param>
      /// <returns>Attribute value.</returns>
      /// <exception cref="XmlException"></exception>
      /// <exception cref="InvalidCastException"></exception>
      public static T GetRequiredAttributeValue<T>(this XElement xElement, string attributeName) where T : struct
      {
         T? nullableAttributeValue = GetXmlAttributeValue<T>(xElement, attributeName, true);
         if (!nullableAttributeValue.HasValue)
         {
            throw new XmlException(string.Format("Required attribute '{0}' wasn't found or has no value.", attributeName));
         }

         return nullableAttributeValue.Value;
      }


      /// <summary>
      /// Deserialize object using default XmlSerializer from given reader and disposes the reader
      /// </summary>
      /// <typeparam name="T">type of object to deserialize</typeparam>
      /// <param name="textReader">reader being source for given XML</param>
      /// <returns>newly created deserialized value</returns>
      public static T DeserializeFromTextAndDispose<T>([NotNull]TextReader textReader)
      {
         using (textReader)
         {
            using (var xmlReader = XmlReader.Create(textReader))
            {
               return XmlUtils.Deserialize<T>(xmlReader);
            }
         }
      }

      /// <summary>
      /// Deserialize object using default XmlSerializer from given XmlReader
      /// </summary>
      /// <typeparam name="T">type of object to deserialize</typeparam>
      /// <param name="reader">reader being source for given XML</param>
      /// <returns>newly created deserialized value</returns>
      public static T Deserialize<T>([NotNull]XmlReader reader)
      {
         //Xdocument-based xml readers does not support ReadElementContentAsBase64
         //so first serialize reader to MemoryStream and deserialize object from there
         using (Stream s = new MemoryStream())
         {
            var document = XDocument.Load(reader);
            document.Save(s);
            s.Seek(0, SeekOrigin.Begin);

            using (XmlReader newReader = XmlReader.Create(s))
            {
               return (T)Serializer<T>.Instance.Deserialize(newReader);
            }
         }
      }

      /// <summary>
      /// validates XML against schema that corresponds to serialized representation of given type
      /// </summary>
      /// <typeparam name="T">type of object to use as source for schema</typeparam>
      /// <param name="reader">reader being source for given XML</param>
      /// <param name="errorMessageWriter">writer object to store textual description of validation errors</param>
      /// <returns>bool indicating full schema conforance</returns>
      public static bool IsValidAgainstSchemaOf<T>([NotNull]XmlReader reader, [NotNull]TextWriter errorMessageWriter)
      {
         bool isValid = true;
         var document = XDocument.Load(reader);
         document.Validate(
            Serializer<T>.CompiledSchemaSet, 
            (sender, e) =>
            {
               isValid = false;
               errorMessageWriter.Write("XML validation error for type " + typeof(T).Name + ". Severity: " + e.Severity + ", problem: " + e.Message);
            }
         );
         return isValid;
      }

      /// <summary>
      /// Serializes object using default XmlSerializer 
      /// </summary>
      /// <typeparam name="T">type of object to serialize</typeparam>
      /// <param name="writer">object to write object to</param>
      /// <param name="data">object to serialize</param>
      public static void Serialize<T>([NotNull]XmlWriter writer, T data)
      {
         //Xdocument-based xml writers does not support elements stored as Base64
         //so first serialize to MemoryStream and reserialize object from there
         using (Stream xmlInMemory = new MemoryStream())
         {
            using (XmlWriter newWriter = XmlWriter.Create(xmlInMemory))
            {
               Serializer<T>.Instance.Serialize(newWriter, data);
            }
            xmlInMemory.Seek(0, SeekOrigin.Begin);
            var document = XDocument.Load(xmlInMemory);
            document.Save(writer);
         }
      }

      /// <summary>
      /// Changes xml attribute value of specified node.
      /// </summary>
      /// <param name="xml">Root XML element.</param>
      /// <param name="xPath">XPath to specified attribute.</param>
      /// <param name="newValue">New attribute value.</param>
      /// <returns><see langword="true"/> if value were changed.</returns>
      /// <exception cref="InvalidOperationException">If XML does not contains specified attribute.</exception>
      /// <exception cref="ArgumentNullException"> If <paramref name="xml"/> or <paramref name="xPath"/> is <see langword="null"/>.</exception>
      public static bool ChangeXmlAttributeValue([NotNull]this XmlNode xml, [NotNull]string xPath, string newValue)
      {
         Guard.VerifyArgumentNotNull(xml, "xml");
         Guard.VerifyArgumentNotNull(xPath, "xPath");

         var attribute = xml.SelectSingleNode(xPath);
         if (attribute == null)
         {
            throw new InvalidOperationException("Not expected XML format.")
                  {
                     Data = {{"xml", xml.InnerText}}
                  };
         }

         if (String.Compare(attribute.Value, newValue, StringComparison.InvariantCultureIgnoreCase) == 0)
         {
            return false;
         }
         attribute.Value = newValue;
         return true;
      }

      #endregion

      #region Private methods

      /// <summary>
      /// Initializes a instance of type using string representation.
      /// </summary>
      private static T readFromString<T>(string value)
      {
         var typeConverter = TypeDescriptor.GetConverter(typeof(T));
         return (T)typeConverter.ConvertFromInvariantString(value);
      }

      #endregion

      #region private static class Serializer<T>
      /// <summary>
      /// Static class to hold static XmlSerializer for any generic type.
      /// </summary>
      /// <typeparam name="T">type that is handler by serializer</typeparam>
      private static class Serializer<T>
      {
         /// <summary>
         /// intialize cached instances
         /// </summary>
         static Serializer()
         {
            var typeMapping = new XmlReflectionImporter().ImportTypeMapping(typeof(T));
            Instance = new XmlSerializer(typeMapping);
            var schemas = new XmlSchemas();
            new XmlSchemaExporter(schemas).ExportTypeMapping(typeMapping);
            var schemaSet = new XmlSchemaSet();
            foreach (XmlSchema schema in schemas)
            {
               schemaSet.Add(schema);
            }
            schemaSet.Compile();
            CompiledSchemaSet = schemaSet;
         }

         /// <summary>
         /// cached instance of serializer that handles type T
         /// </summary>
         public static XmlSerializer Instance
         {
            get; private set;
         }

         /// <summary>
         /// cached instance of serializer that handles type T
         /// </summary>
         public static XmlSchemaSet CompiledSchemaSet
         {
            get; private set;
         }
      }
      #endregion
   }
}
