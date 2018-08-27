using FISE_Browser.Models;
using FISE_Browser.TWebClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using System.Configuration;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
namespace FISE_Browser.Controllers
{
    public class BookController : BaseController
    {
        private TWebClient _webClient;

        public BookController()
        {
            _webClient = new TWebClient();
        }


        public ActionResult BookRead(string id)
        {
            try
            {
                if (Session["UserObject"] == null)
                    CreateSession();
                User userobj = (User)Session["UserObject"];
                if (userobj != null)
                {
                    _webClient.UserId = userobj.UserId;
                    BookRead BookReadobj = _webClient.DownloadData<BookRead>("getbookdetail", new { BookId = id, UserId = userobj.UserId });
                    if (BookReadobj != null)
                    {
                        switch (BookReadobj.Status)
                        {
                            case 1:
                                return View(BookReadobj);
                            case 2:
                                //no book found
                                return RedirectToAction("Home", "Main");
                            default:
                                //error
                                return RedirectToAction("Home", "Main");
                        }
                    }
                    else
                        return RedirectToAction("Home", "Main");
                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Home", "Main");
            }
        }

        public ActionResult BookCompleted(string id)
        {
            try
            {
                if (Session["UserObject"] == null)
                    CreateSession();
                User userobj = (User)Session["UserObject"];
                if (userobj != null)
                {
                    _webClient.UserId = userobj.UserId;
                    int bookid;
                    if (int.TryParse(id, out bookid))
                    {
                        BookRead booksobj = _webClient.DownloadData<BookRead>("bookcompleted", new { UserId = userobj.UserId, BookId = bookid });
                        if (booksobj != null)
                        {
                            switch (booksobj.Status)
                            {
                                case 1:
                                    booksobj.ReviewJson = booksobj.ReviewJson.Replace("!**!slash!**!", "\\\\").Replace("!**!quot!**!", "\\\"");
                                    return View(booksobj);
                                case 2:
                                    // no book found
                                    return RedirectToAction("Home", "Main");
                                case 3:
                                    //book not read
                                    return RedirectToAction("Home", "Main");
                                default:
                                    //error
                                    return RedirectToAction("Home", "Main");
                            }

                        }
                        else
                            //error in api
                            return RedirectToAction("Home", "Main");
                    }
                    else
                        //redirect to not found page
                        return RedirectToAction("Home", "Main");
                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Home", "Main");
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult SetBookReview(int bookid, bool isattempted, string json, string starttime, int rating,
            bool IsautoSync, int UserId, bool IsSessionOut)
        {
            try
            {
                _webClient.UserId = UserId;
                if (IsSessionOut)
                    starttime = DateTime.UtcNow.ToString();

                var CompletionTime = DateTime.UtcNow - Convert.ToDateTime(starttime);

                if (Session["UserObject"] == null && !IsautoSync)
                    CreateSession();
                User userobj = (User)Session["UserObject"];
                if (IsautoSync || userobj != null)
                {
                    int result = _webClient.UploadData<int>("setbookreview", new { BookId = bookid, UserId = UserId, Json = json, IsReviewDone = isattempted, Platform = "Browser", Environment = "Home", CompletedOn = DateTime.UtcNow, CompletionTime = CompletionTime.TotalSeconds / 60, Rating = rating });
                    if (Session["starttime"] != null)
                    {
                        (new Helper.Helper()).UpdateBrowsing("timeout", userobj.SessionId, Convert.ToDateTime(Session["starttime"]), userobj.UserId);
                        if (Session["UserObject"] != null)
                            Session["starttime"] = DateTime.UtcNow;
                        else
                            Session["starttime"] = null;
                    }
                    // return Json(result, JsonRequestBehavior.AllowGet);
                    return Json(DateTime.UtcNow.ToString(), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("logout", JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return Json("logout", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult BookDisplay(string id)
        {
            try
            {
                if (Session["UserObject"] == null)
                    CreateSession();
                User userobj = (User)Session["UserObject"];
                BookDisplay Model = new BookDisplay();
                if (userobj != null)
                {
                    _webClient.UserId = userobj.UserId;
                    int BookId;
                    if (int.TryParse(id, out BookId))
                    {
                        Model = _webClient.DownloadData<BookDisplay>("bookdisplay", new { BookId = BookId, UserId = userobj.UserId });
                        if (Model.Status == 1)
                        {
                            HttpWebRequest request = WebRequest.Create(ConfigurationManager.AppSettings["proxypath"].ToString().Replace("#kitabletid#", Model.KitabletID) + "package.opf") as HttpWebRequest;
                            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                            WebHeaderCollection header = response.Headers;

                            var encoding = ASCIIEncoding.ASCII;
                            using (var reader = new System.IO.StreamReader(response.GetResponseStream(), encoding))
                            {
                                string responseText = reader.ReadToEnd();
                                XDocument package = XDocument.Parse(responseText);

                                XElement metadata = null;
                                XElement manifest = null;
                                XElement spine = null;

                                if (package != null)
                                {
                                    metadata = package.Root.Descendants().Where(x => x.Name.LocalName == "metadata").ToList().FirstOrDefault();
                                    manifest = package.Root.Descendants().Where(x => x.Name.LocalName == "manifest").ToList().FirstOrDefault();
                                    spine = package.Root.Descendants().Where(x => x.Name.LocalName == "spine").ToList().FirstOrDefault();
                                }

                                var menifestlist = manifest.Descendants().Where(e => e.Attribute("media-type").Value == "application/xhtml+xml").ToList();
                                var spinelist = spine.Descendants().ToList();

                                Model.OpfSpine = new List<OpfSpine>();

                                foreach (XElement ele in spinelist)
                                {
                                    OpfSpine _temp = new OpfSpine();
                                    _temp.id = menifestlist.Where(e => e.Attribute("id").Value == ele.Attribute("idref").Value).FirstOrDefault().Attribute("href").Value;
                                    _temp.href = menifestlist.Where(e => e.Attribute("id").Value == ele.Attribute("idref").Value).FirstOrDefault().Attribute("href").Value;
                                    _temp.height1 = ele.Attribute("height1").Value;
                                    _temp.height2 = ele.Attribute("height2").Value;
                                    try
                                    {
                                        _temp.pageno = ele.Attribute("pageno").Value;
                                    }
                                    catch
                                    {
                                        _temp.pageno = "";
                                    }
                                    finally
                                    {
                                        Model.OpfSpine.Add(_temp);
                                    }
                                }

                                if (metadata != null)
                                {
                                    foreach (XElement element in metadata.Elements())
                                    {
                                        if (element.Attribute("property") != null && element.Attribute("property").Value.Trim().Equals("media:height"))
                                        {
                                            Model.height = !string.IsNullOrEmpty(element.Value) ? element.Value : "0";
                                            continue;
                                        }
                                        if (element.Attribute("property") != null && element.Attribute("property").Value.Trim().Equals("media:width"))
                                        {
                                            Model.width = !string.IsNullOrEmpty(element.Value) ? element.Value : "0";
                                            continue;
                                        }
                                        if (element.Attribute("property") != null && element.Attribute("property").Value.Trim().Equals("media:Pagewidth1"))
                                        {
                                            Model.pagewidth1 = !string.IsNullOrEmpty(element.Value) ? element.Value : "0";
                                            continue;
                                        }
                                        if (element.Attribute("property") != null && element.Attribute("property").Value.Trim().Equals("media:Pagewidth2"))
                                        {
                                            Model.pagewidth2 = !string.IsNullOrEmpty(element.Value) ? element.Value : "0";
                                            continue;
                                        }
                                    }
                                }

                                var itemref = spine.Elements();
                                var strin = itemref.Attributes("idref").Select(x => x.Value);
                            }
                            return View(Model);
                        }
                        else if (Model.Status == 2)
                        {
                            //Redirect to resource not found
                            return RedirectToAction("Home", "Main");
                        }
                        else
                        {
                            //redirect to error page
                            return RedirectToAction("Home", "Main");
                        }
                    }
                    else
                    {
                        ////Redirect to resource not found
                        return RedirectToAction("Home", "Main");
                    }
                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }
            }

            catch (Exception ex)
            {
                return RedirectToAction("Home", "Main");
            }
        }

        [HttpPost]
        public JsonResult SetBookReading(int BookId, bool IsCompleted, int CurrentPage, string page1,
            string starttime, bool IsautoSync, int UserId, bool IsSessionOut)
        {
            _webClient.UserId = UserId;
            if (Session["UserObject"] == null && !IsautoSync)
                CreateSession();
            List<Page> page = new List<Page>();
            User userobj = (User)Session["UserObject"];
            page = JsonConvert.DeserializeObject<List<Page>>(page1);
            if (IsautoSync || userobj != null)
            {
                if (IsSessionOut)
                    starttime = DateTime.UtcNow.ToString();

                BookDisplay _objBookDisplay = new BookDisplay();
                _objBookDisplay.UserId = UserId;
                _objBookDisplay.BookId = BookId;
                _objBookDisplay.CurrentPage = CurrentPage;
                _objBookDisplay.page = page;
                _objBookDisplay.IsCompleted = IsCompleted;
                _objBookDisplay.StartDate = Convert.ToDateTime(starttime);
                _objBookDisplay.CompletedOn = DateTime.UtcNow;
                //int UserId, int BookId, bool IsCompleted, int CurrentPage, List<Page> page
                var result = _webClient.UploadData<int>("setbookreading", new { UserId = UserId, BookId = BookId, IsCompleted = IsCompleted, CurrentPage = CurrentPage, page = page1, Environment = "Home", Platform = "Browser", StartDate = Convert.ToDateTime(starttime), CompletedOn = DateTime.UtcNow });
                if (Session["starttime"] != null)
                {
                    (new Helper.Helper()).UpdateBrowsing("timeout", userobj.SessionId, Convert.ToDateTime(Session["starttime"]), userobj.UserId);
                    if (Session["UserObject"] != null)
                        Session["starttime"] = DateTime.UtcNow;
                    else
                        Session["starttime"] = null;
                }

                //return Json(result, JsonRequestBehavior.AllowGet);
                return Json(DateTime.UtcNow.ToString(), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("logout", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Activity(string id)
        {
            try
            {
                if (Session["UserObject"] == null)
                    CreateSession();
                User userobj = (User)Session["UserObject"];
                if (userobj != null)
                {
                    _webClient.UserId = userobj.UserId;
                    int BookId;
                    if (int.TryParse(id, out BookId))
                    {
                        BookActivity booksobj = _webClient.DownloadData<BookActivity>("getbookactivity", new { UserId = userobj.UserId, BookId = BookId });
                        if (booksobj != null)
                        {
                            switch (booksobj.Status)
                            {
                                case 1:
                                    string str = "{\"ActivityAttr\": {\"id\":" + id + ",\"readaloud\":" + booksobj.HasReadAloud.ToString().ToLower() + ",\"activity\":" + booksobj.IsActivityDone.ToString().ToLower() + ",\"animation\":" + booksobj.HasAnimation.ToString().ToLower() + ",\"rating\":" + booksobj.Rating + ",\"device\":\"browser\",\"subsection\":\"" + userobj.SubSectionId + "\",\"orientation\":\"" + booksobj.ViewMode.ToLower() + "\",\"SessionTimeOut\":\"" + 0 + "\"},";
                                    string temp = booksobj.Json.Substring(1);
                                    temp = str + temp;
                                    booksobj.Json = temp;
                                    break;
                                case 2:
                                    //No book Redirect to not found page
                                    return RedirectToAction("Home", "Main");
                                case 3:
                                    //book not read 
                                    return RedirectToAction("Home", "Main");
                                    break;
                                case 4:
                                    //no activity in this book
                                    return RedirectToAction("Home", "Main");
                                default:
                                    break;
                            }

                            return View(booksobj);
                        }
                        else
                        {
                            //redirect to api acces error page
                            return RedirectToAction("Home", "Main");
                        }
                    }
                    else
                    {
                        //redirect to not found page
                        return RedirectToAction("Home", "Main");
                    }
                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Home", "Main");
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SetBookActivity(int bookid, bool isattempted, string json, string starttime, bool IsautoSync, int UserId, bool IsSessionOut)
        {
            try
            {
                _webClient.UserId = UserId;
                if (Session["UserObject"] == null && !IsautoSync)
                    CreateSession();
                var CompletionTime = DateTime.UtcNow - Convert.ToDateTime(starttime);

                User userobj = (User)Session["UserObject"];
                if (IsautoSync || userobj != null)
                {
                    if (IsSessionOut)
                        CompletionTime = DateTime.UtcNow - DateTime.UtcNow;
                    int result = _webClient.UploadData<int>("setbookactivity", new { BookId = bookid, UserId = UserId, Json = json, IsActivityDone = isattempted, Platform = "Browser", Environment = "Home", CompletedOn = DateTime.UtcNow, CompletionTime = CompletionTime.TotalSeconds / 60 });
                    if (Session["starttime"] != null)
                    {
                        (new Helper.Helper()).UpdateBrowsing("timeout", userobj.SessionId, Convert.ToDateTime(Session["starttime"]), userobj.UserId);
                        if (Session["UserObject"] != null)
                            Session["starttime"] = DateTime.UtcNow;
                        else
                            Session["starttime"] = null;
                    }

                    return Json(DateTime.UtcNow.ToString(), JsonRequestBehavior.AllowGet);
                    //return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("logout", JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return Json("logout", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult SetReadLater(int BookId)
        {
            if (Session["UserObject"] == null)
                CreateSession();
            User userobj = (User)Session["UserObject"];
            if (userobj != null)
            {
                _webClient.UserId = userobj.UserId;
                var result = _webClient.UploadData<int>("readlater", new { UserId = userobj.UserId, BookId = BookId, });
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("logout", JsonRequestBehavior.AllowGet);
            }
        }
    }
}