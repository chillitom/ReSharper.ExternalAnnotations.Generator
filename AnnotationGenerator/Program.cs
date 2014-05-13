using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Ninject.Extensions.Logging;

namespace AnnotationGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            NinjectAnnotationFilesGenerator.Generate();
        }
    }

    class NinjectAnnotationFilesGenerator
    {
        private const string TargetFolder = @"..\..\..\ReSharper\vAny\annotations\";

        public static void Generate()
        {
            WriteDocument("Ninject.Extensions.Logging.xml", new LoggingStringFormatAnnotationGenerator().Generate());
        }

        private static void WriteDocument(string name, XDocument xml)
        {
            string filename = Path.Combine(TargetFolder, name);

            using (var writer = new XmlTextWriter(filename, Encoding.UTF8) {Formatting = Formatting.Indented})
            {
                xml.WriteTo(writer);
            }
        }
    }

    class LoggingStringFormatAnnotationGenerator
    {
        public XDocument Generate()
        {
            return new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("assembly", 
                    new XAttribute("name", "Ninject.Extensions.Logging"),
                    GenerateMembersFromReflection())
                );            
        }

        private IEnumerable<XElement> GenerateMembersFromReflection()
        {
            var type = typeof (ILogger);

            var methods = type.GetMethods();

            foreach (var methodInfo in methods)
            {
                ParameterInfo[] parameters = methodInfo.GetParameters();

                var formatParameter = parameters.FirstOrDefault(p => p.Name == "format");

                if (formatParameter != null)
                {
                    var name = "M:Ninject.Extensions.Logging.ILogger." + methodInfo.Name + "(" + ParameterTypes(methodInfo) + ")";

                    yield return new XElement("member", 
                        new XAttribute("name", name),
                        new XElement("attribute", 
                            new XAttribute("ctor", "M:JetBrains.Annotations.StringFormatMethodAttribute.#ctor(System.String)"),
                            new XElement("argument", "format")));
                }
            }
        }

        private string ParameterTypes(MethodInfo methodInfo)
        {
            return string.Join(",", methodInfo.GetParameters().Select(p => p.ParameterType.FullName));
        }                
    }
}
