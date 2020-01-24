using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Xsl;
using VectoInputTester;

namespace System.Xml.Linq
{
    /// <summary>
    /// System.Xml.Linq.XElement and XDocument extensions
    /// </summary>

    public static class Extensions
    {
        /// <summary>
        /// Set the encoding of this XML document to the encoding given, if not set (null) default encoding is UTF8.
        /// Returns this XML document converted to it's string representation.
        /// </summary>

        public static string AsString(this XDocument xmlDoc, Encoding encoding = null)
        {
            if (encoding == null) { encoding = Encoding.UTF8; }
            using (var sw = new ExtentedStringWriter(new StringBuilder(), encoding))
            {
                using (var tx = new XmlTextWriter(sw))
                {
                    xmlDoc.WriteTo(tx);
                    return sw.ToString();
                }
            }
        }

        /// <summary>
        /// Sets the default XML namespace of this System.Xml.Linq.XElement and all its descendants
        /// </summary>

        public static void SetDefaultNamespace(this XElement element, XNamespace newXmlns)
        {
            if (newXmlns == null) { return; }
            var currentXmlns = element.GetDefaultNamespace();
            if (currentXmlns == newXmlns) { return; }

            foreach (var descendant in element.DescendantsAndSelf()
                .Where(e => e.Name.Namespace == currentXmlns)) //!important
            {
                descendant.Name = newXmlns.GetName(descendant.Name.LocalName);
            }
        }

        /// <summary>
        /// Create a copy of this XML element and set the namespace of the new element to the ns-value given
        /// </summary>

        public static XElement SetNamespace(this XElement src, XNamespace ns)
        {
            if (ns == null) { throw new NullReferenceException("Null reference namespace parameter not supported"); }

            var name = src.IsEmptyNamespace() ? ns + src.Name.LocalName : src.Name;
            var element = new XElement(name, src.Attributes(),
                  from e in src.Elements() select e.SetNamespace(ns));
            if (!src.HasElements) { element.Value = src.Value; }
            return element;
        }

        /// <summary>
        /// Test if namespace name of element is null or empty
        /// </summary>

        public static bool IsEmptyNamespace(this XElement src)
        {
            return (string.IsNullOrEmpty(src.Name.NamespaceName));
        }

        /// <summary>
        /// Create a copy of this XML element and set the default XML namespace of this System.Xml.Linq.XElement and all its descendants
        /// </summary>

        public static XElement WithDefaultXmlNamespace(this XElement xelem, XNamespace xmlns)
        {
            XName name;
            if (xelem.Name.NamespaceName == string.Empty)
            { name = xmlns + xelem.Name.LocalName; }
            else
            { name = xelem.Name; }

            XElement retelement;
            if (!xelem.Elements().Any())
            {
                retelement = new XElement(name, xelem.Value);
            }
            else
            {
                retelement = new XElement(name,
                                           from e in xelem.Elements()
                                           select e.WithDefaultXmlNamespace(xmlns));
            }

            foreach (var at in xelem.Attributes())
            {
                retelement.Add(at);
            }

            return retelement;
        }

        /// <summary>
        /// This method use XML Transformation to get only relevant information from a original XML element,
        /// we can find more information from XML Transformation in http://www.w3schools.com/xsl/xsl_examples.asp
        /// </summary>
        /// <param name="originalContent">Original XML Content to transform</param>
        /// <param name="xsltFullPath">Full Path of the Transformation file</param>
        /// <param name="argList">Optional parameter list</param>
        /// <returns>A new String with the transformed XML</returns>

        public static string RunToStringTransform(this XElement originalContent, string xsltFullPath, XsltArgumentList argList = null)
        {
            string output = string.Empty;

            using (var sri = new StringReader(originalContent.ToString()))
            {
                using (var xri = XmlReader.Create(sri))
                {
                    var xslt = new XslCompiledTransform();
                    var xsltSettings = new XsltSettings { EnableScript = true };

                    FileInfo fileInfo = null;

                    try
                    {
                        fileInfo = new FileInfo(xsltFullPath);
                    }
                    catch (Exception ex)
                    {
                        throw new FileNotFoundException("XSLT file is not found", ex);
                    }

                    // Load the Transformation Template File.
                    xslt.Load(fileInfo.FullName, xsltSettings, null);

                    // Use extended string writer to set encoding.
                    using (var sw = new ExtentedStringWriter(new StringBuilder(), Encoding.UTF8))
                    using (XmlWriter xwo = XmlWriter.Create(sw, xslt.OutputSettings))
                    {
                        if (argList != null)
                        { xslt.Transform(xri, argList, xwo); }
                        else { xslt.Transform(xri, xwo); }

                        output = sw.ToString();
                    }
                }
            }

            return output;
        }

        /// <summary>
        /// This method use XML Transformation to get only relevant information from a original XML element,
        /// we can find more information from XML Transformation in http://www.w3schools.com/xsl/xsl_examples.asp
        /// </summary>
        /// <param name="originalContent">Original XML Content to transform</param>
        /// <param name="xsltFullPath">Full Path of the Transformation file</param>
        /// <param name="argList">Optional parameter list</param>
        /// <returns>A new XElement with the transformed XML</returns>

        public static XElement RunToXElementTransform(this XElement originalContent, string xsltFullPath, XsltArgumentList argList = null)
        {
            XElement output;

            using (var sri = new StringReader(originalContent.ToString()))
            {
                using (var xri = XmlReader.Create(sri))
                {
                    var xslt = new XslCompiledTransform();
                    var xsltSettings = new XsltSettings { EnableScript = true };

                    FileInfo fileInfo = null;

                    try
                    {
                        fileInfo = new FileInfo(xsltFullPath);
                    }
                    catch (Exception ex)
                    {
                        throw new FileNotFoundException("XSLT file is not found", ex);
                    }

                    // Load the Transformation Template File.
                    xslt.Load(fileInfo.FullName, xsltSettings, null);

                    // Use extended string writer to set encoding.
                    using (var sw = new ExtentedStringWriter(new StringBuilder(), Encoding.UTF8))
                    using (XmlWriter xwo = XmlWriter.Create(sw, xslt.OutputSettings))
                    {
                        if (argList != null)
                        { xslt.Transform(xri, argList, xwo); }
                        else { xslt.Transform(xri, xwo); }

                        output = XElement.Parse(sw.ToString());
                    }
                }
            }

            return output;
        }

        /// <summary>
        /// This method use XML Transformation to get only relevant information from a original XML element,
        /// we can find more information from XML Transformation in http://www.w3schools.com/xsl/xsl_examples.asp
        /// </summary>
        /// <param name="originalContent">Original XML Content to transform</param>
        /// <param name="xsltString">The transformation style sheet to use</param>
        /// <param name="argList">Optional parameter list</param>
        /// <returns>A new XElement with the transformed XML</returns>

        public static XElement ParseXsltTransform(this XElement originalContent, string xsltString, XsltArgumentList argList = null)
        {
            XElement output;
            if (string.IsNullOrEmpty(xsltString)) { throw new ArgumentException("XSLT input string is null or empty", "xsltString"); }

            using (var sri = new StringReader(originalContent.ToString()))
            using (var xri = XmlReader.Create(sri))
            {
                var xslt = new XslCompiledTransform();
                var xsltSettings = new XsltSettings { EnableScript = true };

                using (var xmlReader = new StringReader(xsltString))
                using (var reader = XmlReader.Create(xmlReader))
                {
                    // Load the Transformation Template File.
                    xslt.Load(reader, xsltSettings, null);

                    // Use extended string writer to set encoding.
                    using (var sw = new ExtentedStringWriter(new StringBuilder(), Encoding.UTF8))
                    using (var xwo = XmlWriter.Create(sw, xslt.OutputSettings))
                    {
                        if (argList != null)
                        { xslt.Transform(xri, argList, xwo); }
                        else { xslt.Transform(xri, xwo); }

                        output = XElement.Parse(sw.ToString());
                    }
                }
            }

            return output;
        }
    }
}
