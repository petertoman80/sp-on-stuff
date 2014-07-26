using ListFromExcelAppWeb.Helpers;
using Microsoft.SharePoint.Client;
using Smartpoint.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Web;
using System.Web.Mvc;

namespace ListFromExcelAppWeb.Controllers
{
    public class HomeController : Controller
    {
        SharePointContext spContext;
        

        [SharePointContextFilter]
        public ActionResult Index()
        {
            var appWeb = HttpContext.Request["SPAppWebUrl"];
            if (!string.IsNullOrWhiteSpace( appWeb))
            {
                HttpContext.Session["SPAppWebUrl"] = appWeb;
            }

            var hostWeb = HttpContext.Request["SPHostUrl"];
            if (!string.IsNullOrWhiteSpace(appWeb))
            {
                HttpContext.Session["SPHostUrl"] = hostWeb;
            }
            
            User spUser = null;

            spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);

            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                if (clientContext != null)
                {
                    spUser = clientContext.Web.CurrentUser;

                    clientContext.Load(spUser, user => user.Title);

                    clientContext.ExecuteQuery();

                    ViewBag.UserName = spUser.Title;
                }
            }

            return View();
        }


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public Stream GetFileFromAppWebLibrary(string fileName, string libraryName)//UploadedFiles
        {
            Stream fileStream = null;

            using (ClientContext clientContext = new ClientContext(HttpContext.Session["SPAppWebUrl"].ToString()))
            {
                SecureString passWord = new SecureString();

                foreach (char c in "Prasnica1234".ToCharArray()) passWord.AppendChar(c);

                if (clientContext != null)
                {
                    clientContext.Credentials = new SharePointOnlineCredentials("peter_srank@optumUk.onmicrosoft.com", passWord);

                    Microsoft.SharePoint.Client.File file = clientContext.Web.GetFileByServerRelativeUrl(@"/" + libraryName + "/" + fileName);

                    clientContext.Load(file);
                    clientContext.ExecuteQuery();

                    fileStream = file.OpenBinaryStream().Value;
                }
            }

            return fileStream;
        }

        public string CreateList(string fileName)
        {

            
            //var spContext = SharePointContextProvider.Current.CreateSharePointContext()
            //    //.GetSharePointContext(HttpContext);

            //using (var clientContext = spContext.CreateUserClientContextForSPHost())
            //{

            //var hostWeb = new Uri("http://optumuk.sharepoint.com");
            //string appOnlyAccessToken = TokenHelper.GetClientContextWithAuthorizationCode(.GetS2SAccessTokenWithWindowsIdentity(hostWeb, null);

            //    using (ClientContext clientContext =
            //        TokenHelper.GetClientContextWithAccessToken(hostWeb.ToString(), appOnlyAccessToken))
            //   {

           
                using (ClientContext clientContext = new ClientContext("https://optumuk.sharepoint.com"))
                {
                    SecureString passWord = new SecureString();

                    foreach (char c in "Prasnica1234".ToCharArray()) passWord.AppendChar(c);

                    if (clientContext != null)
                    {
                        clientContext.Credentials = new SharePointOnlineCredentials("peter_srank@optumUk.onmicrosoft.com", passWord);

                        Stream fileStream = GetFileFromAppWebLibrary(fileName, "UploadedFiles");

                        if (fileStream != null)
                        {
                            ExcelReader reader = new ExcelReader();
                            List<List<string>> rowList = reader.RetrieveRowsCollection(fileStream);

                            string listName = fileName.Remove(fileName.LastIndexOf("."));

                            Web oweb = clientContext.Web;
                            ListCreationInformation listCreationInfo = new ListCreationInformation();
                            listCreationInfo.Title = listName;
                            listCreationInfo.TemplateType = (int)ListTemplateType.GenericList;
                            List olist = oweb.Lists.Add(listCreationInfo);
                            clientContext.Load(olist);

                            var columns = rowList.Take(1).FirstOrDefault().ToArray();

                            foreach (string cell in columns)
                            {
                                olist.Fields.AddFieldAsXml(@"<Field Type='Text' DisplayName='" + cell + "'/>", true, AddFieldOptions.DefaultValue);
                            }

                            //Field field1 = olist.Fields.AddFieldAsXml(@"<Field Type='Text' DisplayName='FirstName'/>", true, AddFieldOptions.DefaultValue);
                            //Field field2 = olist.Fields.AddFieldAsXml(@"<Field Type='Text' DisplayName='LastName'/>", true, AddFieldOptions.DefaultValue);

                            // Add some data.
                            ListItemCreationInformation itemCreateInformation = new ListItemCreationInformation();

                            int counter = 0;
                            ListItem listItem = null;
                            foreach (List<string> row in rowList.Skip(1))
                            {
                                counter = 0;
                                listItem = olist.AddItem(itemCreateInformation);
                                foreach (string cell in row)
                                {
                                    listItem[columns[counter].ToString()] = cell;
                                    counter++;
                                }
                                listItem.Update();
                            }

                            clientContext.ExecuteQuery();
                        }
                    }
                }
            

            return "1";
        }


        

        //https://optumuk-3d3d2536dacb65.sharepoint.com/ListFromExcelApp/
        [HttpPost]
        public ContentResult UploadFiles()
        {
            //if(Request.Files.Cast<string>().Any(f => !f.Contains(".xlsx")))
            //{
            //    return Content("{\"name\":\"Only Excel 2007 file format is allowed.\",\"type\":\"\",\"size\":\"\"}", "application/json");
            //}


            var appWeb = HttpContext.Session["SPAppWebUrl"].ToString(); // HttpContext.Request["SPAppWebUrl"];
            Uri appUri = new Uri(appWeb);

            var r = new List<UploadFilesResult>();

            using (var clientContext = new ClientContext(appUri))
            {
                SecureString passWord = new SecureString();

                foreach (char c in "Prasnica1234".ToCharArray()) passWord.AppendChar(c);

                clientContext.Credentials = new SharePointOnlineCredentials("peter_srank@optumUk.onmicrosoft.com", passWord);

                foreach (string file in Request.Files)
                {
                    HttpPostedFileBase hpf = Request.Files[file] as HttpPostedFileBase;
                    if (hpf.ContentLength == 0) continue;
                    using (var fs = hpf.InputStream)
                    {
                        var fi = new FileInfo(hpf.FileName);
                        //change fi.name so that it is unique
                        var list = clientContext.Web.Lists.GetByTitle("UploadedFiles");
                        clientContext.Load(list.RootFolder);
                        clientContext.ExecuteQuery();
                        var fileUrl = String.Format("{0}/{1}", list.RootFolder.ServerRelativeUrl, fi.Name);

                        Microsoft.SharePoint.Client.File.SaveBinaryDirect(clientContext, fileUrl, fs, true);
                    }
                    //string savedFileName = Path.Combine(Server.MapPath("~/App_Data"), Path.GetFileName(hpf.FileName)); 
                    //hpf.SaveAs(savedFileName); // Save the file 		
                    r.Add(new UploadFilesResult() { Name = hpf.FileName, Length = hpf.ContentLength, Type = hpf.ContentType });
                }	// Returns json	
            }
            return Content("{\"name\":\"" + r[0].Name + "\",\"type\":\"" + r[0].Type + "\",\"size\":\"" + string.Format("{0} bytes", r[0].Length) + "\"}", "application/json");
        }



        public string GetLists()
        {
            var appWeb = HttpContext.Request["SPAppWebUrl"];
            Uri appUri = new Uri("https://optumuk-3d3d2536dacb65.sharepoint.com/");

            var r = new List<UploadFilesResult>();



            using (var clientContext = new ClientContext(appUri))
            {
                        var list = clientContext.Web.Lists;//.GetByTitle("UploadedFiles");
                        clientContext.Load(list);//clientContext.Load(list.RootFolder);
                        clientContext.ExecuteQuery();
                        //var fileUrl = String.Format("{0}/{1}", list.RootFolder.ServerRelativeUrl, fi.Name);
            }
            return "1";
        }
    }
}
