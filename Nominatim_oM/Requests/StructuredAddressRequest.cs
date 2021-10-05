using BH.oM.Base;
using BH.oM.Data.Requests;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.oM.Adapter.Nominatim
{
    [Description("Create the OpenStreetMap Nominatim Request for structured address search.  " +
        "Not all parameters are required. Structured requests are faster but are less robust against alternative OpenStreetMap tagging schemas.")]
    public class StructuredAddressRequest : BHoMObject, INominatimRequest
    {
        public virtual string Street { get; set; } = "";
        public virtual string City { get; set; } = "";
        public virtual string County { get; set; } = "";
        public virtual string State { get; set; } = "";
        public virtual string Country { get; set; } = "";
        public virtual string PostalCode { get; set; } = "";
    }
}
