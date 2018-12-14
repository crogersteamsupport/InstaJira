using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace TeamSupport.Api
{
  public class RestXmlWriter
  {
    private XmlTextWriter _xmlWriter;
    public XmlTextWriter XmlWriter
    {
      get { return _xmlWriter; }
    }

    private MemoryStream _stream;

    public RestXmlWriter(string listName)
    {
      _stream = new MemoryStream();
      _xmlWriter = new XmlTextWriter(_stream, new UTF8Encoding(false));
      _xmlWriter.Formatting = Formatting.Indented;
      _xmlWriter.WriteStartDocument();
      _xmlWriter.WriteStartElement(listName);
    }

    public string GetXml()
    {
      _xmlWriter.WriteFullEndElement();
      _xmlWriter.WriteEndDocument();
      _xmlWriter.Flush();
      _stream.Position = 0;
      StreamReader reader = new StreamReader(_stream);
      return reader.ReadToEnd();
    }


  }
}
