using System.Collections.Generic;

namespace ServiceBusSenderApi.Model
{
    public class SuraInsurance
    {
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        public class Beneficiary
        {
            public Beneficiary beneficiary { get; set; }
            public string relationship { get; set; }
            public int percentageBenefit { get; set; }
            public Person person { get; set; }
            public Identification identification { get; set; }
        }

        public class DetailPEP
        {
            public PEP PEP { get; set; }
        }

        public class ElectronicAddress
        {
            public string emailAddress { get; set; }
            public string webAddress { get; set; }
        }

        public class Identification
        {
            public string personIdentificationNumber { get; set; }
            public string typeOfIdentification { get; set; }
            public string expeditionDate { get; set; }
        }

        public class Insured
        {
            public Person person { get; set; }
            public bool isPEP { get; set; }
            public string typeRelationPEP { get; set; }
            public DetailPEP detailPEP { get; set; }
        }

        public class Nationality
        {
            public string code { get; set; }
            public string name { get; set; }
        }

        public class PEP
        {
            public Person person { get; set; }
            public string relationship { get; set; }
            public string partnership { get; set; }
            public string participationPercentage { get; set; }
        }

        public class Person
        {
            public string givenName { get; set; }
            public string middleName { get; set; }
            public string lastName { get; set; }
            public string surname { get; set; }
            public string gender { get; set; }
            public string birthDate { get; set; }
            public Identification identification { get; set; }
            public Nationality nationality { get; set; }
            public PostalAddress postalAddress { get; set; }
            public ElectronicAddress electronicAddress { get; set; }
            public PhoneAddress phoneAddress { get; set; }
        }

        public class PhoneAddress
        {
            public string phoneNumber { get; set; }
            public string mobileNumber { get; set; }
        }

        public class PostalAddress
        {
            public string city { get; set; }
            public string streetName { get; set; }
            public string streetNumber { get; set; }
            public string streetBuildingIdentification { get; set; }
            public string department { get; set; }
        }

        public class FolioInsurance
        {
            public Insured insured { get; set; }
            public List<Beneficiary> beneficiary { get; set; }
        }
    }
}
