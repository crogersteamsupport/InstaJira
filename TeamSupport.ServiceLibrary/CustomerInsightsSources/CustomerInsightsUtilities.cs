using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TeamSupport.Data;

namespace TeamSupport.ServiceLibrary
{
  public class CustomerInsightsUtilities
  {
    public static bool DownloadImage(string imageUrl, string savePath, int organizationId, AttachmentProxy.References refType, TeamSupport.Data.LoginUser user, out string resultMessage)
    {
      resultMessage = string.Empty;
      Image imageDownloaded = null;

      try
      {
        HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(imageUrl);

        using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
        {
          using (Stream stream = httpWebResponse.GetResponseStream())
          {
            if (httpWebResponse.StatusCode == HttpStatusCode.OK && httpWebResponse.ContentType.Contains("image"))
            {
              imageDownloaded = Image.FromStream(stream);

              if (imageDownloaded != null)
              {
                if (!DownloadCheckingIfDifferent(imageDownloaded, savePath, ImageFormat.Png))
                {
                  resultMessage = "The image is the same as the current one. It was not downloaded.";
                  imageDownloaded = null;
                }
                else
                {
                    resultMessage = string.Format("Image saved: {0}", savePath);

                    //ModelAPI.AttachmentAPI.CreateAttachment(savePath, organizationId, refType, user.UserID, httpWebResponse);
                    TeamSupport.Data.Quarantine.ServiceQ.CreateAttachment(savePath, organizationId, refType, user, httpWebResponse);
                }
            }
              else
              {
                resultMessage = "The image was not downloaded, but no error was thrown.";
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        resultMessage = string.Format("{0}\n{1}", ex.Message, ex.StackTrace);
      }

      return imageDownloaded != null;
    }


    public static string MakeHttpWebRequest(string requestUrl, string apiKey, TeamSupport.Data.Logs log, Settings settings, string apiCallCountKey, ref int currentApiCallCount)
    {
      string responseText = string.Empty;
      HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;
      request.ContentType = "application/json";
      request.Headers.Add(String.Format("X-FullContact-APIKey:{0}", apiKey));

      try
      {
        using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
        {
          if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Accepted)
          {
            throw new Exception(String.Format(
              "Error when connecting to FullContact: Server error (HTTP {0}: {1}).",
              response.StatusCode,
              response.StatusDescription));
          }

          currentApiCallCount++;
          settings.WriteInt(apiCallCountKey, currentApiCallCount);
          
          using (var reader = new StreamReader(response.GetResponseStream(), ASCIIEncoding.UTF8))
          {
            responseText = reader.ReadToEnd();
          }

          if (response.StatusCode == HttpStatusCode.Accepted || (responseText.Contains("status") && responseText.Contains("202") && responseText.Contains("Queued") && responseText.Contains("retry")))
          {
            log.WriteEvent(responseText);
            responseText = null;
          }

          // Get the headers associated with the response.
          WebHeaderCollection headerCollection = response.Headers;
          String[] remaining = headerCollection.GetValues(RateLimitHeaders.Remaining.GetDescription());
          String[] reset = headerCollection.GetValues(RateLimitHeaders.Reset.GetDescription());
          String[] limit = headerCollection.GetValues(RateLimitHeaders.Limit.GetDescription());
		  string rateLimitHeadersString = string.Empty;

		  if (limit.Any() && limit.Length > 0)
		  {
			rateLimitHeadersString = string.Format("{0}: {1}", RateLimitHeaders.Limit.GetDescription(), limit[0]);
		  }

		  if (reset.Any() && reset.Length > 0)
		  {
			rateLimitHeadersString += string.Format("\t{0}: {1}", RateLimitHeaders.Reset.GetDescription(), reset[0]);
		  }

          if (remaining.Any() && remaining.Length > 0)
          {
			rateLimitHeadersString += string.Format("\t{0}: {1}", RateLimitHeaders.Remaining.GetDescription(), remaining[0]);
			log.WriteEvent(rateLimitHeadersString);
            int remainingCount = int.Parse(remaining[0]);

            if (remainingCount == 1)
            {
              //wait
              if (reset.Any() && reset.Length > 0)
              {
                int resetSeconds = int.Parse(reset[0]);
                log.WriteEvent(string.Format("Rate Limit reached. Sleeping {0} seconds until it resets.", resetSeconds));
                System.Threading.Thread.Sleep(resetSeconds * 1000);
                log.WriteEvent("After sleep. Continuing...");
              }
            }
          }
		  else
		  {
			log.WriteEvent(rateLimitHeadersString);
		  }
        }
      }
      catch (WebException webEx)
      {
        log.WriteEvent(requestUrl);
        log.WriteException(webEx);
      }
      catch (Exception ex)
      {
        log.WriteException(ex);
      }

      return responseText;
    }

    public static bool DownloadCheckingIfDifferent(Image dowloadingImage, string currentImagePath, ImageFormat imageFormat)
    {
      bool isDownloaded = false;

      if (File.Exists(currentImagePath))
      {
        string tempFileName = currentImagePath.Replace(".", "_temp.");
        dowloadingImage.Save(tempFileName);

        MemoryStream ms = new MemoryStream();
        Bitmap downloadedImage = new Bitmap(tempFileName);
        downloadedImage.Save(ms, imageFormat);
        String firstBitmap = Convert.ToBase64String(ms.ToArray());
        ms.Position = 0;

        Bitmap currentImage = new Bitmap(currentImagePath);
        currentImage.Save(ms, imageFormat);
        String secondBitmap = Convert.ToBase64String(ms.ToArray());

        downloadedImage.Dispose();
        currentImage.Dispose();

        if (firstBitmap.Equals(secondBitmap))
        {
          isDownloaded = false;
        }
        else
        {
          File.Delete(currentImagePath);
          File.Move(tempFileName, currentImagePath);
          isDownloaded = true;
        }

        if (File.Exists(tempFileName))
        {
          File.Delete(tempFileName);
        }
      }
      else
      {
        dowloadingImage.Save(currentImagePath);
        isDownloaded = true;
      }

      return isDownloaded;
    }

    public enum SocialProfiles : byte
    {
      unknown = 0,
      LinkedIn = 1,
      Twitter = 2,
      Facebook = 3,
      GooglePlus = 4,
      CrunchBase = 5,
      Klout = 6,
      Gravatar = 7,
      AngelList = 8,
      YouTube = 9
    }

    private enum RateLimitHeaders : byte
    {
      unknown = 0,
      [Description("X-Rate-Limit-Limit")]
      Limit = 1,
      [Description("X-Rate-Limit-Remaining")]
      Remaining = 2,
      [Description("X-Rate-Limit-Reset")]
      Reset = 3
    }
  }

  public static class CustomerInsightsExtensions
  {
    public static string GetDescription(this Enum value)
    {
      Type type = value.GetType();
      string name = Enum.GetName(type, value);
      if (name != null)
      {
        FieldInfo field = type.GetField(name);
        if (field != null)
        {
          DescriptionAttribute attr = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
          if (attr != null)
          {
            return attr.Description;
          }
        }
      }
      return null;
    }
  }
}
