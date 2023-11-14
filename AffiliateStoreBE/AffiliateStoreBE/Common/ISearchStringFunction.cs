namespace AffiliateStoreBE.Common
{
    public interface ISearchStringFunction
    {
        List<string> SearchString(string stringInput);
        string RemoveSpaceAndConvert(string listStringInput);
    }
}
