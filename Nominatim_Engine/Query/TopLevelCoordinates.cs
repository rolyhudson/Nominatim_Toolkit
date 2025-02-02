﻿using BH.oM.Base;
using BH.oM.Geospatial;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Adapter.Nominatim
{
    public static partial class Query
    {
        /***************************************************/
        /****           Public Methods                  ****/
        /***************************************************/

        [Description("Extract the top level coordinates from a CustomObject from GeoJSON formatted string.")]
        public static object TopLevelCoordinates(this CustomObject customObject)
        {
            if (customObject.CustomData.ContainsKey("coordinates"))
                return customObject.CustomData["coordinates"];
            Base.Compute.RecordWarning("Coordinates could not be extracted.");
            return null;
        }

        /***************************************************/

    }
}
