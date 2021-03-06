using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEditor.Localization.Plugins.XLIFF.Common;

namespace UnityEditor.Localization.Plugins.XLIFF
{
    public enum XliffVersion
    {
        /// <summary>
        /// XLIFF Version 1.2
        /// </summary>
        V12,

        /// <summary>
        /// XLIFF Version 2.0
        /// </summary>
        V20
    }

    public static class XliffDocument
    {
        public static IXliffDocument Create(XliffVersion version)
        {
            IXliffDocument xdoc;
            if (version == XliffVersion.V12)
            {
                xdoc = new V12.xliff
                {
                    version = V12.AttrType_Version.Item12
                };
            }
            else
            {
                xdoc =  new V20.xliff
                {
                    version = "2.0"
                };
            }

            return xdoc;
        }

        public static IXliffDocument Parse(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            // First parse the version
            var version = GetVersionFromXml(stream);

            Type rootType;
            if (version == "1.2")
                rootType = typeof(V12.xliff);
            else if (version == "2.0")
                rootType = typeof(V20.xliff);
            else
                throw new NotSupportedException($"Unsupported XLIFF version {version}. Supported versions are 1.1 and 2.0");

            var ser = new XmlSerializer(rootType);
            return ser.Deserialize(stream) as IXliffDocument;
        }

        static string GetVersionFromXml(Stream stream)
        {
            var currentPos = stream.Position;
            var reader = XmlReader.Create(stream, new XmlReaderSettings());
            reader.MoveToContent();

            var version = reader.GetAttribute("version");
            if (version == null)
                throw new XmlException("Invalid XLIFF file. Could not determine the version.");

            // Reset stream position
            stream.Position = currentPos;

            return version;
        }
    }
}
