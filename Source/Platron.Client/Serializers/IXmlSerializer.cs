namespace Platron.Client.Serializers
{
    public interface IXmlSerializer
    {
        string Serialize(object value, string root);
    }
}