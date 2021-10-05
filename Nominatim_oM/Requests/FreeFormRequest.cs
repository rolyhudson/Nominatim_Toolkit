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
    [Description("Define a OpenStreetMap Nominatim GetRequest from a free-form string. " +
            "Free-form queries are processed first left-to-right and then right-to-left if that fails. " +
            "So you may search for pilkington avenue, birmingham as well as for birmingham, pilkington avenue. " +
            "Commas are optional, but improve performance by reducing the complexity of the search." +
            "Special phrases can cause Nominatim to search for particular object types see https://wiki.openstreetmap.org/wiki/Nominatim/Special_Phrases/EN for more details.")]
    public class FreeFormRequest : BHoMObject, INominatimRequest
    {
        [Description("Free-form query string to search for.")]
        public virtual string FreeFormQuery { get; set; } = "";
    }
}
