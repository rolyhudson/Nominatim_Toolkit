using BH.oM.Adapter;
using BH.oM.Adapter.Nominatim;
using BH.oM.Adapters.HTTP;
using BH.oM.Data.Requests;
using BH.oM.Geospatial;
using BH.Engine.Adapter.Nominatim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Base;

namespace BH.Adapter.Nominatim
{
    public partial class NominatimAdapter
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public override IEnumerable<object> Pull(IRequest request, PullType pullType = PullType.AdapterDefault, ActionConfig actionConfig = null)
        {
            if (!(actionConfig is NominatimConfig))
                actionConfig = new NominatimConfig();

            NominatimConfig nominatimConfig = actionConfig as NominatimConfig;

            if(nominatimConfig.OutputFormat != OutputFormat.GeoJSON)
            {
                BH.Engine.Reflection.Compute.RecordWarning("Output format not yet supported. Defaulting to GeoJSON.");
                nominatimConfig.OutputFormat = OutputFormat.GeoJSON;
            }

            Response nominationResponse = new Response();
            if(request is INominatimRequest)
            {
                foreach (CustomObject response in Pull(request, nominatimConfig))
                {
                    IGeospatial geospatial = BH.Engine.Adapter.Nominatim.Convert.ToGeospatial(response);
                    if (geospatial is FeatureCollection)
                        nominationResponse.FeatureCollection = geospatial as FeatureCollection;
                    else if (geospatial is Feature)
                        nominationResponse.FeatureCollection.Features.Add(geospatial as Feature);
                    else
                        nominationResponse.FeatureCollection.Features.Add(new Feature() { Geometry = geospatial });
                }
                    
            }
            else
                Engine.Reflection.Compute.RecordError("This type of request is not supported.");
            return new List<object> { nominationResponse }; 
        }

        /***************************************************/

        public IEnumerable<object> Pull(IRequest request, NominatimConfig config)
        {
            GetRequest getRequest = BH.Engine.Adapter.Nominatim.Create.GetRequest(request as dynamic, config);
            List<object> responses = new List<object>();
            responses.Add(m_HTTPAdapter.Pull(getRequest).First());

            return responses;
        }
    }
}
