﻿using BH.oM.Geospatial;
using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace BH.Engine.Adapter.Nominatim
{
    public static partial class Convert
    {
        /***************************************************/
        /****           Public Methods                  ****/
        /***************************************************/

        [Description("Convert a CustomObject based on a GeoJSON formatted string to BHoM Geospatial Feature.")]
        public static Feature ToFeature(CustomObject customObject)
        {
            object fType;
            customObject.CustomData.TryGetValue("type", out fType);

            if (fType == null || ((string)fType).ToLower() != "feature")
            {
                Base.Compute.RecordError("Object was not a feature type.");
                return null;
            }

            Feature feature = new Feature();
            if (customObject.CustomData.ContainsKey("properties"))
            {
                object props;
                customObject.CustomData.TryGetValue("properties", out props);
                feature.Properties = ((CustomObject)props).CustomData;
            }
            if (customObject.CustomData.ContainsKey("geometry"))
            {
                CustomObject geometry = (CustomObject)customObject.CustomData["geometry"];
                feature.Geometry = ToGeospatial(geometry);
            }
            if (customObject.CustomData.ContainsKey("bbox"))
            {
                //box should be list of coordinates per https://datatracker.ietf.org/doc/html/rfc7946#section-5
                List<object> bbox = (List<object>)customObject.CustomData["bbox"];
                feature.BoundingBox = (BoundingBox)ToBoundingBox(bbox);
            }
            return feature;

        }

        /***************************************************/
        
    }
}
