using Entities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;


namespace ConsoleApp
{
    internal class GenerateRent:IGenerateService
    {
        public void Generate(int number, CrmServiceClient service)
        {
            DateTime startDate = new DateTime(2019, 1, 1);
            DateTime endDate = new DateTime(2020, 12, 31);
            Random random = new Random();
            DateTime reservedPickupDateTime;
            DateTime reservedHandoverDateTime;
            ExecuteMultipleRequest multipleRequest = new ExecuteMultipleRequest();
            multipleRequest.Requests = new OrganizationRequestCollection();
            multipleRequest.Settings = new ExecuteMultipleSettings
            {
                ContinueOnError = true,
                ReturnResponses = true
            };

            uds_rent rent;
            int countOfRequests = 0;
            for (int i=56980; i < number; i++, countOfRequests++)
            {

                reservedPickupDateTime = RandomDateTime(startDate, endDate, random);
                reservedHandoverDateTime = reservedPickupDateTime.AddDays(random.Next(1, 31));
                string query = GenerateRent.ReadXml("C:\\Users\\chumi\\source\\repos\\ConsoleApp\\ConsoleApp\\Xml\\Car.xml");
                
                rent = new uds_rent();
                rent.uds_reserved_pickup = reservedPickupDateTime;
                rent.uds_reserved_handover = reservedHandoverDateTime;
                Entity car = RandomEntity(query, service, random);
                rent.uds_name = (i + 1).ToString();
                rent.uds_car = new EntityReference("uds_car", car.Id);

                rent.uds_carclass = new EntityReference("uds_car_class", ((EntityReference)car.Attributes["uds_car"]).Id);
                query = GenerateRent.ReadXml("C:\\Users\\chumi\\source\\repos\\ConsoleApp\\ConsoleApp\\Xml\\Contact.xml");
                Entity contact = RandomEntity(query, service, random);
                rent.uds_customer = new EntityReference("uds_customer", contact.Id);
                rent.uds_paid= random.Next(0, 1)==1;
                rent.uds_pickup_location = (uds_rent_uds_pickup_location)random.Next(0, 2);
                rent.uds_return_location= (uds_rent_uds_return_location)random.Next(0, 2);
                rent.uds_price = new Money(Convert.ToDecimal(random.NextDouble() * (1000 - 1) + 1));

                query = GenerateRent.ReadXml("C:\\Users\\chumi\\source\\repos\\ConsoleApp\\ConsoleApp\\Xml\\State.xml");
                Entity state = RandomForState(query, service, random);
                uds_car_transfer_report report;
                rent.uds_Status = new EntityReference("bookingstatus", state.Id);
                Console.WriteLine(state.Attributes["name"]);
                string strState = state.Attributes["name"].ToString();
                if (strState == "Renting")
                {
                    report = new uds_car_transfer_report();
                    report.uds_car = new EntityReference("uds_car", car.Id);
                    bool damage = SetDamage();
                    report.uds_damages = damage;
                    if (damage)
                    {
                        report.uds_damage_description = "damage";
                    }
                    report.uds_name = i + 1.ToString();
                    report.uds_type = false;
                    report.uds_date = reservedPickupDateTime;
                    Guid id= Guid.NewGuid();
                    report.uds_car_transfer_reportId= id;
                    service.Create(report);
                    rent.uds_Pickupreport = new EntityReference("uds_car_transfer_report", id);
                    Console.WriteLine("_____________");
                }
                else if (strState == "Returned")
                {

                    report = new uds_car_transfer_report();
                    report.uds_car = new EntityReference("uds_car", car.Id);
                    bool damage = SetDamage();
                    report.uds_damages = damage;
                    if (damage)
                    {
                        report.uds_damage_description = "damage";
                    }
                    report.uds_name = i + 1.ToString();
                    report.uds_type = false;
                    report.uds_date = reservedPickupDateTime;
                    Guid id = Guid.NewGuid();
                    report.uds_car_transfer_reportId = id;

                    Console.WriteLine("======================");
                    service.Create(report);
                    rent.uds_Pickupreport = new EntityReference("uds_car_transfer_report", id);

                    report = new uds_car_transfer_report();
                    report.uds_car = new EntityReference("uds_car", car.Id);
                    damage = SetDamage();
                    report.uds_damages = damage;
                    if (damage)
                    {
                        report.uds_damage_description = "damage";
                    }
                    report.uds_name = i + 1.ToString();
                    report.uds_type = false;
                    report.uds_date = reservedHandoverDateTime;
                    id = Guid.NewGuid();
                    report.uds_car_transfer_reportId = id;
                    service.Create(report);
                    rent.uds_Returnreport = new EntityReference("uds_car_transfer_report", id);
                }
                else
                {
                    rent.uds_paid = SetPaid(strState);
                }
                CreateRequest request = new CreateRequest();
                request.Target = rent;
                multipleRequest.Requests.Add(request);
                if (countOfRequests == 999)
                {
                    ExecuteMultipleResponse multipleResponse =
                (ExecuteMultipleResponse)service.Execute(multipleRequest);
                }
                

                Console.WriteLine(i+1);
            }
            
            Console.WriteLine("Finish");
        }
        static bool SetDamage()
        {
            Random random = new Random();
            double probability = random.NextDouble();
            return probability < 0.05;
            
        }

        static bool SetPaid(string status)
        {
            Random random = new Random();
            double probability = random.NextDouble();

            switch (status)
            {
                case "Confirmed":
                    return probability < 0.9;
                case "Renting":
                    return probability < 0.999;
                case "Returned":
                    return probability < 0.9998;
                default:
                    return false;
            }
        }

            static DateTime RandomDateTime(DateTime start, DateTime end, Random random)
        {
            int range = (end - start).Days;
            return start.AddDays(random.Next(range)).AddHours(random.Next(24)).AddMinutes(random.Next(60));
        }

        static Entity RandomEntity(string query, CrmServiceClient service, Random random)
        {
            EntityCollection entities = service.RetrieveMultiple(new FetchExpression(query));
            var colection = entities.Entities[random.Next(0, entities.Entities.Count)];
            return entities.Entities[random.Next(0, entities.Entities.Count)];
        }

        static Entity RandomForState(string query, CrmServiceClient service, Random random)
        {
            EntityCollection entities = service.RetrieveMultiple(new FetchExpression(query));
            List<double> probabilities = new List<double> {0.1, 0.05, 0.05, 0.05, 0.75 };

            double randomValue = random.NextDouble();
            double cumulativeProbability = 0;
            for (int i = 0; i < 5; i++)
            {
                cumulativeProbability += probabilities[i];
                if (randomValue < cumulativeProbability)
                {
                    return entities.Entities[i];
                }
            }
            return null;
        }

        public static string ReadXml(string filePath)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filePath);
            XmlNode root = xmlDoc.DocumentElement; 
            return root.OuterXml;
        }

    }
}
