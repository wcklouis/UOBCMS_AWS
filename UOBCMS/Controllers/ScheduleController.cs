using Microsoft.AspNetCore.Mvc;
using UOBCMS.Data;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Microsoft.EntityFrameworkCore;
using UOBCMS.Classes;
using DevExpress.Data.Filtering;
using UOBCMS.Models;
using UOBCMS.Models.eform;

namespace UOBCMS.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ScheduleController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly ApplicationDbContext _context;
        private readonly EFormApplicationDbContext _eformcontext;

        public ScheduleController(HttpClient httpClient, ApplicationDbContext context, EFormApplicationDbContext eformcontext)
        {
            _httpClient = httpClient;
            _context = context;
            _eformcontext = eformcontext;
        }

        [HttpGet("SyncAEFromEReport")]
        public async Task<IActionResult> SyncAEFromEReport()
        {
            string className = GetType().Name;
            string methodName = "GetAEbyAccno";            

            var url = "http://uhks0064/intranet/ClntShare";
            var filePath = @"c:\temp\download_file.xlsx";

            try
            {
                // Check if the file already exists and delete it if it does
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                // Send a GET request to the API to get the Excel file
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    // Read the content as a byte array
                    var fileBytes = await response.Content.ReadAsByteArrayAsync();

                    /*
                    var fileName = "downloaded_file.xlsx"; // Set the file name

                    // Return the file as a FileResult
                    return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                    */

                    // Save the byte array to the specified file path
                    System.IO.File.WriteAllBytes(filePath, fileBytes);

                    var branch = await _context.Cms_branches
                                        .FirstOrDefaultAsync(c => c.Code == "MAIN");

                    if (branch == null)
                    {
                        Logger.LogErrorMessage(className, methodName, "", $"MAIN branch is not found.", Logger.ERROR);
                        return null;
                    }

                    Models.eform.eformUserGroup m_eformUserGroup = await _eformcontext.EFormUserGroups
                                    .FirstOrDefaultAsync(c => c.Usergpname == "AE");

                    if (m_eformUserGroup == null)
                    {
                        Logger.LogErrorMessage(className, methodName, "", $"AE user group is not found.", Logger.ERROR);
                        return null;
                    }

                    // Load the Excel file
                    using (var file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        IWorkbook workbook = new XSSFWorkbook(file);
                        ISheet sheet = workbook.GetSheetAt(0); // Get the first sheet

                        int aeCode_col = 0;
                        int aeName_col = 0;

                        for (int row = 0; row <= sheet.LastRowNum; row++)
                        {
                            string aeCode = "";
                            string aeName = "";
                            IRow currentRow = sheet.GetRow(row);
                            for (int col = 0; col < currentRow.LastCellNum; col++)
                            {
                                var cellValue = currentRow.GetCell(col)?.ToString();

                                if (row == 0)
                                {
                                    if (cellValue == "AE_Code")
                                        aeCode_col = col;

                                    if (cellValue == "AE_Name")
                                        aeName_col = col;
                                }
                                else
                                {
                                    if (col == aeCode_col)
                                        aeCode = cellValue;
                                    else if (col == aeName_col)
                                        aeName = cellValue;
                                }

                                Console.Write($"{cellValue}\t");
                            }

                            Console.Write("\n");

                            if (aeCode != "" && aeCode != null)
                            {
                                // Sync with CMS
                                var ae = await _context.Cms_aes
                                    .FirstOrDefaultAsync(c => c.Code == aeCode);

                                if (ae == null)
                                {
                                    cms_ae l_ae = new cms_ae();

                                    // Set the Lastupdatedatetime to the current time
                                    l_ae.Code = aeCode;
                                    l_ae.Name = aeName == null ? "" : aeName;
                                    l_ae.Branch_id = branch.Id;
                                    l_ae.Lastupdatedatetime = DateTime.UtcNow;
                                    l_ae.Lastupdateuserid = "sys";
                                    l_ae.Version = 1;
                                    l_ae.Dbopr = "I";

                                    _context.Cms_aes.Add(l_ae);

                                    // Save changes to the database
                                    await _context.SaveChangesAsync();

                                    Console.Write($"Create AE: {aeCode},{aeName} in CMS");
                                    Logger.LogErrorMessage(className, methodName, "", $"Create AE: {aeCode},{aeName} in CMS", Logger.INFO);
                                }
                                else
                                {
                                    if (ae.Name != aeName)
                                    {
                                        ae.Name = aeName == null ? "" : aeName;
                                        ae.Lastupdatedatetime = DateTime.UtcNow;
                                        ae.Lastupdateuserid = "sys";
                                        ae.Version = 1;
                                        ae.Dbopr = "U";

                                        _context.Cms_aes.Update(ae);

                                        // Save changes to the database
                                        await _context.SaveChangesAsync();

                                        Console.Write($"Update AE: {aeCode},{aeName} in CMS");
                                        Logger.LogErrorMessage(className, methodName, "", $"Update AE: {aeCode},{aeName} in CMS", Logger.INFO);
                                    }
                                }

                                // Sync with EForm User
                                int userid = -1;
                                var m_eformUser = await _eformcontext.EFormUsers
                                    .FirstOrDefaultAsync(c => c.Username == aeCode);

                                if (m_eformUser == null)
                                {
                                    Models.eform.eformUser l_eformUser = new Models.eform.eformUser();

                                    // Set the Lastupdatedatetime to the current time
                                    l_eformUser.Username = aeCode;
                                    l_eformUser.Lastupdatedatetime = DateTime.UtcNow;
                                    l_eformUser.Lastupdateuserid = "sys";
                                    l_eformUser.Version = 1;
                                    l_eformUser.Dbopr = "I";

                                    _eformcontext.EFormUsers.Add(l_eformUser);

                                    // Save changes to the database
                                    await _eformcontext.SaveChangesAsync();

                                    userid = l_eformUser.Id;

                                    Console.Write($"Create AE: {aeCode},{aeName} in EForm Portal");
                                    Logger.LogErrorMessage(className, methodName, "", $"Create AE: {aeCode},{aeName} in EForm Portal", Logger.INFO);
                                }
                                else
                                {
                                    userid = m_eformUser.Id;
                                }

                                // Sync with EForm User In Group
                                var m_eformUserInGroup = await _eformcontext.EFormUserInGroups
                                    .FirstOrDefaultAsync(c => c.Userid == userid);

                                if (m_eformUserInGroup == null)
                                {
                                    Models.eform.eformUserInGroup l_eformUserInGroup = new Models.eform.eformUserInGroup();

                                    l_eformUserInGroup.Usergpid = m_eformUserGroup.Id;
                                    l_eformUserInGroup.Userid = userid;
                                    l_eformUserInGroup.Lastupdatedatetime = DateTime.UtcNow;
                                    l_eformUserInGroup.Lastupdateuserid = "sys";
                                    l_eformUserInGroup.Version = 1;
                                    l_eformUserInGroup.Dbopr = "I";

                                    _eformcontext.EFormUserInGroups.Add(l_eformUserInGroup);

                                    // Save changes to the database
                                    await _eformcontext.SaveChangesAsync();

                                    Console.Write($"Create User In Group: {aeCode},{aeName} in EForm Portal");
                                    Logger.LogErrorMessage(className, methodName, "", $"Create Create User In Group: {aeCode},{aeName} in EForm Portal", Logger.INFO);
                                }
                            }
                            Console.WriteLine();
                        }
                    }

                    return Ok("File saved successfully.");
                }
                else
                {
                    return StatusCode((int)response.StatusCode, "Error downloading file.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
