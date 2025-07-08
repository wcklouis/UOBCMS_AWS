using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UOBCMS.Data;
using UOBCMS.Models;
using UOBCMS.Models.api;

namespace UOBCMS.Controllers
{
    [Route("ClientBank")]
    public class ClientBankController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClientBankController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Bank/Edit/5
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            // Retrieve the bank account details using the id
            var bankAccount = await _context.Cms_client_banks
                .Include(cab => cab.Cms_account_banks)
                .Include(c => c.Cms_bank)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (bankAccount == null)
            {
                return NotFound(); // Return a 404 if not found
            }

            // Example list of payment types
            var paymentTypes = GetPaymentTypes();

            body_cms_client_bank clientbank = new body_cms_client_bank();
            clientbank.Id = bankAccount.Id;
            clientbank.Bank_type = bankAccount.Cms_bank.Type;
            clientbank.Bank_name = bankAccount.Cms_bank.Bank_name;
            clientbank.Bank_code = bankAccount.Cms_bank.Bank_code;
            clientbank.Ccy = bankAccount.Ccy;
            clientbank.Bank_accno = bankAccount.Bank_accno;
            clientbank.DefaultLocalHKDBank = bankAccount.Cms_account_banks.FirstOrDefault().DefaultLocalHKDBank;

            clientbank.Bene_bank_swift = bankAccount.Bene_bank_swift;
            clientbank.Bene_bank_address = bankAccount.Bene_bank_address;
            clientbank.Bene_bank_accname = bankAccount.Bene_bank_accname;

            clientbank.Bene_bank_accno = bankAccount.Bank_accno;
            clientbank.Bene_bank_name = bankAccount.Bene_bank_name;

            clientbank.Bene_ccy = bankAccount.Ccy;
            clientbank.Corr_bank_name = bankAccount.Corr_bank_name;
            clientbank.Corr_bank_swift = bankAccount.Corr_bank_swift;

            clientbank.Status = bankAccount.Status;

            clientbank.Default_payment_type = bankAccount.Default_payment_type;

            clientbank.Bank_accname = bankAccount.Bank_accname;
            clientbank.Other_bank_details = bankAccount.Other_bank_details;
            clientbank.Lastupdateuserid = bankAccount.Lastupdateuserid;
            clientbank.Lastupdatedatetime = bankAccount.Lastupdatedatetime;
            clientbank.Version = bankAccount.Version;
            clientbank.Dbopr = bankAccount.Dbopr;

            ViewBag.PaymentTypes = paymentTypes; // Pass the list to the view

            return View(clientbank); // Return the bank account details
        }

        private IEnumerable<SelectListItem> GetPaymentTypes()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Value = "0", Text = "E-Payment" },
                new SelectListItem { Value = "1", Text = "FPS" }
            };
        }

        /*[HttpPost("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(body_cms_client_bank model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Update(model);
                    await _context.SaveChangesAsync(); // Use asynchronous saving

                    // Redirect to the Index or another appropriate action after success
                    return RedirectToAction("Index");
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                // Handle concurrency issues
                ModelState.AddModelError("", "Unable to save changes. The record was modified by another user.");
            }
            catch (Exception ex)
            {
                // Log the exception (consider using a logging framework)
                Console.WriteLine(ex.Message);
                ModelState.AddModelError("", "An error occurred while updating the record. Please try again.");
            }

            // If we got this far, something failed; redisplay the form
            ViewBag.PaymentTypes = GetPaymentTypes(); // Populate the payment types again
            return View(model);
        }*/

        [HttpPost("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(body_cms_client_bank model)
        {
            if (!ModelState.IsValid)
            {
                return View(model); // Return to the view if validation fails
            }

            // Update logic here
            var existingRecord = _context.Cms_client_banks.Find(model.Id);
            if (existingRecord == null)
            {
                return NotFound(); // Return a 404 if not found
            }

            //existingRecord.Name = model.Name; // Update the necessary fields
            //existingRecord.AccountNumber = model.AccountNumber;

            _context.SaveChanges();
            return RedirectToAction("Index"); // Redirect after successful update
        }

        private bool BankAccountExists(int id)
        {
            return _context.Cms_client_banks.Any(e => e.Id == id);
        }

        // GET: ClientBank
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var bankAccounts = await _context.Cms_client_banks.ToListAsync();
            return View(bankAccounts);
        }
    }
}