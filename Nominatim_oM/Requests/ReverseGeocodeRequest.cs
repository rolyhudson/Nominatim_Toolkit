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
    [Description("Define the OpenStreetMap Nominatim Request for reverse geocoding from a location specified by latitude and longitude." +
            "Reverse geocoding API does not exactly compute the address for the coordinate it receives. It works by finding the closest suitable OpenStreetMap object and returning its address information. " +
            "This may occasionally lead to unexpected results." +
            "The API returns exactly one result or an error when the coordinate is in an area with no OSM data coverage.")]
    public class ReverseGeocodeRequest : BHoMObject, INominatimRequest
    {
        [Description("Latitude of a coordinate in WGS84 projection. Permitted range is -90 to 90.")]
        public virtual double Longitude { get; set; } = 0;

        [Description("Longitude of a coordinate in WGS84 projection. Permitted range is -180 to 180.")]
        public virtual double Latitude { get; set; } = 0;
    }
}
