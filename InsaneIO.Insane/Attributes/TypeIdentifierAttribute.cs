namespace InsaneIO.Insane.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class TypeIdentifierAttribute : Attribute
    {
        public string Identifier { get; }

        public TypeIdentifierAttribute(string identifier)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(identifier);
            Identifier = identifier;
        }
    }
}
