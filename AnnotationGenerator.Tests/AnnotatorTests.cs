using System;
using System.Linq;
using System.Xml.XPath;
using AnnotationGenerator.Notes;
using Ninject.Extensions.Logging;
using NUnit.Framework;

namespace AnnotationGenerator.Tests
{
    public class AnnotatorTests
    {
        [Test]
        public void CreatesAssemblyElement()
        {
            Annotator annotator = new Annotator();

            annotator.AnnotateAssemblyContaining<ILogger>(asm => {});

            var doc = annotator.GetDocuments().First();
            var assemblyElement = doc.XPathSelectElement("/assembly");

            Assert.That(assemblyElement, Is.Not.Null);
        }

        [Test]
        public void SetsAssemblyElementNameAttribute()
        {
            Annotator annotator = new Annotator();

            annotator.AnnotateAssemblyContaining<ILogger>(asm => {});

            var doc = annotator.GetDocuments().First();
            var assemblyElement = doc.XPathSelectElement("/assembly[@name=\"Ninject.Extensions.Logging\"]");

            Assert.That(assemblyElement, Is.Not.Null);
        }

        [Test]
        public void CreatesMemberElementIfParameterAnnotated()
        {
            Annotator annotator = new Annotator();

            annotator.AnnotateAssemblyContaining<ILogger>(asm =>
            {
                asm.AnnotateType<ILogger>(type => type.Annotate(i => i.Info(FormatString._, Any<object[]>._)));
            });

            var doc = annotator.GetDocuments().First();
            var memberElement = doc.XPathSelectElement("/assembly/member");

            Assert.That(memberElement, Is.Not.Null);
        }        
        
        [Test]
        public void SetsMemberElementName()
        {
            Annotator annotator = new Annotator();

            annotator.AnnotateAssemblyContaining<ILogger>(asm =>
            {
                asm.AnnotateType<ILogger>(type => type.Annotate(i => i.Info(FormatString._, Any<object[]>._)));
            });

            var doc = annotator.GetDocuments().First();
            var memberElement = doc.XPathSelectElement("/assembly/member");

            Assert.That(memberElement, Is.Not.Null);
            Assert.That(memberElement.Attribute("name").Value, Is.EqualTo("M:Ninject.Extensions.Logging.ILogger.Info(System.String,System.Object[])"));
        }        
        
        [Test]
        public void CreatesMemberElementIfMethodAnnotated()
        {
            Annotator annotator = new Annotator();

            annotator.AnnotateAssemblyContaining<ILogger>(asm =>
            {
                asm.AnnotateType<ILoggerFactory>(type => type.Annotate(i => i.GetLogger(Any<string>._), NotNull._));
            });

            var doc = annotator.GetDocuments().First();
            var memberElement = doc.XPathSelectElement("/assembly/member");

            Assert.That(memberElement, Is.Not.Null);
        }

        [Test]
        public void ThrowsExceptionIfAnnotationContainsNoAdvice()
        {
            Assert.Throws<Exception>(() =>
            {
                Annotator annotator = new Annotator();

                annotator.AnnotateAssemblyContaining<ILogger>(asm =>
                {
                    asm.AnnotateType<ILogger>(type => type.Annotate(i => i.Info(Any<string>._, Any<object[]>._)));
                });
            });
        }

        [Test]
        public void CreatesStringFormatMethodAttribute()
        {
            Annotator annotator = new Annotator();

            annotator.AnnotateAssemblyContaining<ILogger>(asm =>
            {
                asm.AnnotateType<ILogger>(type => type.Annotate(i => i.Info(FormatString._, Any<object[]>._)));
            });

            var doc = annotator.GetDocuments().First();
            var attributeElement = doc.XPathSelectElement("/assembly/member/attribute");

            Assert.That(attributeElement, Is.Not.Null);
        }

        [Test]
        public void SetsAttributeCtorAttribute()
        {
            Annotator annotator = new Annotator();

            annotator.AnnotateAssemblyContaining<ILogger>(asm =>
            {
                asm.AnnotateType<ILogger>(type => type.Annotate(i => i.Info(FormatString._, Any<object[]>._)));
            });

            var doc = annotator.GetDocuments().First();
            var attributeElement = doc.XPathSelectElement("/assembly/member/attribute");

            Assert.That(attributeElement, Is.Not.Null);
            Assert.That(attributeElement.Attribute("ctor").Value, Is.EqualTo("M:JetBrains.Annotations.StringFormatMethodAttribute.#ctor(System.String)"));
        }
        
        [Test]
        public void CreatesStringFormatMethodArgumentElement()
        {
            Annotator annotator = new Annotator();

            annotator.AnnotateAssemblyContaining<ILogger>(asm =>
            {
                asm.AnnotateType<ILogger>(type => type.Annotate(i => i.Info(FormatString._, Any<object[]>._)));
            });

            var doc = annotator.GetDocuments().First();
            var argumentElement = doc.XPathSelectElement("/assembly/member/attribute/argument");

            Assert.That(argumentElement, Is.Not.Null);
            Assert.That(argumentElement.FirstNode.ToString(), Is.EqualTo("format"), "content should match method argument name");
        }

        [Test]
        public void CreatesNotNullAnnotationWhenAppliedToMethod()
        {
            Annotator annotator = new Annotator();

            annotator.AnnotateAssemblyContaining<ILogger>(asm =>
            {
                asm.AnnotateType<ILoggerFactory>(type => type.Annotate(i => i.GetLogger(Any<string>._), NotNull._));
            });

            var doc = annotator.GetDocuments().First();
            var attributeElement = doc.XPathSelectElement("/assembly/member/attribute");

            Assert.That(attributeElement, Is.Not.Null);
            Assert.That(attributeElement.Attribute("ctor").Value, Is.EqualTo("M:JetBrains.Annotations.NotNullAttribute.#ctor"));
        }

    }
}
