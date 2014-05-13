namespace AnnotationGenerator.Notes
{
    public class NotNull : IMemberNote
    {
        private NotNull()
        {            
        }

        public static NotNull _
        {
            get { return new NotNull(); }
        }
    }

    public class NotNull<T> : IParameterNote
    {
        private NotNull()
        { }

        public static NotNull<T> _
        {
            get { return new NotNull<T>(); }
        }
    }
}