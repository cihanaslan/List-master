using Microsoft.AspNetCore.Mvc;
using System.IO;
using ClosedXML.Excel;
using System.Collections.Generic;
using System.Linq;
using List.Models;
using Microsoft.EntityFrameworkCore;

namespace List.Controllers
{
    public class ExcelController : Controller
    {
        private readonly INTERNContext _context;

        public ExcelController(INTERNContext context)
        {
            _context = context;
        }

        public IActionResult ExcelData()
        {
            var excelData = _context.SdDatabases.ToList();
            return View(excelData);
        }

        public IActionResult ExportToExcel()
        {
            var excelData = _context.SdDatabases.ToList();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sheet1");

                var headers = new List<string> { "Sicil No", "Ad", "Soyad", "Bolum" };
                for (int i = 0; i < headers.Count; i++)
                {
                    worksheet.Cell(1, i + 1).Value = headers[i];
                }

                for (int i = 0; i < excelData.Count; i++)
                {
                    worksheet.Cell(i + 2, 1).Value = excelData[i].SicilNo;
                    worksheet.Cell(i + 2, 2).Value = excelData[i].Ad;
                    worksheet.Cell(i + 2, 3).Value = excelData[i].Soyad;
                    worksheet.Cell(i + 2, 4).Value = excelData[i].Bolum;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DAGITIM_WORKERS.xlsx");
                }
            }
        }

    }
}
