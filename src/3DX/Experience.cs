using DataLayer.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using SimulationsLib;
using Reco3Common;

namespace _3DX
{
    public class ThreeDExperience
    {
	    private OauthHelper _oauthClient = null;
		protected ComponentResponse _ComponentResponse = new ComponentResponse();
        protected List<string> _PDIdentifiers = new List<string>();
        protected string _strBaseUrl;
        protected string _strClientId;
        protected string _strClientSecret;
        protected string _strScope;
        protected string _strUrl;

        public bool Initialize(string strBaseUrl,
            string strClientId,
            string strClientSecret,
            string strScope,
            string strUrl)
        {
            try
            {
                _strBaseUrl = strBaseUrl;
                _strClientId = strClientId;
                _strClientSecret = strClientSecret;
                _strScope = strScope;
                _strUrl = strUrl;
	            _oauthClient = null;
				return true;
            }
            catch (Exception ex)
            {
            }

            return false;
        }

        public int PDCount
        {
            get { return _PDIdentifiers.Count; }
        }

        public void Reset()
        {
            try
            {
                _strBaseUrl = "https://api.integration.prod.aws.scania.com/";
                _strClientId = "Iy0BxcywUXU1GdcLHfVnj8e0vvca";
                _strClientSecret = "GhlHbwLkFJrvtf5w64kk0m5syNka";
                _strScope = "PDComponentInfo_Prod_Full";
                _strUrl = "Reco2ComponentInfo/v1/ComponentInfo";
                _PDIdentifiers.Clear();
            }
            catch (Exception ex)
            {
            }
        }

        public bool AddPDNum(string strPDNumber)
        {
            try
            {
                _PDIdentifiers.Add(strPDNumber);
                return true;
            }
            catch (Exception ex)
            {
            }

            return false;
        }

        protected string Base64ToString(string strRaw)
        {
            try
            {
                byte[] data = Convert.FromBase64String(strRaw);
                return Encoding.UTF8.GetString(data);
            }
            catch (Exception ex)
            {
            }

            return "";
        }

	    public List<Reco3Component> Query(int nRetries, int nTimeout, ref int nAttempts)
	    {
			try
			{
				nAttempts++;
				return Query();
			}
			catch (NetworkInformationException e)
			{
				if (e.ErrorCode == Convert.ToInt32(System.Net.HttpStatusCode.GatewayTimeout))
				{
					// If this happens, sleep for 5 seconds and then try again,....
					Thread.Sleep(nTimeout);
					return Query(nRetries - 1, nTimeout, ref nAttempts);
				}
				else
				{
					Helper.ToConsole(string.Format("!! Query: {0}:{1}", e.ErrorCode, e.Message));
				}
			}

		    return null;
	    }
		public List<Reco3Component> Query()
        {
            ComponentResponse response = new ComponentResponse();
	        if (_oauthClient == null)
	        {
		        _oauthClient = new OauthHelper()
		        {
			        BaseUrl = _strBaseUrl, // "https://api.integration.prod.aws.scania.com/",
			        ClientId = _strClientId, // "Iy0BxcywUXU1GdcLHfVnj8e0vvca",
			        ClientSecret = _strClientSecret, // "GhlHbwLkFJrvtf5w64kk0m5syNka",
			        Scope = _strScope // "PDComponentInfo_Prod_Full"
				};
	        }
			response = CallApiAsync(_strBaseUrl,
                                    _strClientId,
                                    _strClientSecret,
                                    _strScope,
                                    _strUrl,
									_oauthClient,
									_PDIdentifiers).GetAwaiter().GetResult();
            if (response != null)
            {
                List<Reco3Component> Reco3Components = new List<Reco3Component>();
                foreach (ComponentData ThreeDcomponent in response.ComponentData)
                {
                    Reco3Component reco3Component = new Reco3Component();
                    reco3Component.PDNumber = ThreeDcomponent.PDId.ToString();
                    reco3Component.XML = Base64ToString(ThreeDcomponent.ComponentInfo);
                    reco3Component.SetComponentTypeFromXml();
                    reco3Component.SetComponentDescriptionFromXml();
	                reco3Component.PD_Status = Reco3_Enums.PDStatus.ctReleased;

					Reco3Components.Add(reco3Component);
                }

                return Reco3Components;
            }
            else
            {
                int n = 0;
            }

            return null;
        }

        static private async Task<ComponentResponse> CallApiAsync(string strBaseUrl,
            string strClientId,
            string strClientSecret,
            string strScope,
            string strUrl,
	        OauthHelper oauth,
			List<string> PDIdentifiers)
        {
            ComponentRequest request = new ComponentRequest();
            request.PDIdentifiers = PDIdentifiers;
	        return await oauth.SendHttpPostAsync<ComponentResponse>(strUrl, request);
        }
    }
}
