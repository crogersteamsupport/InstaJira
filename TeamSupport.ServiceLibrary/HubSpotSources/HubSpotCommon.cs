using System;
using Newtonsoft.Json;

namespace TeamSupport.ServiceLibrary
{
  public class HubSpotCommon
  {
    public class MyDateTimeConverter : Newtonsoft.Json.JsonConverter
    {
      public override bool CanConvert(Type objectType)
      {
        return objectType == typeof(object) || objectType == typeof(long);
      }

      public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
      {
        if (reader.Path.Contains("timestamp") && reader.Value != null)
        {
          var t = long.Parse(reader.Value.ToString());
          return new DateTime(1970, 1, 1).AddMilliseconds(t);
        }
        else
        {
          return reader.Value;
        }
      }

      public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
      {
        throw new NotImplementedException();
      }
    }
  }
}
