using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Mvc;

namespace AffiliateStoreBE.Common
{
    public class ApiRespone<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public T Result { get; set; }
    }
}
