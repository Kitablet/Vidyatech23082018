using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using FISE_Cloud.Services.ExportImport.Help;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using FISE_Cloud.Models.School;
using FISE_Cloud.Models;

namespace FISE_Cloud.Services.ExportImport
{
    /// <summary>
    /// Export manager
    /// </summary>
    public partial class ExportManager
    {

        #region Utilities

        protected virtual void SetCaptionStyle(ExcelStyle style)
        {
            style.Fill.PatternType = ExcelFillStyle.Solid;
            style.Fill.BackgroundColor.SetColor(Color.FromArgb(184, 204, 228));
            style.Font.Bold = true;
        }


        /// <summary>
        /// Export objects to XLSX
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <param name="properties">Class access to the object through its properties</param>
        /// <param name="itemsToExport">The objects to export</param>
        /// <returns></returns>
        protected virtual byte[] ExportToXlsx<T>(PropertyByName<T>[] properties, IEnumerable<T> itemsToExport)
        {
            using (var stream = new MemoryStream())
            {
                // ok, we can run the real code of the sample now
                using (var xlPackage = new ExcelPackage(stream))
                {
                    // uncomment this line if you want the XML written out to the outputDir
                    //xlPackage.DebugMode = true; 

                    // get handle to the existing worksheet
                    var worksheet = xlPackage.Workbook.Worksheets.Add(typeof(T).Name);
                    //create Headers and format them 

                    var manager = new PropertyManager<T>(properties.Where(p => !p.Ignore).ToArray());
                    manager.WriteCaption(worksheet, SetCaptionStyle);

                    var row = 2;
                    foreach (var items in itemsToExport)
                    {
                        manager.CurrentObject = items;
                        manager.WriteToXlsx(worksheet, row++);
                    }

                    xlPackage.Save();
                }
                return stream.ToArray();
            }
        }

        protected virtual byte[] ExportToXlsx<T, V>(PropertyByName<T>[] properties, IEnumerable<T> itemsToExport, PropertyByName<V>[] properties1, IEnumerable<V> itemsToExport1,string sheet1,string sheet2)
        {
            using (var stream = new MemoryStream())
            {
                // ok, we can run the real code of the sample now
                using (var xlPackage = new ExcelPackage(stream))
                {
                    // uncomment this line if you want the XML written out to the outputDir
                    //xlPackage.DebugMode = true; 

                    // get handle to the existing worksheet
                    var worksheet = xlPackage.Workbook.Worksheets.Add(sheet1);
                    var worksheet1 = xlPackage.Workbook.Worksheets.Add(sheet2);
                    //create Headers and format them 

                    var manager = new PropertyManager<T>(properties.Where(p => !p.Ignore).ToArray());
                    manager.WriteCaption(worksheet, SetCaptionStyle);

                   var manager1 = new PropertyManager<V>(properties1.Where(p => !p.Ignore).ToArray());
                    manager1.WriteCaption(worksheet1, SetCaptionStyle);

                    var row = 2;
                    foreach (var items in itemsToExport)
                    {
                        manager.CurrentObject = items;
                        manager.WriteToXlsx(worksheet, row++);
                    }

                    var row1 = 2;
                    foreach (var items in itemsToExport1)
                    {
                        manager1.CurrentObject = items;
                        manager1.WriteToXlsx(worksheet1, row1++);
                    }

                    xlPackage.Save();
                }
                return stream.ToArray();
            }
        }

        #endregion

        #region Methods


        /// <summary>
        /// Export students to XLSX
        /// </summary>
        /// <param name="students">Manufactures</param>
        public virtual byte[] ExportStudentsToXlsx(IEnumerable<StudentImportExport> students)
        {
            //property array
            var properties = new[]
            {
                new PropertyByName<StudentImportExport>("SNO", p => p.SNO),
                new PropertyByName<StudentImportExport>("UniqueId", p =>p.UniqueId),
                new PropertyByName<StudentImportExport>("FirstName", p => p.FirstName),
                new PropertyByName<StudentImportExport>("LastName", p => p.LastName),
                new PropertyByName<StudentImportExport>("Grade", p => p.Grade),
                new PropertyByName<StudentImportExport>("Section", p => p.SubSection),
                new PropertyByName<StudentImportExport>("RollNo", p => p.RollNo),
                new PropertyByName<StudentImportExport>("SubscriptionStartDate", p => p.SubscriptionStartDate.HasValue?p.SubscriptionStartDate.Value.ToString("d MMMM yyyy"):""),
                new PropertyByName<StudentImportExport>("SubscriptionEndDate", p => p.SubscriptionEndDate.HasValue?p.SubscriptionEndDate.Value.ToString("d MMMM yyyy"):""),
                new PropertyByName<StudentImportExport>("IsRenew", p => String.IsNullOrEmpty(p.IsRenew)?"No":p.IsRenew)
                
            };

            return ExportToXlsx(properties, students);
        }
        /// <summary>
        /// Export students to XLSX
        /// </summary>
        /// <param name="students">Manufactures</param>
        public virtual byte[] ExportFailedStudentsToXlsx(IEnumerable<StudentImportExport> students)
        {
            //property array
            var properties = new[]
            {
                new PropertyByName<StudentImportExport>("SNO", p => p.SNO),
                new PropertyByName<StudentImportExport>("UniqueId", p =>p.UniqueId),
                new PropertyByName<StudentImportExport>("FirstName", p => p.FirstName),
                new PropertyByName<StudentImportExport>("LastName", p => p.LastName),
                new PropertyByName<StudentImportExport>("Grade", p => p.Grade),
                new PropertyByName<StudentImportExport>("Section", p => p.SubSection),
                new PropertyByName<StudentImportExport>("RollNo", p => p.RollNo),
                new PropertyByName<StudentImportExport>("SubscriptionStartDate", p => p.SubscriptionStartDate.HasValue?p.SubscriptionStartDate.Value.ToString("d MMMM yyyy"):""),
                new PropertyByName<StudentImportExport>("SubscriptionEndDate", p => p.SubscriptionEndDate.HasValue?p.SubscriptionEndDate.Value.ToString("d MMMM yyyy"):""),
                new PropertyByName<StudentImportExport>("IsRenew", p => String.IsNullOrEmpty(p.IsRenew)?"No":p.IsRenew),
                new PropertyByName<StudentImportExport>("UploadSuccess", p => p.Status)
            };

            return ExportToXlsx(properties, students);
        }
        public virtual byte[] ExportFaliedToImportStudentsToXlsx(IEnumerable<StudentImportExport> students)
        {
            //property array
            var properties = new[]
            {
                new PropertyByName<StudentImportExport>("SNO", p => p.SNO),
                new PropertyByName<StudentImportExport>("FirstName", p => p.FirstName),
                new PropertyByName<StudentImportExport>("LastName", p => p.LastName),
                new PropertyByName<StudentImportExport>("Gender", p => p.Gender),
                new PropertyByName<StudentImportExport>("Grade", p => p.Grade),
                new PropertyByName<StudentImportExport>("Section", p => p.SubSection), 
                new PropertyByName<StudentImportExport>("RollNo", p => p.RollNo),
                new PropertyByName<StudentImportExport>("DateOfBirth", p => p.DateOfBirth.HasValue?p.DateOfBirth.Value.ToString("d MMMM yyyy"):""),
                new PropertyByName<StudentImportExport>("ParentEmail", p => p.ParentEmail),
                new PropertyByName<StudentImportExport>("ParentMobileNumber", p => p.ParentMobileNumber),
                new PropertyByName<StudentImportExport>("SubscriptionStartDate", p => p.SubscriptionStartDate.HasValue?p.SubscriptionStartDate.Value.ToString("d MMMM yyyy"):""),
                new PropertyByName<StudentImportExport>("SubscriptionEndDate", p => p.SubscriptionEndDate.HasValue?p.SubscriptionEndDate.Value.ToString("d MMMM yyyy"):""),
                new PropertyByName<StudentImportExport>("UploadSuccess", p => p.Status)
            };
            return ExportToXlsx(properties, students);
        }
        public virtual byte[] ExportFaliedToImportChildrenToXlsx(IEnumerable<ChildImport> students)
        {
            //property array
            var properties = new[]
            {
                new PropertyByName<ChildImport>("SNO", p => p.SNO),
                new PropertyByName<ChildImport>("ChildFirstName", p => p.ChildFirstName),
                new PropertyByName<ChildImport>("ChildLastName", p => p.ChildLastName),
                new PropertyByName<ChildImport>("Grade", p => p.Grade),
                new PropertyByName<ChildImport>("ParentEmailID", p => p.ParentEmailID),
                new PropertyByName<ChildImport>("SubscriptionStartDate", p => p.SubscriptionStartDate.HasValue?p.SubscriptionStartDate.Value.ToString("d MMMM yyyy"):""),
                new PropertyByName<ChildImport>("SubscriptionEndDate", p => p.SubscriptionEndDate.HasValue?p.SubscriptionEndDate.Value.ToString("d MMMM yyyy"):""),
                new PropertyByName<ChildImport>("ParentFirstName", p => p.ParentFirstName),
                new PropertyByName<ChildImport>("ParentLastName", p => p.ParentLastName),
                new PropertyByName<ChildImport>("UploadSuccess", p => p.Status)
            };
            return ExportToXlsx(properties, students);
        }
        #endregion

        public virtual byte[] ExportSchoolStudentsToXlsx(IEnumerable<StudentRegistrationModel> students)
        {
            //property array
            var properties = new[]
            {
                new PropertyByName<StudentRegistrationModel>("First Name", p => p.FirstName),
                new PropertyByName<StudentRegistrationModel>("Last Name", p => p.LastName),
                new PropertyByName<StudentRegistrationModel>("Student Username", p => p.Username),
                new PropertyByName<StudentRegistrationModel>("Grade", p => p.Grade),
                new PropertyByName<StudentRegistrationModel>("Section", p => p.SubSection),
                new PropertyByName<StudentRegistrationModel>("Gender", p => p.Gender),
                new PropertyByName<StudentRegistrationModel>("Creation Date", p => p.CreationDate.ToString("d MMMM yyyy")),
                new PropertyByName<StudentRegistrationModel>("Registration Date", p => p.RegistrationDate.HasValue?p.RegistrationDate.Value.ToString("d MMMM yyyy"):""),
                new PropertyByName<StudentRegistrationModel>("Subscription Start Date", p => p.SubscriptionStartDate.HasValue?p.SubscriptionStartDate.Value.ToString("d MMMM yyyy"):""),
                new PropertyByName<StudentRegistrationModel>("Subscription End Date", p => p.SubscriptionEndDate.HasValue?p.SubscriptionEndDate.Value.ToString("d MMMM yyyy"):""),
                //new PropertyByName<StudentImportExport>("Roll No", p => p.RollNo),
                new PropertyByName<StudentRegistrationModel>("Date Of Birth", p => p.DateOfBirth.HasValue?p.DateOfBirth.Value.ToString("d MMMM yyyy"):""),
                new PropertyByName<StudentRegistrationModel>("Parent Email", p => p.ParentEmail),
                new PropertyByName<StudentRegistrationModel>("Parent Mobile Number", p => p.ParentMobileNo),
                new PropertyByName<StudentRegistrationModel>("Status", p => p.Result),
                new PropertyByName<StudentRegistrationModel>("Parent Username", p => p.ParentUsername),
                new PropertyByName<StudentRegistrationModel>("Parent First Name", p => p.ParentFirstname),
                new PropertyByName<StudentRegistrationModel>("Parent Last Name", p => p.ParentLastname),
            };

            return ExportToXlsx(properties, students);
        }

        public virtual byte[] ExportBooksToXlsx(IEnumerable<ExportBook> books)
        {
            //property array
            var properties = new[]
            {
                //new PropertyByName<ExportBook>("BookId", p => p.BookId),
                new PropertyByName<ExportBook>("KitabletID", p => p.KitabletID),
                //new PropertyByName<ExportBook>("Folder Name", p => p.FolderName),
                new PropertyByName<ExportBook>("Title", p => p.Title),
                //new PropertyByName<ExportBook>("Hinglish Title", p=> p.HinglishTitle),
                new PropertyByName<ExportBook>("Author", p => p.Author),
                new PropertyByName<ExportBook>("Short Description", p => p.ShortDescription),
                //new PropertyByName<ExportBook>("Genre", p=>p.Genre),
                new PropertyByName<ExportBook>("Language", p => p.Language),
                new PropertyByName<ExportBook>("SubSection",p=>p.SubSection),
                new PropertyByName<ExportBook>("Type", p=>p.Type),
                new PropertyByName<ExportBook>("Illustrator", p => p.Illustrator),
                new PropertyByName<ExportBook>("Translator", p => p.Translator),
                //new PropertyByName<ExportBook>("ISBN", p=>p.ISBN),
                //new PropertyByName<ExportBook>("No Of Pages", p => p.NoOfPages),
                new PropertyByName<ExportBook>("Publisher", p => p.Publisher),
                new PropertyByName<ExportBook>("Enabled", p => p.Enabled),
                //new PropertyByName<ExportBook>("Thumbnail1",p=>p.Thumbnail1),
                //new PropertyByName<ExportBook>("Thumbnail2",p=>p.Thumbnail2),
                //new PropertyByName<ExportBook>("Thumbnail3",p=>p.Thumbnail3),
                //new PropertyByName<ExportBook>("View Mode", p => p.ViewMode),
                //new PropertyByName<ExportBook>("Page Display",p=>p.PageDisplay),
                //new PropertyByName<ExportBook>("BackCover", p=> p.BackCover),
                //new PropertyByName<ExportBook>("HasActivity", p=> p.HasActivity),
                //new PropertyByName<ExportBook>("HasAnimation", p=> p.HasAnimation),
                //new PropertyByName<ExportBook>("HasReadAloud", p=> p.HasReadAloud),
            };

            return ExportToXlsx(properties, books);
        }

        public virtual byte[] ExportSchoolHomePageDetailsToXlsx(IEnumerable<SchoolDetails> school)
        {
            //property array
            var properties = new[]
            {
                new PropertyByName<SchoolDetails>("School Name", p => p.SchoolDetail.SchoolName),
              //  new PropertyByName<SchoolDetails>("School Id", p => p.SchoolDetail.SchoolId),
                new PropertyByName<SchoolDetails>("SchoolUId", p => p.SchoolDetail.SchoolUId),
                new PropertyByName<SchoolDetails>("No. of School Librarians", p => p.SchoolDetail.SchoolAdminCount),
                new PropertyByName<SchoolDetails>("No. of Students", p=>p.SchoolDetail.StudentCount),
                //new PropertyByName<School>("Is Active", p => p.IsActive),
            };

            return ExportToXlsx(properties, school);
        }

        public virtual byte[] ExportSchoolsToXlsx(IEnumerable<School> schools)
        {
            //property array
            var properties = new[]
            {
                //new PropertyByName<School>("School Id", p => p.SchoolId),
                new PropertyByName<School>("SchoolUId", p => p.SchoolUId),
                new PropertyByName<School>("School Name", p => p.SchoolName),
                new PropertyByName<School>("Address Line1", p => p.AddressLine1),
                new PropertyByName<School>("Address Line2",p=>p.AddressLine2),
                new PropertyByName<School>("City", p => p.City),
                new PropertyByName<School>("State", p=> p.State),
                new PropertyByName<School>("Country", p => p.Country),
                new PropertyByName<School>("Pin Code", p => p.PinCode),
                new PropertyByName<School>("Principal Name", p => p.PrincipalName),
                new PropertyByName<School>("Principal Email", p => p.PrincipalEmail),
                new PropertyByName<School>("Phone Number", p => p.PhoneNumber),
                new PropertyByName<School>("No. of Students", p=>p.StudentCount),
                new PropertyByName<School>("No. of School Librarians", p => p.SchoolAdminCount),
                new PropertyByName<School>("Is Active", p => p.Result),
            };

            return ExportToXlsx(properties, schools);
        }

        public virtual byte[] ExportSchoolAdminsToXlsx(IEnumerable<UserRegistrationModel> schooladmins)
        {
            //property array
            var properties = new[]
            {
                //new PropertyByName<UserRegistrationModel>("User Id", p => p.UserId),
                new PropertyByName<UserRegistrationModel>("First Name", p => p.FirstName),
                new PropertyByName<UserRegistrationModel>("Last Name", p => p.LastName),
                new PropertyByName<UserRegistrationModel>("Email", p => p.Email),
                new PropertyByName<UserRegistrationModel>("Mobile Number", p => p.MobileNumber),
                new PropertyByName<UserRegistrationModel>("Username", p => p.Username),
                new PropertyByName<UserRegistrationModel>("Gender", p => p.Gender),
                new PropertyByName<UserRegistrationModel>("Creation Date", p => p.CreationDate.ToString("d MMMM yyyy")),
                new PropertyByName<UserRegistrationModel>("Registration Date", p => p.RegistrationDate.HasValue?p.RegistrationDate.Value.ToString("d MMMM yyyy"):""),
                new PropertyByName<UserRegistrationModel>("Status", p => p.Result),
            };

            return ExportToXlsx(properties, schooladmins);
        }

        public virtual byte[] ExportElibraryAdminsToXlsx(IEnumerable<UserRegistrationModel> elibadmins)
        {
            //property array
            var properties = new[]
            {
                //new PropertyByName<UserRegistrationModel>("User Id", p => p.UserId),
                new PropertyByName<UserRegistrationModel>("First Name", p => p.FirstName),
                new PropertyByName<UserRegistrationModel>("Last Name", p => p.LastName),
                new PropertyByName<UserRegistrationModel>("Email", p => p.Email),
                new PropertyByName<UserRegistrationModel>("Mobile Number", p => p.MobileNumber),
                new PropertyByName<UserRegistrationModel>("Username", p => p.Username),
                new PropertyByName<UserRegistrationModel>("Gender", p => p.Gender),
                new PropertyByName<UserRegistrationModel>("Creation Date", p => p.CreationDate.ToString("d MMMM yyyy")),
                new PropertyByName<UserRegistrationModel>("Registration Date", p => p.RegistrationDate.HasValue?p.RegistrationDate.Value.ToString("d MMMM yyyy"):""),
                new PropertyByName<UserRegistrationModel>("Status", p => p.Result),
            };

            return ExportToXlsx(properties, elibadmins);
        }

        public virtual byte[] ExportReport6ToXlsx(IEnumerable<Book> books)
        {
            //property array
            var properties = new[]
            {
                new PropertyByName<Book>("BookId", p => p.BookId),
                new PropertyByName<Book>("Title", p => p.Title),
                new PropertyByName<Book>("Author", p => p.Author),
                new PropertyByName<Book>("Short Description", p => p.ShortDescription),
                new PropertyByName<Book>("Genre", p=>p.Genre),
                new PropertyByName<Book>("Language", p => p.Language),
                new PropertyByName<Book>("SubSection",p=>p.SubSection),
                new PropertyByName<Book>("Type", p=>p.Type),
                new PropertyByName<Book>("Illustrator", p => p.Illustrator),
                new PropertyByName<Book>("Translator", p => p.Translator),
                new PropertyByName<Book>("Publisher", p => p.Publisher),
                new PropertyByName<Book>("BackCover", p=> p.BackCover),
                new PropertyByName<Book>("HasActivity", p=> p.HasActivity),
                new PropertyByName<Book>("HasAnimation", p=> p.HasAnimation),
                new PropertyByName<Book>("HasReadAloud", p=> p.HasReadAloud),
            };

            return ExportToXlsx(properties, books);
        }

        public virtual byte[] ExportReport1ToXlsx(IEnumerable<StudentRegistrationModel> Users)
        {
            //property array
            var properties = new[]
            {
                new PropertyByName<StudentRegistrationModel>("First Name", p => p.FirstName),
                new PropertyByName<StudentRegistrationModel>("Last Name", p => p.LastName),
                new PropertyByName<StudentRegistrationModel>("Email", p => p.Email),
                new PropertyByName<StudentRegistrationModel>("Mobile Number", p=>p.MobileNumber),
                new PropertyByName<StudentRegistrationModel>("Username", p => p.Username),
                new PropertyByName<StudentRegistrationModel>("Gender",p=>p.Gender),
                new PropertyByName<StudentRegistrationModel>("Creation Date", p=> p.CreationDate.ToString("d MMMM yyyy")),
                new PropertyByName<StudentRegistrationModel>("Registration Date", p =>p.RegistrationDate.HasValue? p.RegistrationDate.Value.ToString("d MMMM yyyy"):""),
                new PropertyByName<StudentRegistrationModel>("Status", p => p.Result),
                 new PropertyByName<StudentRegistrationModel>("SchoolName", p => p.SchoolName) ,
                 new PropertyByName<StudentRegistrationModel>("Grade", p => p.Grade) 
            };

            return ExportToXlsx(properties, Users);
        }

        public virtual byte[] ExportReport7ToXlsx(IEnumerable<Report7ExportModel> Users)
        {
            //property array
            var properties = new[]
            {
                new PropertyByName<Report7ExportModel>("First Name", p => p.FirstName),
                new PropertyByName<Report7ExportModel>("Last Name", p => p.LastName),
                new PropertyByName<Report7ExportModel>("UserName", p => p.UserName),
                //new PropertyByName<Report7ExportModel>("Activity Type", p => p.ActivityType),
                //new PropertyByName<Report7ExportModel>("Activity Date", p=>p.ActivityDate),
                //new PropertyByName<Report7ExportModel>("Activity Duration", p => p.ActivityDuration),
                new PropertyByName<Report7ExportModel>("Platform",p=>p.Platform),
                new PropertyByName<Report7ExportModel>("Environment", p=>p.Environment),
                new PropertyByName<Report7ExportModel>("SchoolName", p=>p.SchoolName),
                 new PropertyByName<Report7ExportModel>("Grade", p=>p.Grade)
            };

            return ExportToXlsx(properties, Users);
        }

        public virtual byte[] ExportReport2ToXlsx(IEnumerable<Report2Export> Users)
        {
            //property array
            var properties = new[]
            {
                new PropertyByName<Report2Export>("SchoolName", p => p.SchoolName),
                new PropertyByName<Report2Export>("FirstName", p => p.FirstName),
                new PropertyByName<Report2Export>("LastName", p => p.LastName),
                new PropertyByName<Report2Export>("City", p => p.City),
                new PropertyByName<Report2Export>("Section", p=>p.Section),
                new PropertyByName<Report2Export>("TimeSpentActivity", p => p.TimeSpentActivity),
                new PropertyByName<Report2Export>("TimeSpentBookRead",p=>p.TimeSpentBookRead),
                new PropertyByName<Report2Export>("TimeSpentBrowsing", p=>p.TimeSpentBrowsing),
                new PropertyByName<Report2Export>("TotalActivitiesCompleted", p => p.TotalActivitiesCompleted),
                //new PropertyByName<Report2Export>("TotalBookRated",p=>p.TotalBookRated),
                new PropertyByName<Report2Export>("TotalBookRead", p=>p.TotalBookRead),
                new PropertyByName<Report2Export>("SubSection", p=>p.SubSection),
                new PropertyByName<Report2Export>("Grade", p=>p.Grade)
            };

            return ExportToXlsx(properties, Users);
        }

        public virtual byte[] ExportReport3ToXlsx(IEnumerable<Report3Export> Books)
        {
            //property array
            var properties = new[]
            {
                new PropertyByName<Report3Export>("Title", p => p.Title),
                new PropertyByName<Report3Export>("Language", p => p.Language),
                new PropertyByName<Report3Export>("SubSection", p => p.SubSection),
                new PropertyByName<Report3Export>("Read Grade1", p => p.Grade1Read),
                new PropertyByName<Report3Export>("Read Grade2", p=>p.Grade2Read),
                new PropertyByName<Report3Export>("Read Grade3", p => p.Grade3Read),
                new PropertyByName<Report3Export>("Read Grade4", p=>p.Grade4Read),
                new PropertyByName<Report3Export>("Read Grade5", p => p.Grade5Read),
                new PropertyByName<Report3Export>("Read Grade6", p=>p.Grade6Read),
                new PropertyByName<Report3Export>("Read Grade7", p => p.Grade7Read),
                new PropertyByName<Report3Export>("Read Grade8", p=>p.Grade8Read),
                new PropertyByName<Report3Export>("Activity Grade1", p => p.Grade1Activity),
                new PropertyByName<Report3Export>("Activity Grade2", p=>p.Grade2Activity),
                new PropertyByName<Report3Export>("Activity Grade3", p => p.Grade3Activity),
                new PropertyByName<Report3Export>("Activity Grade4", p=>p.Grade4Activity),
                new PropertyByName<Report3Export>("Activity Grade5", p => p.Grade5Activity),
                new PropertyByName<Report3Export>("Activity Grade6", p=>p.Grade6Activity),
                new PropertyByName<Report3Export>("Activity Grade7", p => p.Grade7Activity),
                new PropertyByName<Report3Export>("Activity Grade8", p=>p.Grade8Activity),
                new PropertyByName<Report3Export>("Rating 1", p=>p.Rating1),
                new PropertyByName<Report3Export>("Rating 2", p => p.Rating2),
                new PropertyByName<Report3Export>("Rating 3", p=>p.Rating3),
                new PropertyByName<Report3Export>("Rating 4", p => p.Rating4),
                new PropertyByName<Report3Export>("Rating 5", p=>p.Rating5),
                 new PropertyByName<Report3Export>("Total Read", p => p.TotalRead),
                new PropertyByName<Report3Export>("Total Read Completed", p=>p.TotalReadCompleted)
            };

            return ExportToXlsx(properties, Books);
        }

        public virtual byte[] ExportReport4ToXlsx(IEnumerable<Report3Export> Books)
        {
            //property array
            var properties = new[]
            {
                new PropertyByName<Report3Export>("Title", p => p.Title),
                new PropertyByName<Report3Export>("Publisher", p => p.Publisher),
                new PropertyByName<Report3Export>("Language", p => p.Language),
                new PropertyByName<Report3Export>("SubSection", p => p.SubSection),
                new PropertyByName<Report3Export>("Read Grade1", p => p.Grade1Read),
                new PropertyByName<Report3Export>("Read Grade2", p=>p.Grade2Read),
                new PropertyByName<Report3Export>("Read Grade3", p => p.Grade3Read),
                new PropertyByName<Report3Export>("Read Grade4", p=>p.Grade4Read),
                new PropertyByName<Report3Export>("Read Grade5", p => p.Grade5Read),
                new PropertyByName<Report3Export>("Read Grade6", p=>p.Grade6Read),
                new PropertyByName<Report3Export>("Read Grade7", p => p.Grade7Read),
                new PropertyByName<Report3Export>("Read Grade8", p=>p.Grade8Read),
                new PropertyByName<Report3Export>("Rating 1", p=>p.Rating1),
                new PropertyByName<Report3Export>("Rating 2", p => p.Rating2),
                new PropertyByName<Report3Export>("Rating 3", p=>p.Rating3),
                new PropertyByName<Report3Export>("Rating 4", p => p.Rating4),
                new PropertyByName<Report3Export>("Rating 5", p=>p.Rating5),
                 new PropertyByName<Report3Export>("Total Read", p => p.TotalRead),
                new PropertyByName<Report3Export>("Total Read Completed", p=>p.TotalReadCompleted)
            };

            return ExportToXlsx(properties, Books);
        }

        public virtual byte[] ExportReport5ToXlsx(IEnumerable<Report5Export> data, IEnumerable<Report5Export> data1)
        {
            //property array
            var properties = new[]
            {
                new PropertyByName<Report5Export>("OverAll", p => p.OverAll),
                new PropertyByName<Report5Export>("School", p => p.School),
                new PropertyByName<Report5Export>("OverAll Grade1", p => p.OverAllGrade1),
                new PropertyByName<Report5Export>("School Grade1", p => p.SchoolGrade1),
                new PropertyByName<Report5Export>("OverAll Grade2", p => p.OverAllGrade2),
                new PropertyByName<Report5Export>("School Grade2", p=>p.SchoolGrade2),
                new PropertyByName<Report5Export>("OverAll Grade3", p => p.OverAllGrade3),
                new PropertyByName<Report5Export>("School Grade3", p=>p.SchoolGrade3),
                new PropertyByName<Report5Export>("OverAll Grade4", p => p.OverAllGrade4),
                new PropertyByName<Report5Export>("School Grade4", p=>p.SchoolGrade4),
                new PropertyByName<Report5Export>("OverAll Grade5", p => p.OverAllGrade5),
                new PropertyByName<Report5Export>("School Grade5", p=>p.SchoolGrade5),
                new PropertyByName<Report5Export>("OverAll Grade6", p=>p.OverAllGrade6),
                new PropertyByName<Report5Export>("School Grade6", p => p.SchoolGrade6),
                new PropertyByName<Report5Export>("OverAll Grade7", p=>p.OverAllGrade7),
                new PropertyByName<Report5Export>("School Grade7", p => p.SchoolGrade7),
                new PropertyByName<Report5Export>("OverAll Grade8", p=>p.OverAllGrade8),
                 new PropertyByName<Report5Export>("School Grade8", p => p.SchoolGrade8),
            };

            var properties1 = new[]
            {
                new PropertyByName<Report5Export>("OverAll", p => p.OverAll),
                new PropertyByName<Report5Export>("School", p => p.School),
                new PropertyByName<Report5Export>("OverAll Grade1", p => p.OverAllGrade1),
                new PropertyByName<Report5Export>("School Grade1", p => p.SchoolGrade1),
                new PropertyByName<Report5Export>("OverAll Grade2", p => p.OverAllGrade2),
                new PropertyByName<Report5Export>("School Grade2", p=>p.SchoolGrade2),
                new PropertyByName<Report5Export>("OverAll Grade3", p => p.OverAllGrade3),
                new PropertyByName<Report5Export>("School Grade3", p=>p.SchoolGrade3),
                new PropertyByName<Report5Export>("OverAll Grade4", p => p.OverAllGrade4),
                new PropertyByName<Report5Export>("School Grade4", p=>p.SchoolGrade4),
                new PropertyByName<Report5Export>("OverAll Grade5", p => p.OverAllGrade5),
                new PropertyByName<Report5Export>("School Grade5", p=>p.SchoolGrade5),
                new PropertyByName<Report5Export>("OverAll Grade6", p=>p.OverAllGrade6),
                new PropertyByName<Report5Export>("School Grade6", p => p.SchoolGrade6),
                new PropertyByName<Report5Export>("OverAll Grade7", p=>p.OverAllGrade7),
                new PropertyByName<Report5Export>("School Grade7", p => p.SchoolGrade7),
                new PropertyByName<Report5Export>("OverAll Grade8", p=>p.OverAllGrade8),
                 new PropertyByName<Report5Export>("School Grade8", p => p.SchoolGrade8),
            };

            return ExportToXlsx(properties, data,properties1,data1,"Book Read","Time Spent");
        }

        public virtual byte[] ExportReport8ToXlsx(IEnumerable<Report8Export> Users)
        {
            //property array
            var properties = new[]
            {
                new PropertyByName<Report8Export>("Title", p => p.Title),
                new PropertyByName<Report8Export>("First Name", p => p.FirstName),
                new PropertyByName<Report8Export>("Last Name", p => p.LastName),
                new PropertyByName<Report8Export>("Rated", p => p.Rated),
                new PropertyByName<Report8Export>("Activity Done", p => p.ActivityDone),
                new PropertyByName<Report8Export>("Reading Time", p=>p.ReadingTime),
                new PropertyByName<Report8Export>("Activity Time", p => p.ActivityTime),
                new PropertyByName<Report8Export>("Browsing Time", p=>p.BrowsingTime),
            };

            return ExportToXlsx(properties, Users);
        }
    }
}
