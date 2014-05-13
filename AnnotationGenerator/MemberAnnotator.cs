using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml.Linq;
using AnnotationGenerator.Notes;

namespace AnnotationGenerator
{
    public class MemberAnnotator<TClass,TAsm>
    {
        private readonly AssemblyAnnotator<TAsm> _assemblyAnnotator;

        internal MemberAnnotator(AssemblyAnnotator<TAsm> assemblyAnnotator)
        {
            _assemblyAnnotator = assemblyAnnotator;
        }

        public void Annotate(Expression<Action<TClass>> expression, params IMemberNote[] memberNotes)
        {
            var methodInfo = GetMethodInfo(expression);

            var parameterNotes = GetNotesFromExpression(expression);

            if (parameterNotes.Any() || memberNotes.Any())
            {
                var memberAnnotations = GetMemberAnnotations(memberNotes, parameterNotes);
                var parameterAnnotations = GetParameterAnnotations(parameterNotes);

                _assemblyAnnotator.AddElement(new XElement("member",
                    new XAttribute("name", GetMethodNameString(methodInfo)),
                    memberAnnotations, parameterAnnotations));
            }
            else
            {
                throw new Exception("Expected some advice about method or member of method " + methodInfo);
            }
        }

        private string GetMethodNameString(MethodInfo methodInfo)
        {
            string parameters = string.Join(",", methodInfo.GetParameters().Select(p => p.ParameterType.FullName));

            Type declaringType = methodInfo.DeclaringType;

            return string.Format("M:{0}.{1}({2})", declaringType.FullName, methodInfo.Name, parameters);
        }

        private IEnumerable<XElement> GetParameterAnnotations(IEnumerable<INote> parameterNotes)
        {
            yield break;
        }

        private IEnumerable<XElement> GetMemberAnnotations(INote[] memberNotes, IEnumerable<INote> parameterNotes)
        {
            foreach (var memberNote in memberNotes)
            {
                if (memberNote is NotNull)
                {
                    yield return new XElement("attribute", new XAttribute("ctor", "M:JetBrains.Annotations.NotNullAttribute.#ctor"));
                }
            }

            foreach (var parameterNote in parameterNotes)
            {
                if (parameterNote is FormatString)
                {
                    FormatString formatStringNote = (FormatString) parameterNote;
                    yield return new XElement("attribute", 
                        new XAttribute("ctor", "M:JetBrains.Annotations.StringFormatMethodAttribute.#ctor(System.String)"),
                        new XElement("argument", formatStringNote.ParameterName));
                }
            }
        }

        public IEnumerable<INote> GetNotesFromExpression(Expression<Action<TClass>> expression)
        {
            var methodCallExpression = expression.Body as MethodCallExpression;
            if (methodCallExpression != null)
            {
                int param = 0;
                var parameters = methodCallExpression.Method.GetParameters();

                foreach (var arg in methodCallExpression.Arguments)
                {
                    var parameter = parameters[param];
                    param++;

                    var memberExpression = arg as MemberExpression;

                    var declaringType = memberExpression.Member.DeclaringType;

                    if (typeof(INote).IsAssignableFrom(declaringType))
                    {
                        yield return ExpressionNoteExtractor.FromExpression(arg, parameter);
                    }                    
                }
            }
        }

        public static MethodInfo GetMethodInfo<T, U>(Expression<Func<T, U>> expression)
        {
            var methodCallExpression = expression.Body as MethodCallExpression;
            if (methodCallExpression != null)
                return methodCallExpression.Method;

            throw new ArgumentException("Expression is not a member access", "expression");
        }

        public static MethodInfo GetMethodInfo<T>(Expression<Action<T>> expression)
        {
            var methodCallExpression = expression.Body as MethodCallExpression;
            if (methodCallExpression != null)
                return methodCallExpression.Method;

            throw new ArgumentException("Expression is not a member access", "expression");
        }
    }
}