namespace System
{
    [Serializable]
    public sealed class ExpectedException : Exception
    {
        public ExpectedException(string message) : base(message) { }
    }
}
