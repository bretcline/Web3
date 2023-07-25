using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace WebThree.Shared.Blockchain
{
    public abstract class BaseBlockchainAPIData
    {
        public string apiKey;

        public BaseBlockchainAPIData(string apiKey)
        {
            this.apiKey = apiKey;
        }

        public abstract string GetBaseURL();
        public abstract string GetURL();

    }
}
