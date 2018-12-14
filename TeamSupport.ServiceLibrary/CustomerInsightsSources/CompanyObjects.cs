using System.Collections.Generic;

namespace TeamSupport.ServiceLibrary
{
  public class CompanyObjects
  {
    public enum Property : byte
    {
      unknown           = 0,
      status            = 1,
      requestId         = 2,
      likelihood        = 3,
      photos            = 4,
      contactInfo       = 5,
      organizations     = 6,
      demographics      = 7,
      socialProfiles    = 8,
      digitalFootprint  = 9,
      RootObject        = 10
    }

    public enum Lookup : byte
    {
      unknown   = 0,
      email     = 1,
      facebook  = 2,
      twitter   = 3,
      domain    = 4
    }

    public class Category
    {
        public string name { get; set; }
        public string code { get; set; }
    }

    public class PhoneNumber
    {
        public string number { get; set; }
        public string label { get; set; }
    }

    public class Region
    {
        public string name { get; set; }
        public string code { get; set; }
    }

    public class Country
    {
        public string name { get; set; }
        public string code { get; set; }
    }

    public class Address
    {
        public string addressLine1 { get; set; }
        public string locality { get; set; }
        public Region region { get; set; }
        public Country country { get; set; }
        public string postalCode { get; set; }
        public string label { get; set; }
    }

    public class ContactInfo
    {
        public List<PhoneNumber> phoneNumbers { get; set; }
        public List<Address> addresses { get; set; }
    }

    public class Link
    {
        public string url { get; set; }
        public string label { get; set; }
    }

    public class Image
    {
        public string url { get; set; }
        public string label { get; set; }
    }

    public class Organization
    {
        public string name { get; set; }
        public int approxEmployees { get; set; }
        public string founded { get; set; }
        public string overview { get; set; }
        public ContactInfo contactInfo { get; set; }
        public List<Link> links { get; set; }
        public List<Image> images { get; set; }
        public List<string> keywords { get; set; }
    }

    public class SocialProfile
    {
        public string bio { get; set; }
        public string typeId { get; set; }
        public string typeName { get; set; }
        public string url { get; set; }
        public string username { get; set; }
        public string id { get; set; }
        public int? followers { get; set; }
        public int? following { get; set; }
    }

    public class TopCountryRanking
    {
      public int rank { get; set; }
      public string locale { get; set; }
    }

    public class Ranking
    {
      public int rank { get; set; }
      public string locale { get; set; }
    }

    public class Traffic
    {
      public List<TopCountryRanking> topCountryRanking { get; set; }
      public List<Ranking> ranking { get; set; }
    }

    public class RootObject
    {
        public int status { get; set; }
        public string requestId { get; set; }
        public List<Category> category { get; set; }
        public string logo { get; set; }
        public string website { get; set; }
        public string languageLocale { get; set; }
        public string onlineSince { get; set; }
        public Organization organization { get; set; }
        public List<SocialProfile> socialProfiles { get; set; }
        public Traffic traffic { get; set; }
    }
  }
}
