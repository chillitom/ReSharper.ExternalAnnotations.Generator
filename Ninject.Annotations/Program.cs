using System;
using AnnotationGenerator;
using AnnotationGenerator.Notes;
using Ninject.Extensions.Logging;

namespace Ninject.Annotations
{
    class Program
    {
        static void Main(string[] args)
        {
            Annotator annotator = new Annotator();

            annotator.AnnotateAssemblyContaining<ILogger>(asm =>
            {
                asm.AnnotateType<ILogger>(type =>
                {
                    type.Annotate(logger => logger.Debug(FormatString._, Any<object[]>._));
                    type.Annotate(logger => logger.Debug(Any<Exception>._, FormatString._, Any<object[]>._));
                    type.Annotate(logger => logger.Info(FormatString._, Any<object[]>._));
                    type.Annotate(logger => logger.Info(Any<Exception>._, FormatString._, Any<object[]>._));
                    type.Annotate(logger => logger.Trace(FormatString._, Any<object[]>._));
                    type.Annotate(logger => logger.Trace(Any<Exception>._, FormatString._, Any<object[]>._));
                    type.Annotate(logger => logger.Warn(FormatString._, Any<object[]>._));
                    type.Annotate(logger => logger.Warn(Any<Exception>._, FormatString._, Any<object[]>._));
                    type.Annotate(logger => logger.Error(FormatString._, Any<object[]>._));
                    type.Annotate(logger => logger.Error(Any<Exception>._, FormatString._, Any<object[]>._));
                    type.Annotate(logger => logger.Fatal(FormatString._, Any<object[]>._));
                    type.Annotate(logger => logger.Fatal(Any<Exception>._, FormatString._, Any<object[]>._));
                });

                asm.AnnotateType<ILoggerFactory>(type =>
                {
                    type.Annotate(logger => logger.GetLogger(Any<Type>._), NotNull._);
                });
            });

            var version = args.Length > 0 ? args[0] : "1.0.0.0";

            annotator.CreateNugetPackage(
                new NugetSpec(
                    version: version, 
                    id: "Ninject.Annotations", 
                    title: "Ninject Annotations",
                    authors: "Tom Rathbone", 
                    owners: "Tom Rathbone",
                    projectUrl: "https://github.com/chillitom/ReSharper.ExternalAnnotations.Generator/blob/master/Ninject.Annotations/Program.cs", 
                    iconUrl: "https://raw.githubusercontent.com/ninject/ninject/master/logos/Ninject-Logo32.png", 
                    description: "External Annotations for Ninject and Ninject Extensions"));
        }
    }
}
