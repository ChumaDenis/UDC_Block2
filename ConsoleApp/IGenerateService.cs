using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using Entities;
using Microsoft.Xrm.Sdk.Messages;

namespace ConsoleApp
{
    internal interface IGenerateService
    {
        void Generate(int number, CrmServiceClient service);
    }
}
