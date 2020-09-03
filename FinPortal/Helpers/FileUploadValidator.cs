using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace FinPortal.Helpers
{
    public class FileUploadValidator
    {
        public static bool IsWebFriendlyFile(HttpPostedFileBase file)
        {
            if (file == null)
            {
                return false;
            }
            if (file.ContentLength > 1024 * 1024 * 2 || file.ContentLength < 2048)
            {
                return false;
            }
            try
            {
                var fileExt = Path.GetExtension(file.FileName);
                var allowableExtensions = WebConfigurationManager.AppSettings["AllowableExtensions"].Split(',').ToList();
                return allowableExtensions.Contains(fileExt);
            }
            catch
            {
                return false;
            }
        }
    }
}