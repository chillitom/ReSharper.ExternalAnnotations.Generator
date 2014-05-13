namespace AnnotationGenerator.Notes
{
    public class FormatString : IParameterNote
    {
        public FormatString(string parameterName)
        {
            ParameterName = parameterName;
        }

        internal string ParameterName { get; private set; }

        public readonly static string _ = "FormatString";
    }
}