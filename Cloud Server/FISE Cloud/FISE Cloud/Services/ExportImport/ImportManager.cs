using System;
using System.IO;
using System.Linq;
using FISE_Cloud.Services.ExportImport.Help;
using OfficeOpenXml;
using FISE_Cloud.Models.School;
using System.Collections.Generic;

namespace FISE_Cloud.Services.ExportImport
{
    /// <summary>
    /// Import manager
    /// </summary>
    public partial class ImportManager
    {
        #region Methods

        /// <summary>
        /// Import Student from XLSX file
        /// </summary>
        /// <param name="stream">Stream</param>
        public List<StudentImportExport> ImportStudentsFromXlsx(Stream stream, out bool IsValidExcel)
        {
            //property array
            var properties = new[]
            {
                new PropertyByName<StudentImportExport>("SNO"),
                new PropertyByName<StudentImportExport>("FirstName"),
                new PropertyByName<StudentImportExport>("LastName"),
                new PropertyByName<StudentImportExport>("Gender"),
                new PropertyByName<StudentImportExport>("Grade"),
                new PropertyByName<StudentImportExport>("Section"),
                new PropertyByName<StudentImportExport>("RollNo"),
                new PropertyByName<StudentImportExport>("DateOfBirth"),
                new PropertyByName<StudentImportExport>("ParentEmail"),
                new PropertyByName<StudentImportExport>("ParentMobileNumber"),
                new PropertyByName<StudentImportExport>("SubscriptionStartDate"),
                new PropertyByName<StudentImportExport>("SubscriptionEndDate")
            };

            var manager = new PropertyManager<StudentImportExport>(properties);
            List<StudentImportExport> students = new List<StudentImportExport>();
            using (var xlPackage = new ExcelPackage(stream))
            {
                // get the first worksheet in the workbook
                var worksheet = xlPackage.Workbook.Worksheets.FirstOrDefault();
                IsValidExcel = true;

                if (worksheet != null)
                {
                    try
                    {
                        for (int i = 1; i <= 12; i++)
                        {

                            if (manager.GetProperties[i - 1].PropertyName.ToString() != worksheet.Cells[1, i].Value.ToString())
                            {
                                IsValidExcel = false;
                            }
                        }
                    }
                    catch {
                        IsValidExcel = false;
                    }
                    var iRow = 2;

                    while (true)
                    {
                        var allColumnsAreEmpty = manager.GetProperties
                            .Select(property => worksheet.Cells[iRow, property.PropertyOrderPosition])
                            .All(cell => cell == null || cell.Value == null || String.IsNullOrEmpty(cell.Value.ToString()));

                        if (allColumnsAreEmpty)
                            break;

                        manager.ReadFromXlsx(worksheet, iRow);
                        var student = new StudentImportExport();
                        student.SNO = manager.GetProperty("SNO").IntValue;
                        student.FirstName = manager.GetProperty("FirstName").StringValue;
                        student.LastName = manager.GetProperty("LastName").StringValue;
                        student.Gender = manager.GetProperty("Gender").StringValue;
                        student.Grade = manager.GetProperty("Grade").StringValue;
                        student.SubSection = manager.GetProperty("Section").StringValue;
                        student.RollNo = manager.GetProperty("RollNo").StringValue;
                        student.DateOfBirth = manager.GetProperty("DateOfBirth").DateTimeNullable;
                        student.ParentEmail = manager.GetProperty("ParentEmail").StringValue;/*"test@test.com";*/ //manager.GetProperty("Published").StringValue;
                        student.ParentMobileNumber = manager.GetProperty("ParentMobileNumber").StringValue;/*"1212121";*/ // manager.GetProperty("DisplayOrder").StringValue;
                        student.SubscriptionStartDate = manager.GetProperty("SubscriptionStartDate").DateTimeNullable;
                        student.SubscriptionEndDate = manager.GetProperty("SubscriptionEndDate").DateTimeNullable;
                        student.TotalFailed = string.IsNullOrEmpty(manager.GetProperty("DateOfBirth").StringValue) ? 0 : 1;
                        students.Add(student);
                        iRow++;
                    }

                }
            }

            return students;
        }

        public List<StudentImportExport> BulkUpdateStudentsFromXlsx(Stream stream, out bool IsValidExcel)
        {
            //property array
            var properties = new[]
            {
                new PropertyByName<StudentImportExport>("SNO"),
                new PropertyByName<StudentImportExport>("UniqueId"),
                new PropertyByName<StudentImportExport>("FirstName"),
                new PropertyByName<StudentImportExport>("LastName"),
                new PropertyByName<StudentImportExport>("Grade"),
                new PropertyByName<StudentImportExport>("Section"),
                new PropertyByName<StudentImportExport>("RollNo"),
                new PropertyByName<StudentImportExport>("SubscriptionStartDate"),
                new PropertyByName<StudentImportExport>("SubscriptionEndDate"),
                new PropertyByName<StudentImportExport>("IsRenew")
            };

            var manager = new PropertyManager<StudentImportExport>(properties);
            List<StudentImportExport> students = new List<StudentImportExport>();
            using (var xlPackage = new ExcelPackage(stream))
            {
                // get the first worksheet in the workbook
                var worksheet = xlPackage.Workbook.Worksheets.FirstOrDefault();
                IsValidExcel = true;

                if (worksheet != null)
                {
                    try
                    {
                        for (int i = 1; i <= 10; i++)
                        {
                            if (manager.GetProperties[i - 1].PropertyName.ToString() != worksheet.Cells[1, i].Value.ToString())
                            {
                                IsValidExcel = false;
                            }
                        }
                    }
                    catch {
                        IsValidExcel = false;
                    }
                    var iRow = 2;

                    while (true)
                    {
                        var allColumnsAreEmpty = manager.GetProperties
                            .Select(property => worksheet.Cells[iRow, property.PropertyOrderPosition])
                            .All(cell => cell == null || cell.Value == null || String.IsNullOrEmpty(cell.Value.ToString()));

                        if (allColumnsAreEmpty)
                            break;

                        manager.ReadFromXlsx(worksheet, iRow);
                        var student = new StudentImportExport();
                        student.SNO = manager.GetProperty("SNO").IntValue;
                        student.FirstName = manager.GetProperty("FirstName").StringValue;
                        student.LastName = manager.GetProperty("LastName").StringValue;
                        student.Grade = manager.GetProperty("Grade").StringValue;
                        student.SubSection = manager.GetProperty("Section").StringValue;
                        student.RollNo = manager.GetProperty("RollNo").StringValue;
                        student.SubscriptionStartDate = manager.GetProperty("SubscriptionStartDate").DateTimeNullable;
                        student.SubscriptionEndDate = manager.GetProperty("SubscriptionEndDate").DateTimeNullable;
                        student.Status = manager.GetProperty("IsRenew").StringToBooleanValue;
                        student.IsRenew = manager.GetProperty("IsRenew").StringValue;
                        student.UniqueId = manager.GetProperty("UniqueId").StringValue;
                        students.Add(student);
                        iRow++;
                    }
                }
            }
            return students;
        }


        public List<ChildImport> ImportChildrensFromXlsx(Stream stream, out bool IsValidExcel)
        {
            //property array
            var properties = new[]
            {
                new PropertyByName<ChildImport>("SNO"),
                new PropertyByName<ChildImport>("ParentFirstName"),
                new PropertyByName<ChildImport>("ParentLastName"),
                new PropertyByName<ChildImport>("ParentEmailID"),
                new PropertyByName<ChildImport>("ChildFirstName"),
                new PropertyByName<ChildImport>("ChildLastName"),
                new PropertyByName<ChildImport>("Grade"),
                new PropertyByName<ChildImport>("Section"),
                new PropertyByName<ChildImport>("SubscriptionStartDate"),
                new PropertyByName<ChildImport>("SubscriptionEndDate")                
            };

            var manager = new PropertyManager<ChildImport>(properties);
            List<ChildImport> children = new List<ChildImport>();
            using (var xlPackage = new ExcelPackage(stream))
            {
                // get the first worksheet in the workbook
                var worksheet = xlPackage.Workbook.Worksheets.FirstOrDefault();
                IsValidExcel = true;

                if (worksheet != null)
                {
                    try
                    {
                        for (int i = 1; i <= 10; i++)
                        {

                            if (manager.GetProperties[i - 1].PropertyName.ToString() != worksheet.Cells[1, i].Value.ToString())
                            {
                                IsValidExcel = false;
                            }
                        }
                    }
                    catch
                    {
                        IsValidExcel = false;
                    }
                    var iRow = 2;

                    while (true)
                    {
                        var allColumnsAreEmpty = manager.GetProperties
                            .Select(property => worksheet.Cells[iRow, property.PropertyOrderPosition])
                            .All(cell => cell == null || cell.Value == null || String.IsNullOrEmpty(cell.Value.ToString()));

                        if (allColumnsAreEmpty)
                            break;

                        manager.ReadFromXlsx(worksheet, iRow);
                        var child = new ChildImport();
                        child.SNO = manager.GetProperty("SNO").IntValue;
                        child.ChildFirstName = manager.GetProperty("ChildFirstName").StringValue;
                        child.ChildLastName = manager.GetProperty("ChildLastName").StringValue;
                        child.Grade = manager.GetProperty("Grade").StringValue;
                        child.SubSection = manager.GetProperty("Section").StringValue;
                        child.ParentEmailID = manager.GetProperty("ParentEmailID").StringValue;/*"test@test.com";*/ //manager.GetProperty("Published").StringValue;
                        child.SubscriptionStartDate = manager.GetProperty("SubscriptionStartDate").DateTimeNullable;
                        child.SubscriptionEndDate = manager.GetProperty("SubscriptionEndDate").DateTimeNullable;
                        child.ParentFirstName = manager.GetProperty("ParentFirstName").StringValue;
                        child.ParentLastName = manager.GetProperty("ParentLastName").StringValue;
                        children.Add(child);
                        iRow++;
                    }

                }
            }

            return children;
        }
        #endregion

    }
}
