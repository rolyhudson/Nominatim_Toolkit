using BH.oM.Adapter.Nominatim;
using BH.oM.Adapters.HTTP;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Adapter.Nominatim
{
    public static partial class Create
    {
        [Description("Create a OpenStreetMap Nominatim GetRequest from a free-form string. " +
            "Free-form queries are processed first left-to-right and then right-to-left if that fails. " +
            "So you may search for pilkington avenue, birmingham as well as for birmingham, pilkington avenue. " +
            "Commas are optional, but improve performance by reducing the complexity of the search." +
            "Special phrases can cause Nominatim to search for particular object types see https://wiki.openstreetmap.org/wiki/Nominatim/Special_Phrases/EN for more details.")]
        [Input("FreeFormRequest", "FreeFormRequest with Free-form query string to search for.")]
        [Input("NominatimConfig", "Optional settings for pulling nominatim data.")]
        [Output("getRequest", "The GetRequest.")]
        public static GetRequest GetRequest(FreeFormRequest request, NominatimConfig config = null)
        {
            if (String.IsNullOrEmpty(request.FreeFormQuery))
            {
                Base.Compute.RecordError("The freeFormQuery cannot be null or empty.");
                return null;
            }

            if (config == null)
                config = config = new NominatimConfig();
            
            GetRequest getrequest = new GetRequest() { BaseUrl = BaseUriNominatimSearch() };

            getrequest.Parameters.Add("q", request.FreeFormQuery.Replace(" ", "+"));

            SetParameters(ref getrequest, config);

            AddUserAgentHeader(ref getrequest);

            return getrequest;
        }

        /***************************************************/

        [Description("Create the OpenStreetMap Nominatim GetRequest from a StructuredAddressRequest.")]
        [Input("StructuredAddressRequest", "Query structured by address parameters from a StructuredAddressRequest.")]
        [Input("NominatimConfig", "Optional settings for pulling nominatim data.")]
        [Output("getRequest", "The GetRequest.")]
        public static GetRequest GetRequest(StructuredAddressRequest request, NominatimConfig config = null)
        {
            if (request == null)
            {
                Base.Compute.RecordError("The addressSearch cannot be null");
                return null;
            }
            if (config == null)
                config = config = new NominatimConfig();

            GetRequest getrequest = new GetRequest() { BaseUrl = BaseUriNominatimSearch() };

            request.AddToParameters(ref getrequest);

            SetParameters(ref getrequest, config);

            AddUserAgentHeader(ref getrequest);

            return getrequest;
        }

        /***************************************************/

        [Description("Create the OpenStreetMap Nominatim GetRequest for reverse geocoding from a location specified by latitude and longitude." +
            "Reverse geocoding API does not exactly compute the address for the coordinate it receives. It works by finding the closest suitable OpenStreetMap object and returning its address information. " +
            "This may occasionally lead to unexpected results." +
            "The API returns exactly one result or an error when the coordinate is in an area with no OSM data coverage.")]
        [Input("latitude", "Latitude of a coordinate in WGS84 projection. Permitted range is -90 to 90.")]
        [Input("longitude", "Longitude of a coordinate in WGS84 projection. Permitted range is -180 to 180.")]
        
        [Output("getRequest", "The GetRequest.")]
        public static GetRequest GetRequest(ReverseGeocodeRequest request, NominatimConfig config = null)
        {
            if (request.Latitude < -90 || request.Latitude > 90 || request.Longitude < -180 || request.Longitude > 180)
            {
                Base.Compute.RecordWarning("The latitude or longitude provided is outside the permitted range. Latitude range is -90 to 90. Longitude range is -180 to 180.");
                return null;
            }
            if (config == null)
                config = config = new NominatimConfig();

            GetRequest getrequest = new GetRequest() { BaseUrl = BaseUriNominatimReverse() };

            getrequest.Parameters.Add("lat", request.Latitude);
            getrequest.Parameters.Add("lon", request.Longitude);

            SetParameters(ref getrequest, config);

            AddUserAgentHeader(ref getrequest);

            return getrequest;
        }

        /***************************************************/
        /****           Private Methods                 ****/
        /***************************************************/

        private static string BaseUriNominatimSearch()
        {
            return "https://nominatim.openstreetmap.org/search?";
        }

        /***************************************************/

        private static string BaseUriNominatimReverse()
        {
            return "https://nominatim.openstreetmap.org/reverse?";
        }

        /***************************************************/

        private static void AddUserAgentHeader(ref GetRequest request)
        {
            //to conform with usage policy we provide a user agent in the header see https://operations.osmfoundation.org/policies/nominatim/
            request.Headers.Add("User-Agent", "BHoM.xyz");
        }

        /***************************************************/

        private static void AddToParameters(this OutputDetails outputDetails, ref GetRequest request)
        {
            if (outputDetails == null)
                return;

            if (outputDetails.AddressDetails)
                request.Parameters.Add("addressdetails", 1);

            if (outputDetails.NameDetails)
                request.Parameters.Add("namedetails", 1);

            if (outputDetails.ExtraTags)
                request.Parameters.Add("extratags", 1);

        }

        /***************************************************/

        private static void AddToParameters(this ResultLimitation resultLimitation, ref GetRequest request)
        {
            if (resultLimitation == null)
                return;

            //for reverse calls we can only use Zoom limit
            if (request.BaseUrl.Contains("reverse"))
            {
                if (resultLimitation.Zoom >= 0 && resultLimitation.Zoom <= 18)
                    request.Parameters.Add("zoom", resultLimitation.Zoom);
                else
                    Base.Compute.RecordWarning("Zoom level of detail is outside of range (0 to 18), zoom detail is set to 18.");
                return;
            }

            if (resultLimitation.Limit > 50)
            {
                Base.Compute.RecordWarning("Maximum result limit is 50, limit set to 50.");
                request.Parameters.Add("limit", 50);
            }
            else if (resultLimitation.Limit < 0)
            {
                Base.Compute.RecordWarning("Result limit cannot be less than 1, limit set to 1.");
                request.Parameters.Add("limit", 1);
            }
            else
                request.Parameters.Add("limit", resultLimitation.Limit);

            if (resultLimitation.CountryCodes.Count > 0)
                request.Parameters.Add("countrycodes", String.Join(",", resultLimitation.CountryCodes));

            if (resultLimitation.BoundingBox != null)
                request.Parameters.Add("viewbox", $"" +
                    $"{resultLimitation.BoundingBox.Min.Longitude}," +
                    $"{resultLimitation.BoundingBox.Min.Latitude}," +
                    $"{resultLimitation.BoundingBox.Max.Longitude}," +
                    $"{resultLimitation.BoundingBox.Max.Latitude}");

            if (resultLimitation.ExcludedPlaceIds.Count > 0)
                request.Parameters.Add("exclude_place_ids", String.Join(",", resultLimitation.ExcludedPlaceIds.Select(x => x.ToString()).ToList()));
        }

        /***************************************************/

        private static void AddToParameters(this StructuredAddressRequest addressRequest, ref GetRequest request)
        {
            if (addressRequest == null)
                return;

            if (!String.IsNullOrEmpty(addressRequest.Country))
                request.Parameters.Add("country", addressRequest.Country);

            if (!String.IsNullOrEmpty(addressRequest.County))
                request.Parameters.Add("county", addressRequest.County);

            if (!String.IsNullOrEmpty(addressRequest.City))
                request.Parameters.Add("city", addressRequest.City);

            if (!String.IsNullOrEmpty(addressRequest.State))
                request.Parameters.Add("state", addressRequest.State);

            if (!String.IsNullOrEmpty(addressRequest.Street))
                request.Parameters.Add("street", addressRequest.Street);

            if (!String.IsNullOrEmpty(addressRequest.PostalCode))
                request.Parameters.Add("postalcode", addressRequest.PostalCode);
        }

        /***************************************************/

        private static void SetParameters(ref GetRequest request, NominatimConfig config)
        {
            request.Parameters.Add("format", config.OutputFormat.ToString().ToLower());
            config.OutputDetails.AddToParameters(ref request);

            if (config.PolygonOutput != PolygonOutput.None)
                request.Parameters.Add($"polygon_{config.PolygonOutput.ToString().ToLower()}", 1);

            config.ResultLimitation.AddToParameters(ref request);
        }

        /***************************************************/
    }
}
