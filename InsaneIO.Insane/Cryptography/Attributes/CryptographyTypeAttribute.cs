namespace InsaneIO.Insane.Cryptography.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class CryptographyTypeAttribute : Attribute
    {
        public string Identifier { get; }

        public CryptographyTypeAttribute(string identifier)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(identifier);
            Identifier = identifier;
        }
    }
}
