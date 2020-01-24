using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;


namespace YDMC.Integration
{
    public class MsTeams
    {
        public class Payload
        {
            public string Title;
            public string Text;
        };

        public void PostMessage(string uri, string title, string text)
        {
            try
            {
                Payload payload = new Payload()
                {
                    Title = title,
                    Text = text
                };

                PostMessage(uri, payload);
            }
            catch 
            {
            }
        }

        //Post a message using a Payload object
        public void PostMessage(string strUri, Payload payload)
        {
            try
            {
        return;
                //string payloadJson = JsonConvert.SerializeObject(payload);
                //var content = new StringContent(payloadJson);
                //content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                //var client = new HttpClient();
                //var uri = new Uri(strUri);
                //await client.PostAsync(uri, content);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }
        
    }
}
