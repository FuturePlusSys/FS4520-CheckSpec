using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// Validation tests attributes
/// </summary>
///
///https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/attributes/accessing-attributes-by-using-reflection
///
namespace DP14ValidationAttributes_ClassLibrary
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]  // Multiuse attribute.

    public class DP14ValidationAttributes : System.Attribute
    {
        public string CategoryID { get; set; } = "Testing";

        public string ProtocolID { get; set; } = "SST";

        public int ListOrder { get; set; } = 0;

        public DP14ValidationAttributes(string catID, string pID, int orderID)
        {
            CategoryID = catID;
            ProtocolID = pID;
            ListOrder = orderID;
        }
    }
}
