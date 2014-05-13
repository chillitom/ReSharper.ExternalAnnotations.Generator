using System;
using System.Linq.Expressions;
using System.Reflection;
using AnnotationGenerator.Notes;

namespace AnnotationGenerator
{
    internal static class ExpressionNoteExtractor
    {
        public static INote FromExpression(Expression expression, ParameterInfo parameter)
        {
            MemberExpression memberExpression = expression as MemberExpression;
            if (memberExpression != null)
            {
                if (typeof (FormatString).IsAssignableFrom(memberExpression.Member.DeclaringType))
                {
                    return new FormatString(parameter.Name);
                }
            }
            throw new Exception("Couldn't convert expressions to note " + expression);
        }
    }
}