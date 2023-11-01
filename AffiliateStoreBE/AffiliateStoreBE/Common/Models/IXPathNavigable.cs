using System.Xml.XPath;

namespace AffiliateStoreBE.Common.Models
{
    //
    // Summary:
    //     Provides an accessor to the System.Xml.XPath.XPathNavigator class.
    public interface IXPathNavigable
    {
        //
        // Summary:
        //     Returns a new System.Xml.XPath.XPathNavigator object.
        //
        // Returns:
        //     An System.Xml.XPath.XPathNavigator object.
        XPathNavigator? CreateNavigator();
    }
}
