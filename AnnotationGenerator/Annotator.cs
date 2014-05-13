using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace AnnotationGenerator
{
    public class Annotator
    {
        readonly IList<XDocument> _documents = new List<XDocument>();

        public void AnnotateAssemblyContaining<T>(Action<AssemblyAnnotator<T>> annotationActions)
        {
            annotationActions(new AssemblyAnnotator<T>(this));
        }

        internal IEnumerable<XDocument> GetDocuments()
        {
            return _documents;
        }

        internal void AddDocument(XDocument document)
        {
            _documents.Add(document);
        }

        public void CreateNugetPackage(NugetSpec spec)
        {
            var directory = Path.Combine(spec.Title, "ReSharper", "vAny", "annotations");

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            WriteSpecFile(spec);
            WriteAnnotationFiles(spec);
        }

        private static void WriteSpecFile(NugetSpec spec)
        {
            var specFilename = spec.Title + "." + spec.Version + ".nuspec";
            var specFilePath = Path.Combine(spec.Title, specFilename);

            using (var writer = new XmlTextWriter(specFilePath, Encoding.UTF8) {Formatting = Formatting.Indented})
            {
                spec.GetXml().WriteTo(writer);
            }
        }

        private void WriteAnnotationFiles(NugetSpec spec)
        {
            foreach (var document in _documents)
            {
                var assemblyName = document.XPathSelectElement("/assembly").Attribute("name").Value;
                var filename = assemblyName + ".xml";                

                var path = Path.Combine(spec.Title, "ReSharper", "vAny", "annotations", filename);

                using (var writer = new XmlTextWriter(path, Encoding.UTF8) {Formatting = Formatting.Indented})
                {
                    document.WriteTo(writer);
                }
            }
        }
    }
}