using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.oM.Adapter.Nominatim
{
    [Description("Define configuration settings for pulling Nominatim data using the Nominatim Adapter")]
    public class NominatimConfig : ActionConfig
    {
        [Description("The data format for places found. Default is GeoJSON. Use GeoJSON to take advantage of converts to GeoJSON based Geospatial objects in the BHoM.")]
        public virtual OutputFormat OutputFormat { get; set; } = OutputFormat.GeoJSON;

        [Description("Details to be included in the returned data of places found. By default all details are included.")]
        public virtual OutputDetails OutputDetails { get; set; } = new OutputDetails();

        [Description("Limit the results to certain areas or number of results. The default search is zoom detail of 18. Other ResultLimitation parameters are not applicable to reverse geocoding.")]
        public virtual ResultLimitation ResultLimitation { get; set; } = new ResultLimitation();

        [Description("Format of the polygon geometry of the places found. Default is GeoJSON. Use GeoJSON to take advantage of converts to GeoJSON based Geospatial objects in the BHoM.")]
        public virtual PolygonOutput PolygonOutput { get; set; } = PolygonOutput.GeoJSON;
    }
}
