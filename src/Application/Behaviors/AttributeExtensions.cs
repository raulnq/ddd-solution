namespace Application
{
    public static class AttributeExtensions
    {
        public static bool ExistsAtrribute<TAttribute>(this Type type)
            where TAttribute : Attribute
        {
            var att = type.GetCustomAttributes(typeof(TAttribute), true);

            return !(att == null || att.Length == 0);
        }
    }
}
