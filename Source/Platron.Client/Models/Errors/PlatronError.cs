namespace Platron.Client
{
    public sealed class PlatronError
    {
        public PlatronError(int code, string description)
        {
            Code = code;
            Description = description;
        }

        public int Code { get; }
        public string Description { get; }

        /// <summary>
        ///     Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        ///     A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return $"Code: {Code}, Description: {Description}";
        }
    }
}