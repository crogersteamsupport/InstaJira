using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TeamSupport.ServiceLibrary
{
  public class ContactObjects
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

    public class Photo
    {
      public string type { get; set; }
      public string typeId { get; set; }
      public string typeName { get; set; }
      public string url { get; set; }
      public bool isPrimary { get; set; }
      public string photoBytesMD5 { get; set; }
    }

    public class Website
    {
      public string url { get; set; }
    }

    public class ContactInfo
    {
      public List<Website> websites { get; set; }
      public string familyName { get; set; }
      public string fullName { get; set; }
      public string givenName { get; set; }
    }

    public class Organization
    {
      public bool isPrimary { get; set; }
      public string name { get; set; }
      public string startDate { get; set; }
      public string title { get; set; }
      public bool current { get; set; }
    }

    public class City
    {
      public bool deduced { get; set; }
      public string name { get; set; }
    }

    public class State
    {
      public bool deduced { get; set; }
      public string name { get; set; }
      public string code { get; set; }
    }

    public class Country
    {
      public bool deduced { get; set; }
      public string name { get; set; }
      public string code { get; set; }
    }

    public class Continent
    {
      public bool deduced { get; set; }
      public string name { get; set; }
    }

    public class County
    {
      public bool deduced { get; set; }
      public string name { get; set; }
      public string code { get; set; }
    }

    public class LocationDeduced
    {
      public string normalizedLocation { get; set; }
      public string deducedLocation { get; set; }
      public City city { get; set; }
      public State state { get; set; }
      public Country country { get; set; }
      public Continent continent { get; set; }
      public County county { get; set; }
      public double likelihood { get; set; }
    }

    public class Demographics
    {
      public LocationDeduced locationDeduced { get; set; }
      public string locationGeneral { get; set; }
    }

    public class SocialProfile
    {
      public string bio { get; set; }
      public int followers { get; set; }
      public int following { get; set; }
      public string type { get; set; }
      public string typeId { get; set; }
      public string typeName { get; set; }
      public string url { get; set; }
      public string username { get; set; }
      public string id { get; set; }
    }

    public class Score
    {
      public string provider { get; set; }
      public string type { get; set; }
      public int value { get; set; }
    }

    public class Topic
    {
      public string provider { get; set; }
      public string value { get; set; }
    }

    public class DigitalFootprint
    {
      public List<Score> scores { get; set; }
      public List<Topic> topics { get; set; }
    }

    public class RootObject
    {
      public int status { get; set; }
      public string requestId { get; set; }
      public double likelihood { get; set; }
      public List<Photo> photos { get; set; }
      public ContactInfo contactInfo { get; set; }
      public List<Organization> organizations { get; set; }
      public Demographics demographics { get; set; }
      public List<SocialProfile> socialProfiles { get; set; }
      public DigitalFootprint digitalFootprint { get; set; }
    }
  }
}
