namespace Platron.Client
{
    public sealed class PlatronError
    {
        public PlatronError(int code, string description) : this((ErrorCode)code, description)
        {
        }

        public PlatronError(ErrorCode code, string description)
        {
            Code = code;
            Description = description;
        }

        public ErrorCode Code { get; }
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