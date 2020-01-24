using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Reco3
{
    [Authorize]
    public class SimJobController : ApiController
    {
        // GET api/<controller>   : url to use => api/vs
        public int GetQueueSize()
        {
            try
            {
                return 0;
            }
            catch 
            {
            }
            return -1;
        }

        public IEnumerable<string> GetShare(int MaxShareSize)
        {
            try
            {
            }
            catch
            {

            }
            return null;
        }

        /*
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
        */
    }
}