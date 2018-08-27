using FISE_Cloud.Filters;
using FISE_Cloud.Models;
using FISE_Cloud.Models.School;
using FISE_Cloud.Services.Authentication;
using FISE_Cloud.TWebClients;
using FISE_Cloud.Validators.School;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;

namespace FISE_Cloud.Controllers
{
    [NoCache]
    /// <summary>
    /// This controller will handle books related requests
    /// </summary>
    public class BooksController : Controller
    {
        private readonly IAuthenticationService _authService;
        private readonly ITWebClient _webClient;
        private readonly int _pageSize = int.Parse(System.Configuration.ConfigurationManager.AppSettings["PageSize"]);        
        public BooksController()
        {
            _authService = new FormsAuthenticationService();
            _webClient = new TWebClient(_authService.CurrentUserData!=null?_authService.CurrentUserData.UserId:0);
        }

        /// <summary>
        /// This is to render the book list page
        /// </summary>
        /// <returns>List of Books</returns>
        [HttpGet]
        [FISEAuthorize(InRoles = "superadmin,elibadmin")]        
        public ActionResult BooksList(int pageno = 1, int pagesize = 0)
        {
            if (pagesize <= 0)
            {
                pagesize = _pageSize;
            }
            var user = _authService.CurrentUser;
            var model = _webClient.DownloadData<BooksListAPIResult>("getbookslistwithfilter", new { PageIndex = pageno, PageSize = pagesize, SearchText = "" });
            var pmodel = new PagedList<Book>(model.Books.Items, pageno, pagesize, model.Books.TotalItems);
            var viewmodel = new BooksListResult();
            viewmodel.Books = pmodel;
            viewmodel.BookType = model.BookType;
            viewmodel.Language = model.Language;
            //viewmodel.Genre  = model.Genre;
            viewmodel.SubSection = model.SubSection;
            return View(viewmodel);
        }

        /// <summary>
        /// This is to handle the actions like paging and filtering posted from the book list page
        /// </summary>
        /// <returns>List of Books</returns>
        [HttpPost]
        [FISEAuthorize(InRoles = "superadmin,elibadmin")]       
        public ActionResult BooksList(string SearchText = "", int bpageno = 1, int pagesize = 0, string selectedSubSectionIds = "", string selectedLanguageIds = "", string selectedBookTypeIds = "", bool HasActivity = false, bool HasAnimation = false, bool HasReadAloud = false)
        {
            if (pagesize <= 0)
            {
                pagesize = _pageSize;
            }
            if (Request.IsAjaxRequest())
            {
                var model = _webClient.DownloadData<APIPagedList<Book>>("getbookslistbyfilter", new { PageIndex = bpageno, PageSize = pagesize, SearchText = "", SubSection = selectedSubSectionIds, Language = selectedLanguageIds, BookType = selectedBookTypeIds, HasActivity, HasAnimation, HasReadAloud });
                var pmodel = new PagedList<Book>(model.Items, bpageno, pagesize, model.TotalItems);
                return PartialView("_BooksListPost", pmodel);
            }
            else
            {
                var model = _webClient.DownloadData<BooksListAPIResult>("getbookslistwithfilter", new { PageIndex =bpageno, PageSize = pagesize, SearchText = "" });
                var pmodel = new PagedList<Book>(model.Books.Items, bpageno, pagesize, model.Books.TotalItems);
                var viewmodel = new BooksListResult();
                viewmodel.Books = pmodel;
                viewmodel.BookType = model.BookType;
                viewmodel.Language = model.Language;
                //viewmodel.Genre = model.Genre;
                viewmodel.SubSection = model.SubSection;
                return View(viewmodel);
            }
        }
        /// <summary>
        /// Get Book details with the help of BookId
        /// </summary>
        /// <returns>Book details and metadata</returns>
        [HttpGet]
        [FISEAuthorize(InRoles = "superadmin,elibadmin")]
        public ActionResult BookInfo(int bookid)
        {
            var model = _webClient.DownloadData<BooksListResult>("getbookdetailsbyid", new { BookId = bookid });
            if (MvcSiteMapProvider.SiteMaps.Current.CurrentNode != null)
            {
                if ((model.Book.Title).Length > 20)
                    MvcSiteMapProvider.SiteMaps.Current.CurrentNode.Title = (model.Book.Title).Substring(0,20) + "...";
                else
                    MvcSiteMapProvider.SiteMaps.Current.CurrentNode.Title = model.Book.Title;
            }
            return View(model.Book);
        }

        /// <summary>
        /// Get Book details with the help of BookId to render the page for editing
        /// </summary>
        /// <returns>Book details and metadata</returns>
        [HttpGet]
        [FISEAuthorize(InRoles = "superadmin,elibadmin")]
        public ActionResult EditBooksMetadata(int bookid)
        {
            var model = _webClient.DownloadData<BooksListResult>("getbookdetailsbyid", new { Bookid = bookid });

            if (MvcSiteMapProvider.SiteMaps.Current.CurrentNode != null)
            {
                if ((model.Book.Title).Length > 20)
                    MvcSiteMapProvider.SiteMaps.Current.CurrentNode.ParentNode.Title = (model.Book.Title).Substring(0, 20) + "...";
                else
                    MvcSiteMapProvider.SiteMaps.Current.CurrentNode.ParentNode.Title = model.Book.Title;
            }
            return View(model);
        }
        /// <summary>
        /// Update Book details
        /// </summary>
        /// <returns>Book details and metadata</returns>
        [HttpPost]
        [FISEAuthorize(InRoles = "superadmin,elibadmin")]
        
        public ActionResult EditBooksMetadata(BooksListResult model)
        {
            model.Book.Language = string.Join(",", model.SelectedLanguageIds.Select(x => x).ToList());
            model.Book.Genre = string.Join(",", model.SelectedGenreIds.Select(x => x).ToList());
            model.Book.SubSection = string.Join(",", model.SelectedSubSectionIds.Select(x => x).ToList());
            model.Book.Type = string.Join(",", model.SelectedTypeIds.Select(x => x).ToList());
            FluentValidation.IValidator<BooksListResult> validator = new EditBookValidator();
            var validationResults = validator.Validate(model);
            foreach (var item in validationResults.Errors)
            {
                ModelState.AddModelError(item.PropertyName, item.ErrorMessage);
            }

            try
            {
                if (ModelState.IsValid)
                {
                    var result = _webClient.UploadData<GenericStatus>("updatebookmetadata", model.Book);
                    switch (result)
                    {
                        case GenericStatus.Sucess:
                            return RedirectToRoute("BookInfo", new { bookid = model.Book.BookId });
                        default:
                            ModelState.AddModelError("", Resource.EditBook_error);
                            break;
                    }
                }
                else
                {
                    var model1 = _webClient.DownloadData<BooksListResult>("getbookdetailsbyid", new { Bookid = model.Book.BookId });
                    model.Language = model1.Language;
                    model.SubSection = model1.SubSection;
                    model.BookType = model1.BookType;
                    model.Genre = model1.Genre;
                    model.Book.ViewMode = model1.Book.ViewMode;
                    model.Book.Thumbnail1 = model1.Book.Thumbnail1;
                    for (int i = 0; i < model.Language.Count; i++)
                    {
                        model.Language[i].IsSelected = model.SelectedLanguageIds.Contains(model.Language[i].Id);
                    }
                    for (int i = 0; i < model.Genre.Count; i++)
                    {
                        model.Genre[i].IsSelected = model.SelectedGenreIds.Contains(model.Genre[i].Id);
                    }
                    for (int i = 0; i < model.SubSection.Count; i++)
                    {
                        model.SubSection[i].IsSelected = model.SelectedSubSectionIds.Contains(model.SubSection[i].Id);
                    }
                    for (int i = 0; i < model.BookType.Count; i++)
                    {
                        model.BookType[i].IsSelected = model.SelectedTypeIds.Contains(model.BookType[i].Id);
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            if (MvcSiteMapProvider.SiteMaps.Current.CurrentNode != null)
            {
                if (model.Book.Title!=null&& (model.Book.Title).Length > 20)
                    MvcSiteMapProvider.SiteMaps.Current.CurrentNode.ParentNode.Title = (model.Book.Title).Substring(0, 20) + "...";
                else
                    MvcSiteMapProvider.SiteMaps.Current.CurrentNode.ParentNode.Title =model.Book.Title!=null? model.Book.Title:"";
            }
            return View(model);
        }

        /// <summary>
        /// Disable a Book by making it Trashed.
        /// </summary>
        /// <returns>True/False</returns>
        [HttpPost]
        [FISEAuthorize(InRoles = "superadmin,elibadmin")]
        public ActionResult DisableBook(int bookid)
        {
            
            bool _Result = false;
            try
            {
                if (bookid != 0)
                {
                    _Result = _webClient.UploadData<bool>("disablebook", new { BookId = bookid });
                }
            }
            catch { }
            return Json(new
            {
                Status = _Result
            }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Exports the books in an excel file.
        /// </summary>
        [HttpGet]
        public ActionResult ExportSelectedBookstoExcel()
        {
            return RedirectToRoute("BooksList");
        }
        /// <summary>
        /// Exports the books in an excel file.
        /// </summary>
        /// <returns>An Excel file with list of books will be downloaded on user's device</returns>
        /// 
        [HttpPost]
        public ActionResult ExportSelectedBookstoExcel(string selectedSubSectionIds_export = "", string selectedLanguageIds_export = "", string selectedBookTypeIds_export = "", bool HasActivity_export = false, bool HasAnimation_export = false, bool HasReadAloud_export = false)
        {
            var model = _webClient.DownloadData<List<ExportBook>>("getbookslistforexport", new { SubSection = selectedSubSectionIds_export, Language = selectedLanguageIds_export, BookType = selectedBookTypeIds_export, HasActivity=HasActivity_export, HasAnimation=HasAnimation_export, HasReadAloud=HasReadAloud_export });           
            var _exportManager = new Services.ExportImport.ExportManager();
            var bytes = _exportManager.ExportBooksToXlsx(model);
            string TextXlsx = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return File(bytes, TextXlsx, "books.xlsx");
        }
    }
}