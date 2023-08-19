using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using List.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ExcelDataReader;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Microsoft.Data.SqlClient;
using OfficeOpenXml;

namespace List.Controllers
{
    public class SdDatabasesController : Controller
    {
        private readonly INTERNContext _context;

        public SdDatabasesController(INTERNContext context)
        {
            _context = context;
        }

        private bool SdDatabaseExists(int id)
        {
            return _context.SdDatabases.Any(e => e.SicilNo == id);
        }

        public async Task<IActionResult> Index()
        {
            if (_context.SdDatabases == null)
            {
                return Problem("Entity set 'INTERNContext.SdDatabases' is null.");
            }

            var sdDatabases = await _context.SdDatabases.ToListAsync();
            return View(sdDatabases);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.SdDatabases == null)
            {
                return NotFound();
            }

            var sdDatabase = await _context.SdDatabases
                .FirstOrDefaultAsync(m => m.SicilNo == id);
            if (sdDatabase == null)
            {
                return NotFound();
            }

            return View(sdDatabase);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SicilNo,Ad,Soyad,Bolum,DagitimId,EkleyenKisi,EklendigiTarih,Flos,VerilisTarih")] SdDatabase sdDatabase)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sdDatabase);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(sdDatabase);
        }
        [HttpDelete]
        public IActionResult ClearTable()
        {
            try
            {
                _context.Database.ExecuteSqlRaw("DELETE FROM SD_DATABASE"); 

                return Ok(new { message = "Tablo başarıyla temizlendi." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Tablo temizlenirken bir hata oluştu: " + ex.Message });
            }
        }

        [HttpPost]
        public IActionResult SaveToDatabase([FromBody] List<SdDatabase> data)
        {
            if (data != null && data.Count > 0)
            {
                var insertQuery = "INSERT INTO SD_DATABASE (SICIL_NO, AD, SOYAD, BOLUM, DAGITIM_ID, EKLEYEN_KISI, EKLENDIGI_TARIH, FLOS, VERILIS_TARIH) " +
                                  "VALUES (@p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8)";
                Console.WriteLine(JsonConvert.SerializeObject(data, Formatting.Indented));

                using (var dbContext = new INTERNContext())
                {
                    Console.WriteLine($"ProviderName: {dbContext.Database.ProviderName}");
                    Console.WriteLine($"ConnectionString: {dbContext.Database.GetConnectionString()}");

                    using (var transaction = dbContext.Database.BeginTransaction())
                    {
                        foreach (var item in data)
                        {
                            dbContext.Database.ExecuteSqlRaw(
                                insertQuery,
                                new SqlParameter("@p0", item.SicilNo),
                                new SqlParameter("@p1", item.Ad),
                                new SqlParameter("@p2", item.Soyad),
                                new SqlParameter("@p3", item.Bolum),
                                new SqlParameter("@p4", item.DagitimId),
                                new SqlParameter("@p5", item.EkleyenKisi),
                                new SqlParameter("@p6", item.EklendigiTarih),
                                new SqlParameter("@p7", item.Flos),
                                new SqlParameter("@p8", item.VerilisTarih)
                            );
                        }

                        transaction.Commit();
                    }
                }
                return Ok(new { message = "Veriler başarıyla kaydedildi." });
            }
            else
            {
                return BadRequest(new { message = "Kaydedilecek veri bulunamadı." });
            }
        }

        private IEnumerable<SdDatabase> ReadExcelData(string filePath)
        {
            var data = new List<SdDatabase>();

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    reader.Read();

                    while (reader.Read())
                    {
                        var sicilNoValue = reader.GetValue(0)?.ToString();
                        if (int.TryParse(sicilNoValue, out int sicilNo))
                        {
                            var sdDatabase = new SdDatabase();
                            sdDatabase.SicilNo = sicilNo;
                            sdDatabase.Ad = reader.GetString(1);
                            sdDatabase.Soyad = reader.GetString(2);
                            sdDatabase.Bolum = reader.GetString(3);

                            data.Add(sdDatabase);
                        }
                        else
                        {
                        }
                    }
                }
            }

            return data;
        }
        public IActionResult ExportToExcel()
        {
            List<SdDatabase> data = _context.SdDatabases.ToList();

            byte[] fileContents;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                worksheet.Cells[1, 1].Value = "SicilNo";
                worksheet.Cells[1, 2].Value = "Ad";
                worksheet.Cells[1, 3].Value = "Soyad";
                worksheet.Cells[1, 4].Value = "Bölüm";
                worksheet.Cells[1, 5].Value = "DağıtımID";
                worksheet.Cells[1, 6].Value = "EkleyenKişi";
                worksheet.Cells[1, 7].Value = "EklendiğiTarih";
                worksheet.Cells[1, 8].Value = "Flos";
                worksheet.Cells[1, 9].Value = "VerilişTarih";

                int row = 2;
                foreach (var item in data)
                {
                    worksheet.Cells[row, 1].Value = item.SicilNo;
                    worksheet.Cells[row, 2].Value = item.Ad;
                    worksheet.Cells[row, 3].Value = item.Soyad;
                    worksheet.Cells[row, 4].Value = item.Bolum;
                    worksheet.Cells[row, 5].Value = item.DagitimId;
                    worksheet.Cells[row, 6].Value = item.EkleyenKisi;
                    worksheet.Cells[row, 7].Value = item.EklendigiTarih;
                    worksheet.Cells[row, 8].Value = item.Flos;
                    worksheet.Cells[row, 9].Value = item.VerilisTarih;

                    row++;
                }

                fileContents = package.GetAsByteArray();
            }

            return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Dağıtım.xlsx");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Excel file is required.");
            }

            var fileName = "DAGITIM_WORKERS.xlsx";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var excelData = ReadExcelData(filePath);

            await SaveExcelDataToDatabase(excelData);

            System.IO.File.Delete(filePath);

            return RedirectToAction(nameof(Index));
        }

        private async Task SaveExcelDataToDatabase(IEnumerable<SdDatabase> excelData)
        {
            foreach (var item in excelData)
            {
                _context.SdDatabases.Add(item);
            }

            await _context.SaveChangesAsync();
        }
    }
}