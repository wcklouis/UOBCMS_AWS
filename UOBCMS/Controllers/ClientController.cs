using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UOBCMS.Data;
using UOBCMS.Models;
using UOBCMS.Models.dto;

namespace UOBCMS.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClientController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IBOApplicationDbContext _iboContext;

        public ClientController(IBOApplicationDbContext iboContext, ApplicationDbContext context)
        {
            _context = context;

            _iboContext = iboContext;
        }

        [HttpGet("api/clients")]
        public async Task<IActionResult> GetClients(string searchTerm = "", int pageNumber = 1, int pageSize = 100)
        {
            var query = _context.Cms_clients.AsQueryable();

            // Apply filtering 
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(b => (b.Cif.Contains(searchTerm) || b.Ename.Contains(searchTerm) || b.Cname.Contains(searchTerm)));
            }

            var totalRecords = await query.CountAsync();

            var clients = await query
                                .OrderBy(b => b.Cif)
                                .Skip((pageNumber - 1) * pageSize)
                                .Take(pageSize).ToListAsync();

            // Return the results using CustomJsonResult
            return Ok(new
            {
                TotalRecords = totalRecords,
                Clients = clients
            });
        }

        // GET: Clients
        /*
        [HttpGet("api/clients")]
        public async Task<IActionResult> GetClients()
        {
            //var clients = await _context.Cms_clients.Include(c => c.Address).Include(c => c.BankAccounts).ToListAsync();
            var clients = await _context.Cms_clients.ToListAsync();
            return Ok(clients);  // Return clients with OK status
        }
        */

        [HttpGet("clients")]
        public async Task<IActionResult> Index()
        {
            List<cms_client> clients = await _context.Cms_clients.ToListAsync();
            return View(clients); // Return the list to the view
        }

        // GET: Client Details
        [HttpGet("api/clients/{id}")]
        public async Task<IActionResult> GetClientDetails(int id)
        {
            var client = _context.Cms_clients
                .Where(c => c.Id == id)
                .Select(c => new ClientDto
                {
                    Id = c.Id,
                    Cif = c.Cif,
                    Ename = c.Ename,
                    Cname = c.Cname,
                    Status = c.Status,
                    Nature = c.Nature,
                    Category = c.Category,
                    Fullname = c.Fullname,
                    Firstname = c.Firstname,
                    Surname = c.Surname,
                    Tin = c.Tin,
                    Gender = c.Gender,
                    Nationality = c.Nationality,
                    Occupation = c.Occupation,
                    Employer = c.Employer,
                    ClientAccounts = c.Cms_client_accounts.Select(a => new ClientAccountDto
                    {
                        Client_id = a.Client_id,
                        Acc_id = a.Acc_id,
                        Account = new AccountDto
                        {
                            Id = a.Cms_account.Id,
                            AccNo = a.Cms_account.AccNo,
                            Type = a.Cms_account.Type,
                            Status = a.Cms_account.Status,
                            Phones = a.Cms_account.Cms_account_phones.Select(b => new AccountPhoneDto
                            {
                                Id = b.Id,
                                Client_phone_id = b.Client_phone_id,
                                Acc_id = b.Acc_id,
                                ClientPhone = new ClientPhoneDto
                                {
                                    Id = b.Cms_client_phone.Id,
                                    Client_id = b.Cms_client_phone.Client_id,
                                    Type = b.Cms_client_phone.Type,
                                    Country_code = b.Cms_client_phone.Country_code,
                                    Area_code = b.Cms_client_phone.Area_code,
                                    Value = b.Cms_client_phone.Value
                                }
                            }).ToList(),
                            Emails = a.Cms_account.Cms_account_emails.Select(e => new AccountEmailDto
                            {
                                Id = e.Id,
                                Client_email_id = e.Client_email_id,
                                Acc_id = e.Acc_id,
                                ClientEmail = new ClientEmailDto
                                {
                                    Id = e.Cms_client_email.Id,
                                    Client_id = e.Cms_client_email.Client_id,
                                    Type = e.Cms_client_email.Type,
                                    Email = e.Cms_client_email.Email
                                }
                            }).ToList(),
                            Addresses = a.Cms_account.Cms_account_addresses.Select(addr => new AccountAddressDto
                            {
                                Id = addr.Id,
                                Client_address_id = addr.Client_address_id,
                                Acc_id = addr.Acc_id,
                                ClientAddress = new ClientAddressDto
                                {
                                    Id = addr.Cms_client_address.Id,
                                    Client_id = addr.Cms_client_address.Client_id,
                                    Type = addr.Cms_client_address.Type,
                                    Addr1 = addr.Cms_client_address.Addr1,
                                    Addr2 = addr.Cms_client_address.Addr2,
                                    Addr3 = addr.Cms_client_address.Addr3,
                                    Addr4 = addr.Cms_client_address.Addr4,
                                    Country = addr.Cms_client_address.Country
                                }

                            }).ToList()
                        }
                    }).ToList()
                })
                .FirstOrDefault();


            if (client == null)
            {
                return NotFound(); // Return 404 if the product is not found
            }

            return Ok(client);
        }


        [HttpGet("api/clients/phone/{id}")]
        public async Task<IActionResult> GetClientPhone(int id)
        {
            var phone = _context.Cms_client_phones
                .Where(p => p.Id == id)
                .FirstOrDefault();

            if (phone == null)
            {
                return NotFound();
            }

            return Ok(phone);
        }


        [HttpGet("api/clients/email/{id}")]
        public async Task<IActionResult> GetClientEmail(int id)
        {
            var email = _context.Cms_client_emails
                .Where(p => p.Id == id)
                .FirstOrDefault();

            if (email == null)
            {
                return NotFound();
            }

            return Ok(email);
        }


        [HttpGet("api/clients/address/{id}")]
        public async Task<IActionResult> GetClientAddress(int id)
        {
            var address = _context.Cms_client_addresses
                .Where(p => p.Id == id)
                .FirstOrDefault();

            if (address == null)
            {
                return NotFound();
            }

            return Ok(address);
        }


        // GET: RFQ_FCN/Details/5
        [HttpGet("clients/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var client = await _context.Cms_clients
                .Include(c => c.Cms_client_accounts)
                    .ThenInclude(ca => ca.Cms_account) // Include the related Account entity
                        .ThenInclude(cab => cab.Cms_account_banks)
                            .ThenInclude(ccb => ccb.Cms_client_bank)
                            .ThenInclude(b => b.Cms_bank)
                .Include(c => c.Cms_client_accounts)
                    .ThenInclude(ca => ca.Cms_account) // Include the related Account entity
                        .ThenInclude(cab => cab.Cms_account_phones)
                            .ThenInclude(ccb => ccb.Cms_client_phone)
                .Include(c => c.Cms_client_accounts)
                    .ThenInclude(ca => ca.Cms_account) // Include the related Account entity
                        .ThenInclude(cab => cab.Cms_account_emails)
                            .ThenInclude(ccb => ccb.Cms_client_email)
                .Include(c => c.Cms_client_accounts)
                    .ThenInclude(ca => ca.Cms_account) // Include the related Account entity
                        .ThenInclude(cab => cab.Cms_account_addresses)
                            .ThenInclude(ccb => ccb.Cms_client_address)
                .Include(c => c.Cms_client_additionalinfos)
                    .ThenInclude(ca => ca.Cms_additionalinfo)
                .Include(c => c.Cms_client_related_staffs)
                //.Include(c => c.Cms_client_accounts)
                //    .ThenInclude(ca => ca.Cms_account) // Include the related Account entity
                //        .ThenInclude(cab => cab.Cms_account_mthsaving_plans)
                .Include(c => c.Cms_child_clients)
                    .ThenInclude(ca => ca.Cms_child_client)
                .Include(c => c.Cms_virtual_clients)
                    .ThenInclude(ca => ca.Cms_parent_client)
                        .ThenInclude(c => c.Cms_client_accounts)
                            .ThenInclude(c => c.Cms_account)
                .Include(c => c.Cms_client_ids)
                .Include(c => c.Cms_client_north_bound)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (client == null)
            {
                return NotFound(); // Return 404 if the product is not found
            }

            client.Cms_client_phones = client.Cms_client_phones
               .OrderBy(cp => cp.Type) // Adjust the property as needed
               .ToList();

            /*foreach (cms_client_account client_account in address.Cms_client_accounts)
            {
                foreach (cms_account_mthsaving_plan mthsaving_plan in client_account.Cms_account.Cms_account_mthsaving_plans)
                {
                    mthsaving_plan.Inst = getInstrument(mthsaving_plan.Mkt_code, mthsaving_plan.Sec_code);
                }
            }*/

            return View(client); // Return the detailed view of the address
        }

        /*public Instrument getInstrument(string mktcode, string seccode)
        {
            Instrument Inst = _iboContext.Instruments.FirstOrDefault(c => (c.Market == mktcode && c.Instr == seccode));
            return Inst;
        }*/


        #region Submit updated information

        // Post update records
        [HttpPost("api/clients/submit-update")]
        public async Task<IActionResult> SubmitUpdateRequest([FromBody] update_cms_client request)
        {
            if (request == null || request.client_id <= 0)
            {
                return BadRequest(new { success = false, message = "Invalid request data." });
            }

            request.submitted_dt = DateTime.Now;
            request.approval_status = "P";

            _context.Update_cms_clients.Add(request);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Update request submitted successfully." });
        }

        [HttpPost("api/clients/submit-updated-phone")]
        public async Task<IActionResult> SubmitUpdatedPhone([FromBody] update_cms_client_phone request)
        {
            if (request == null || request.Client_id <= 0)
            {
                return BadRequest(new { success = false, message = "Invalid request data." });
            }

            request.submitted_dt = DateTime.Now;
            request.approval_status = "P";

            _context.Update_cms_client_phones.Add(request);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Update phone submitted successfully." });
        }


        [HttpPost("api/clients/submit-updated-email")]
        public async Task<IActionResult> SubmitUpdatedEmail([FromBody] update_cms_client_email request)
        {
            if (request == null || request.Client_id <= 0)
            {
                return BadRequest(new { success = false, message = "Invalid request data." });
            }

            request.submitted_dt = DateTime.Now;
            request.approval_status = "P";

            _context.Update_cms_client_emails.Add(request);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Update email submitted successfully." });
        }

        [HttpPost("api/clients/submit-updated-address")]
        public async Task<IActionResult> SubmitUpdatedAddress([FromBody] update_cms_client_address request)
        {
            if (request == null || request.Client_id <= 0)
            {
                return BadRequest(new { success = false, message = "Invalid request data." });
            }

            request.submitted_dt = DateTime.Now;
            request.approval_status = "P";

            _context.Update_cms_client_addresses.Add(request);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Update address submitted successfully." });
        }

        #endregion



        #region Get all updated records or detail by id

        // Get all the update records
        [HttpGet("api/clients/get-all-updates")]
        public async Task<IActionResult> GetAllUpdateRequests(string searchTerm = "", int pageNumber = 1, int pageSize = 30)
        {
            var query = _context.Update_cms_clients.AsQueryable();

            // Apply filtering 
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(b => (b.Cif.Contains(searchTerm) || b.Ename.Contains(searchTerm) || b.Cname.Contains(searchTerm)));
            }

            var totalRecords = await query.CountAsync();

            var updates = await query
                                .OrderBy(b => b.Id)
                                .Skip((pageNumber - 1) * pageSize)
                                .Take(pageSize).ToListAsync();

            // Return the results using CustomJsonResult
            return Ok(new
            {
                TotalRecords = totalRecords,
                Updates = updates
            });
        }

        // Get the updated record detail by id
        [HttpGet("api/clients/getupdatedetail/{id}")]
        public async Task<IActionResult> GetUpdatedetail(int id)
        {
            var updateRecord = await _context.Update_cms_clients.FirstOrDefaultAsync(c => c.Id == id);

            if (updateRecord == null)
            {
                return NotFound(new { success = false, message = "Client not found." }); // Return 404 if the product is not found
            }

            return Ok(updateRecord);
        }

        // Get all the updated address records
        [HttpGet("api/clients/get-all-updatedphones")]
        public async Task<IActionResult> GetAllUpdatedPhones(string searchTerm = "", int pageNumber = 1, int pageSize = 30)
        {
            var query = _context.Update_cms_client_phones.AsQueryable();

            // Apply filtering 
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(b => (b.TypeString.Contains(searchTerm) || b.Country_code.Contains(searchTerm) || b.Value.Contains(searchTerm)));
            }

            var totalRecords = await query.CountAsync();

            var updates = await query
                                .OrderBy(b => b.Id)
                                .Skip((pageNumber - 1) * pageSize)
                                .Take(pageSize).ToListAsync();

            // Return the results using CustomJsonResult
            return Ok(new
            {
                TotalRecords = totalRecords,
                Phones = updates
            });
        }

        // Get the updated address record detail by id
        [HttpGet("api/clients/get-updatedphone-detail/{id}")]
        public async Task<IActionResult> GetUpdatedPhoneDetail(int id)
        {
            var updateRecord = await _context.Update_cms_client_phones.FirstOrDefaultAsync(c => c.Id == id);

            if (updateRecord == null)
            {
                return NotFound(new { success = false, message = "Client not found." }); // Return 404 if the product is not found
            }

            return Ok(updateRecord);
        }

        // Get all the updated address records
        [HttpGet("api/clients/get-all-updatedemails")]
        public async Task<IActionResult> GetAllUpdatedEmails(string searchTerm = "", int pageNumber = 1, int pageSize = 30)
        {
            var query = _context.Update_cms_client_emails.AsQueryable();

            // Apply filtering 
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(b => (b.TypeString.Contains(searchTerm) || b.Email.Contains(searchTerm)));
            }

            var totalRecords = await query.CountAsync();

            var updates = await query
                                .OrderBy(b => b.Id)
                                .Skip((pageNumber - 1) * pageSize)
                                .Take(pageSize).ToListAsync();

            // Return the results using CustomJsonResult
            return Ok(new
            {
                TotalRecords = totalRecords,
                Emails = updates
            });
        }

        // Get the updated address record detail by id
        [HttpGet("api/clients/get-updatedemail-detail/{id}")]
        public async Task<IActionResult> GetUpdatedEmailDetail(int id)
        {
            var updateRecord = await _context.Update_cms_client_emails.FirstOrDefaultAsync(c => c.Id == id);

            if (updateRecord == null)
            {
                return NotFound(new { success = false, message = "Client not found." }); // Return 404 if the product is not found
            }

            return Ok(updateRecord);
        }


        // Get all the updated address records
        [HttpGet("api/clients/get-all-updatedaddresses")]
        public async Task<IActionResult> GetAllUpdatedAddresses(string searchTerm = "", int pageNumber = 1, int pageSize = 30)
        {
            var query = _context.Update_cms_client_addresses.AsQueryable();

            // Apply filtering 
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(b => (b.TypeString.Contains(searchTerm) || b.Addr1.Contains(searchTerm)));
            }

            var totalRecords = await query.CountAsync();

            var updates = await query
                                .OrderBy(b => b.Id)
                                .Skip((pageNumber - 1) * pageSize)
                                .Take(pageSize).ToListAsync();

            // Return the results using CustomJsonResult
            return Ok(new
            {
                TotalRecords = totalRecords,
                Addresses = updates
            });
        }

        // Get the updated address record detail by id
        [HttpGet("api/clients/get-updatedaddress-detail/{id}")]
        public async Task<IActionResult> GetUpdatedAddressDetail(int id)
        {
            var updateRecord = await _context.Update_cms_client_addresses.FirstOrDefaultAsync(c => c.Id == id);

            if (updateRecord == null)
            {
                return NotFound(new { success = false, message = "Client not found." }); // Return 404 if the product is not found
            }

            return Ok(updateRecord);
        }
        #endregion



        #region Approve updated records

        // Approve update records
        [HttpPost("api/clients/approve-update/{id}")]
        public async Task<IActionResult> ApproveUpdateRequest(int id, [FromQuery] string approvalStatus)
        {
            var updateRequest = await _context.Update_cms_clients.FirstOrDefaultAsync(r => r.Id == id);

            if (updateRequest == null)
            {
                return NotFound(new { success = false, message = "Update request not found." });
            }

            if (approvalStatus != "A" && approvalStatus != "R")
            {
                return BadRequest(new { success = false, message = "Invalid approval status." });
            }

            updateRequest.approval_status = approvalStatus;
            updateRequest.approved_by = "Leader"; // Replace with actual approver's name
            updateRequest.approved_dt = DateTime.Now;

            if (approvalStatus == "A")
            {
                // 更新 cms_client 表
                var client = await _context.Cms_clients.FirstOrDefaultAsync(c => c.Id == updateRequest.client_id);
                if (client != null)
                {
                    client.Cif = updateRequest.Cif;
                    client.Ename = updateRequest.Ename;
                    client.Cname = updateRequest.Cname;
                    client.Status = updateRequest.Status;
                    client.Nature = updateRequest.Nature;
                    client.Category = updateRequest.Category;
                    client.Gender = updateRequest.Gender;
                    client.Occupation = updateRequest.Occupation;
                    client.Employer = updateRequest.Employer;
                    client.Lastupdateuserid = updateRequest.approved_by;
                    client.Lastupdatedatetime = DateTime.Now;

                    _context.Cms_clients.Update(client);
                }
            }

            //_context.Cms_client_update_requests.Add(updateRequest);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = $"Update request {approvalStatus.ToLower()}." });
        }


        // Approve update records
        [HttpPost("api/clients/approve-updated-phone/{id}")]
        public async Task<IActionResult> ApproveUpdatedPhone(int id, [FromQuery] string approvalStatus)
        {
            var updateRequest = await _context.Update_cms_client_phones.FirstOrDefaultAsync(r => r.Id == id);

            if (updateRequest == null)
            {
                return NotFound(new { success = false, message = "Update request not found." });
            }

            if (approvalStatus != "A" && approvalStatus != "R")
            {
                return BadRequest(new { success = false, message = "Invalid approval status." });
            }

            updateRequest.approval_status = approvalStatus;
            updateRequest.approved_by = "Leader"; // Replace with actual approver's name
            updateRequest.approved_dt = DateTime.Now;

            if (approvalStatus == "A")
            {
                // 更新 cms_client_phones 表
                var phone = await _context.Cms_client_phones.FirstOrDefaultAsync(c => c.Id == updateRequest.client_phone_id);
                if (phone != null)
                {
                    phone.Country_code = updateRequest.Country_code;
                    phone.Area_code = updateRequest.Area_code;
                    phone.Value = updateRequest.Value;

                    _context.Cms_client_phones.Update(phone);
                }
            }

            //_context.Cms_client_update_requests.Add(updateRequest);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = $"Update request {approvalStatus.ToLower()}." });
        }

        // Approve update records
        [HttpPost("api/clients/approve-updated-email/{id}")]
        public async Task<IActionResult> ApproveUpdatedEmail(int id, [FromQuery] string approvalStatus)
        {
            var updateRequest = await _context.Update_cms_client_emails.FirstOrDefaultAsync(r => r.Id == id);

            if (updateRequest == null)
            {
                return NotFound(new { success = false, message = "Update request not found." });
            }

            if (approvalStatus != "A" && approvalStatus != "R")
            {
                return BadRequest(new { success = false, message = "Invalid approval status." });
            }

            updateRequest.approval_status = approvalStatus;
            updateRequest.approved_by = "Leader"; // Replace with actual approver's name
            updateRequest.approved_dt = DateTime.Now;

            if (approvalStatus == "A")
            {
                // 更新 cms_client_emails 表
                var email = await _context.Cms_client_emails.FirstOrDefaultAsync(c => c.Id == updateRequest.client_email_id);
                if (email != null)
                {
                    email.Type = updateRequest.Type;
                    email.Email = updateRequest.Email;

                    _context.Cms_client_emails.Update(email);
                }
            }

            //_context.Cms_client_update_requests.Add(updateRequest);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = $"Update request {approvalStatus.ToLower()}." });
        }


        // Approve update records
        [HttpPost("api/clients/approve-updated-address/{id}")]
        public async Task<IActionResult> ApproveUpdatedAddress(int id, [FromQuery] string approvalStatus)
        {
            var updateRequest = await _context.Update_cms_client_addresses.FirstOrDefaultAsync(r => r.Id == id);

            if (updateRequest == null)
            {
                return NotFound(new { success = false, message = "Update request not found." });
            }

            if (approvalStatus != "A" && approvalStatus != "R")
            {
                return BadRequest(new { success = false, message = "Invalid approval status." });
            }

            updateRequest.approval_status = approvalStatus;
            updateRequest.approved_by = "Leader"; // Replace with actual approver's name
            updateRequest.approved_dt = DateTime.Now;

            if (approvalStatus == "A")
            {
                // 更新 cms_client_emails 表
                var address = await _context.Cms_client_addresses.FirstOrDefaultAsync(c => c.Id == updateRequest.client_address_id);
                if (address != null)
                {
                    address.Type = updateRequest.Type;
                    address.Addr1 = updateRequest.Addr1;
                    address.Addr2 = updateRequest.Addr2;
                    address.Addr3 = updateRequest.Addr3;
                    address.Addr4 = updateRequest.Addr4;
                    address.District = updateRequest.District;
                    address.City = updateRequest.City;
                    address.Province = updateRequest.Province;
                    address.Country = updateRequest.Country;

                    _context.Cms_client_addresses.Update(address);
                }
            }

            //_context.Cms_client_update_requests.Add(updateRequest);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = $"Update request {approvalStatus.ToLower()}." });
        }

        #endregion

    }
}
