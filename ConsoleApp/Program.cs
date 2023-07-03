using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
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
    internal class Program
    {
        static void Main(string[] args)
        {

            string connectionString = "AuthType=Office365;Url=https://org810cf953.crm4.dynamics.com;Username=denischuma@udc463.onmicrosoft.com;Password=Etepov01";
            CrmServiceClient service = new CrmServiceClient(connectionString);
            GenerateRent generateRent = new GenerateRent();
            generateRent.Generate(57000, service);

            //MultipleExecute(service);

            Console.ReadLine();
        }


        static void Execute(CrmServiceClient service)
        {
            Entity contact = new Entity("contact");
            contact.Attributes.Add("fullname", "denis chuma");
            CreateRequest request = new CreateRequest();
            request.Target = contact;
            CreateResponse response= (CreateResponse)service.Execute(request);

            int i = 0;
        }

        static void MultipleExecute(CrmServiceClient service)
        {
            Entity contact1 = new Entity("contact");
            contact1.Attributes.Add("fullname", "denis chuma");

            CreateRequest request1 = new CreateRequest();
            request1.Target = contact1;    

            Entity contact2 = new Entity("contact");
            contact2.Attributes.Add("fullname", "denis chuma");
            CreateRequest request2 = new CreateRequest();
            request2.Target = contact2;

            ExecuteMultipleRequest multipleRequest = new ExecuteMultipleRequest();
            multipleRequest.Requests = new OrganizationRequestCollection();
            multipleRequest.Requests.Add(request1);
            multipleRequest.Requests.Add(request2);
            multipleRequest.Settings = new ExecuteMultipleSettings
            {
                ContinueOnError = true,
                ReturnResponses = true
            };

            ExecuteMultipleResponse multipleResponse =
                (ExecuteMultipleResponse)service.Execute(multipleRequest);

        }
    }





}
