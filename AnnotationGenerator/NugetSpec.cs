using System.Xml.Linq;

namespace AnnotationGenerator
{
    public class NugetSpec
    {
        public NugetSpec(string id, 
            string version = "1.0.0.0", 
            string title = "Annotations", 
            string authors = "Anonymous", 
            string owners = "Anonymous",
            string projectUrl = "",
            string iconUrl = "", 
            string description = "", 
            string tags = "")
        {
            Id = id;
            Version = version;
            Title = title;
            Authors = authors;
            Owners = owners;
            ProjectUrl = projectUrl;
            IconUrl = iconUrl;
            Description = description;
            Tags = tags;
        }

        public string Id { get; private set; }
        public string Version { get; private set; }
        private string Title { get; set; }
        private string Authors { get; set; }
        private string Owners { get; set; }
        private string ProjectUrl { get; set; }
        private string IconUrl { get; set; }
        private string Description { get; set; }
        private string Tags { get; set; }

        internal XDocument GetXml()
        {
            var doc = new XDocument(
                new XDeclaration("1.0", "utf-8", null),                
                new XElement("package",
                    new XAttribute("xmlns", "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd"),
                    new XElement("metadata",
                        new XElement("id", Id),
                        new XElement("version", Version),
                        new XElement("title", Title),
                        new XElement("authors", Authors),
                        new XElement("owners", Owners),
                        new XElement("projectUrl", ProjectUrl),
                        new XElement("iconUrl", IconUrl),
                        new XElement("description", Description),
                        new XElement("tags", Tags),
                        new XElement("dependencies",
                            new XElement("dependency",
                                new XAttribute("id", "ReSharper"),
                                new XAttribute("version", "[7.0,]")))
                        )));

            return doc;
        }
    }
}