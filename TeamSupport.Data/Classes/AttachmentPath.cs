using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TeamSupport.Data
{
  internal class AttachmentPath
  {
    public enum Folder
    {
        None,
        Images,
        Styles,
        ChatImages,
        ChatStyles,
        TicketTypeImages,
        Products,
        Actions,
        Organizations,
        ProfileImages,
        WaterCooler,
        OrganizationAttachments,
        UserAttachments,
        AgentRating,
        AssetAttachments,
        Imports,
        OrganizationsLogo,
        ContactImages,
        ImportLogs,
        TempImages,
        CustomerHubLogo,
        ScheduledReports,
        ScheduledReportsLogs,
        ChatUploads,
        Tasks,
        CompanyActivityAttachments,
        ContactActivityAttachments
        };

    /// <summary>
    /// Gets the root path for attachments as specified in the SystemSettings table
    /// </summary>
    /// <param name="loginUser"></param>
    /// <param name="organizationID"></param>
    /// <returns></returns>
    public static string GetRoot(LoginUser loginUser, int organizationID)
    {
      //string root = SystemSettings.ReadString(loginUser, "FilePath", "C:\\TSData");
      FilePaths filePath = new Data.FilePaths(loginUser);
      filePath.LoadByID(1);
      string root = filePath[0].Value;
      string path = Path.Combine(Path.Combine(root, "Organizations"), organizationID.ToString());
      Directory.CreateDirectory(path);
      return path;
    }

        public static string GetRoot(LoginUser loginUser, int organizationID, int filePathID)
        {
            FilePaths filePath = new FilePaths(loginUser);
            filePath.LoadByID(filePathID);
            string root = filePath[0].Value;
            string path = Path.Combine(Path.Combine(root, "Organizations"), organizationID.ToString());
            Directory.CreateDirectory(path);
            return path;
        }

        public static string GetImageCachePath(LoginUser loginUser)
    {
      //string path = SystemSettings.ReadString(loginUser, "ImageCachePath", "C:\\TSData\\ImageCache");
      FilePaths filePaths = new Data.FilePaths(loginUser);
      filePaths.LoadByID(2);
      string path = filePaths[0].Value;
      Directory.CreateDirectory(path);
      return path;
    }

    /// <summary>
    /// Gets the default path for attachments.
    /// </summary>
    /// <param name="loginUser"></param>
    /// <returns></returns>
    public static string GetDefaultRoot(LoginUser loginUser)
    {
      //string root = SystemSettings.ReadString(loginUser, "FilePath", "C:\\TSData");
        FilePaths filePaths = new FilePaths(loginUser);
        filePaths.LoadByID(1);
        string root = filePaths[0].Value;
      return Path.Combine(root, "Default\\");
    }

    public static string GetDefaultRoot(LoginUser loginUser, int filePathID)
    {
        FilePaths filePaths = new FilePaths(loginUser);
        filePaths.LoadByID(filePathID);
        string root = filePaths[0].Value;
      return Path.Combine(root, "Default\\");
    }

    /// <summary>
    /// Gets path for organization.
    /// </summary>
    /// <param name="loginUser"></param>
    /// <param name="organizationID"></param>
    /// <param name="folder"></param>
    /// <returns></returns>
    public static string GetPath(LoginUser loginUser, int organizationID, Folder folder)
    {
      string root = GetRoot(loginUser, organizationID);
      string path = Path.Combine(root, GetFolderName(folder));
      Directory.CreateDirectory(path);
      return path;
    }

    public static string GetPath(LoginUser loginUser, int organizationID, Folder folder, int filePathID)
    {
      string root = GetRoot(loginUser, organizationID, filePathID);
      string path = Path.Combine(root, GetFolderName(folder));
      Directory.CreateDirectory(path);
      return path;
    }

    /// <summary>
    /// Gets the default path.
    /// </summary>
    /// <param name="loginUser"></param>
    /// <param name="folder"></param>
    /// <returns></returns>
    public static string GetDefaultPath(LoginUser loginUser, Folder folder)
    {
      string root = GetDefaultRoot(loginUser);
      return Path.Combine(root, GetFolderName(folder));
    }

    public static string GetDefaultPath(LoginUser loginUser, Folder folder, int filePathID)
    {
      string root = GetDefaultRoot(loginUser, filePathID);
      return Path.Combine(root, GetFolderName(folder));
    }

    /// <summary>
    /// Converts AttachmentPath.Folder ENUM to a STRING
    /// </summary>
    /// <param name="folder"></param>
    /// <returns></returns>
    public static string GetFolderName(Folder folder)
    {
      string result;
      switch (folder)
      {
        case Folder.Images: result = "Images"; break;
        case Folder.Styles: result = "Styles"; break;
        case Folder.ChatImages: result = "Images\\Chat"; break;
        case Folder.ChatStyles: result = "Styles\\Chat"; break;
        case Folder.TicketTypeImages: result = "Images\\TicketTypes"; break;
        case Folder.Organizations: result = "Organizations"; break;
        case Folder.Products: result = "Products"; break;
        case Folder.Actions: result = "Actions"; break;
        case Folder.ProfileImages: result = "Images\\Avatars"; break;
        case Folder.WaterCooler: result = "WaterCooler"; break;
        case Folder.OrganizationAttachments: result = "OrganizationAttachments"; break;
        case Folder.UserAttachments: result = "UserAttachments"; break;
        case Folder.AgentRating: result = "AgentRating"; break;
        case Folder.AssetAttachments: result = "AssetAttachments"; break;
        case Folder.Imports: result = "Imports"; break;
        case Folder.ImportLogs: result = "Imports\\Logs"; break;
        case Folder.OrganizationsLogo: result = "Images\\CompanyLogo"; break;
        case Folder.ContactImages: result = "Images\\Avatars\\Contacts"; break;
        case Folder.TempImages: result = "Images\\Temp"; break;
		case Folder.CustomerHubLogo: result = "Images\\HubLogo"; break;
        case Folder.ScheduledReports: result = "ScheduledReports"; break;
        case Folder.ScheduledReportsLogs: result = "ScheduledReports\\Logs"; break;
        case Folder.ChatUploads: result = "ChatAttachments"; break;
        case Folder.Tasks: result = "Tasks"; break;
        case Folder.CompanyActivityAttachments: result = "CustomerActivityAttachments"; break;
        case Folder.ContactActivityAttachments: result = "ContactActivityAttachments"; break;
        default: result = ""; break;
      }
      return result;
    }

    /// <summary>
    /// Gets the associated reference type of an associated folder. (None if there isn't one)
    /// </summary>
    /// <param name="folder"></param>
    /// <returns></returns>
    public static AttachmentProxy.References GetFolderReferenceType(Folder folder)
    {
      AttachmentProxy.References result;
      switch (folder)
      {
        case Folder.Organizations: result = AttachmentProxy.References.Organizations; break;
        case Folder.Products: result = AttachmentProxy.References.ProductVersions; break;
        case Folder.Actions: result = AttachmentProxy.References.Actions; break;
        case Folder.WaterCooler: result = AttachmentProxy.References.WaterCooler; break;
        case Folder.OrganizationAttachments: result = AttachmentProxy.References.Organizations; break;
        case Folder.UserAttachments: result = AttachmentProxy.References.Users; break;
        case Folder.AssetAttachments: result = AttachmentProxy.References.Assets; break;
		case Folder.CustomerHubLogo: result = AttachmentProxy.References.CustomerHubLogo; break;
        case Folder.ChatUploads: result = AttachmentProxy.References.ChatAttachments; break;
        case Folder.Tasks: result = AttachmentProxy.References.Tasks; break;
        case Folder.ContactActivityAttachments: result = AttachmentProxy.References.ContactActivity; break;
        case Folder.CompanyActivityAttachments: result = AttachmentProxy.References.CompanyActivity; break;
        default: result = AttachmentProxy.References.None; break;
      }
      return result;
    }

    public static Folder GetFolderByName(string folderName)
    {
      folderName = folderName.ToLower().Trim();
      if (folderName.Length < 1) return Folder.None;
      if (folderName[0] == '\\') folderName = folderName.Substring(1, folderName.Length - 1);

      foreach (Folder folder in Enum.GetValues(typeof(Folder)))
      {
        if (folderName == GetFolderName(folder).ToLower()) return folder;
      }
      return Folder.None;
    }

    
    /// <summary>
    /// Removes the path and extension from the file name.
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static string RemovePathAndExt(string fileName)
    {
      return Path.GetFileNameWithoutExtension(Path.GetFileName(fileName));
    }


    /// <summary>
    /// Gets the image file name from the specified folder, no matter the extension
    /// </summary>
    /// <param name="path">Directory to look for the image.</param>
    /// <param name="fileName">Image's file name with or without extension.  Ex: 'chat-logo'</param>
    /// <returns></returns>
    public static string GetImageFileName(string path, string fileName)
    {
      string pattern = RemovePathAndExt(fileName) + ".*";
      string[] files = Directory.GetFiles(path, pattern, SearchOption.TopDirectoryOnly);
      if (files.Length > 0) return files[0]; else return null;
    }

    /// <summary>
    /// Finds an image by name, regardless of extension.  If it does not exist per organization, it retrieves it from the default folder.
    /// </summary>
    /// <param name="loginUser"></param>
    /// <param name="organizationID"></param>
    /// <param name="folder"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static string FindImageFileName(LoginUser loginUser, int organizationID, Folder folder, string fileName)
    {
      try
      {
        string path = GetPath(loginUser, organizationID, folder);
        string result = GetImageFileName(path, fileName);
        if (!File.Exists(result))
        {
          path = GetDefaultPath(loginUser, folder);
          result = GetImageFileName(path, fileName);
        }
        return result;
      }
      catch (Exception ex)
      {
        
      }
      return "";
    }

    /// <summary>
    /// Finds a file by name.  If it does not exist per organization, it retrieves it from the default folder.
    /// </summary>
    /// <param name="loginUser"></param>
    /// <param name="organizationID"></param>
    /// <param name="folder"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static string FindFileName(LoginUser loginUser, int organizationID, Folder folder, string fileName)
    {
      string path = Path.Combine(GetPath(loginUser, organizationID, folder), fileName);
      if (!File.Exists(path))
      {
        path = Path.Combine(GetDefaultPath(loginUser, folder), fileName);
      }

      return path;
    }

    
    /// <summary>
    /// Deletes all files with the same name, regardless of extension
    /// </summary>
    /// <param name="path"></param>
    /// <param name="fileName"></param>
    public static void DeleteFile(string path, string fileName)
    {
      string pattern = RemovePathAndExt(fileName) + ".*";
      string[] files = Directory.GetFiles(path, pattern, SearchOption.TopDirectoryOnly);

      foreach (string s in files)
      {
        File.Delete(s);
      }
    }
  }
}
