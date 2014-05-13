using System;
using System.Xml.Linq;

namespace AnnotationGenerator
{
    public class AssemblyAnnotator<TAsm>
    {
        private readonly Annotator _annotator;
        private readonly XElement _rootElement;

        internal AssemblyAnnotator(Annotator annotator)
        {
            _annotator = annotator;

            _rootElement = CreateDocument();
        }

        private XElement CreateDocument()
        {
            var element = new XElement("assembly",
                new XAttribute("name", typeof (TAsm).Assembly.GetName().Name));

            var document = new XDocument(
                new XDeclaration("1.0", "utf-8", null), element);

            _annotator.AddDocument(document);

            return element;
        }

        internal void AddElement(XElement element)
        {
            _rootElement.Add(element);
        }

        public void AnnotateType<TType>(Action<MemberAnnotator<TType,TAsm>> annotationActions)
        {
            annotationActions(new MemberAnnotator<TType,TAsm>(this));
        }
    }
}