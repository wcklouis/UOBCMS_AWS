using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UOBCMS.Models;

namespace UOBCMS.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            this.Database.SetCommandTimeout(120); // Set timeout to 120 seconds
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Define keyless entity type
            modelBuilder.Entity<SelectListGroup>(eb =>
            {
                eb.HasNoKey();
                // If you have specific properties, configure them here                
            });

            modelBuilder.Entity<cms_client>()
                .ToTable("cms_clients", "dbo");

            modelBuilder.Entity<cms_account>()
                .ToTable("cms_accounts", "dbo");

            modelBuilder.Entity<cms_client_account>()
                .ToTable("cms_client_accounts", "dbo");

            modelBuilder.Entity<cms_addr_comp_opt>()
                .ToTable("cms_addr_comp_opts", "dbo");

            modelBuilder.Entity<cms_client_address>()
                .ToTable("cms_client_addresses", "dbo");

            modelBuilder.Entity<cms_account_address>()
                .ToTable("cms_account_addresses", "dbo");

            modelBuilder.Entity<cms_client_email>()
                .ToTable("cms_client_emails", "dbo");

            modelBuilder.Entity<cms_account_email>()
                .ToTable("cms_account_emails", "dbo");

            modelBuilder.Entity<cms_account_north_bound>()
                .ToTable("cms_account_north_bounds", "dbo");

            modelBuilder.Entity<cms_account_company_structure>()
                .ToTable("cms_account_company_structures", "dbo");

            modelBuilder.Entity<cms_client_phone>()
                .ToTable("cms_client_phones", "dbo");

            modelBuilder.Entity<cms_client_id>()
                .ToTable("cms_client_ids", "dbo");

            modelBuilder.Entity<cms_client_north_bound>()
                .ToTable("cms_client_north_bounds", "dbo");

            modelBuilder.Entity<cms_account_doc>()
                .ToTable("cms_account_docs", "dbo");

            modelBuilder.Entity<cms_account_doc>()
                .ToTable("cms_account_docs", "dbo");

            modelBuilder.Entity<cms_account_phone>()
                .ToTable("cms_account_phones", "dbo");

            modelBuilder.Entity<cms_account_company_structure_director>()
                .ToTable("cms_account_company_structure_directors", "dbo");

            modelBuilder.Entity<cms_account_company_structure_shareholder>()
                .ToTable("cms_account_company_structure_shareholders", "dbo");

            modelBuilder.Entity<cms_account_company_structure_intermediary>()
                .ToTable("cms_account_company_structure_intermediaries", "dbo");
            
            modelBuilder.Entity<cms_bank>()
                .ToTable("cms_banks", "dbo");

            modelBuilder.Entity<cms_client_bank>()
                .ToTable("cms_client_banks", "dbo");

            modelBuilder.Entity<cms_account_bank>()
                .ToTable("cms_account_banks", "dbo");

            modelBuilder.Entity<cms_account_controlling_person>()
                .ToTable("cms_account_controlling_persons", "dbo");

            modelBuilder.Entity<cms_account_auth_party>()
                .ToTable("cms_account_auth_parties", "dbo");

            modelBuilder.Entity<cms_account_dkq>()
                .ToTable("cms_account_dkqs", "dbo");

            modelBuilder.Entity<cms_account_w8>()
                .ToTable("cms_account_w8s", "dbo");

            modelBuilder.Entity<cms_ae>()
                .ToTable("cms_aes", "dbo");

            modelBuilder.Entity<cms_branch>()
                .ToTable("cms_branches", "dbo");

            modelBuilder.Entity<cms_account_hkidr>()
                .ToTable("cms_account_hkidrs", "dbo");

            modelBuilder.Entity<cms_account_mthsaving_plan>()
                .ToTable("cms_account_mthsaving_plans", "dbo");

            modelBuilder.Entity<cms_account_additionalinfo>()
                .ToTable("cms_account_additionalinfos", "dbo");

            modelBuilder.Entity<cms_account_limit>()
                .ToTable("cms_account_limits", "dbo");

            modelBuilder.Entity<cms_account_market>()
                .ToTable("cms_account_markets", "dbo");

            modelBuilder.Entity<cms_account_market_limit>()
                .ToTable("cms_account_market_limits", "dbo");

            modelBuilder.Entity<cms_account_market_cash_si>()
                .ToTable("cms_account_market_cash_sis", "dbo");

            modelBuilder.Entity<cms_account_market_inst_si>()
                .ToTable("cms_account_market_inst_sis", "dbo");

            modelBuilder.Entity<cms_account_market_confo>()
                .ToTable("cms_account_market_confos", "dbo");

            modelBuilder.Entity<cms_account_market_inscat>()
                .ToTable("cms_account_market_inscats", "dbo");

            modelBuilder.Entity<cms_account_market_interest>()
                .ToTable("cms_account_market_interests", "dbo");

            modelBuilder.Entity<cms_account_market_interest_detail>()
                .ToTable("cms_account_market_interest_details", "dbo");

            modelBuilder.Entity<cms_account_market_brokerage>()
                .ToTable("cms_account_market_brokerages", "dbo");

            modelBuilder.Entity<cms_account_market_brokerage_detail>()
                .ToTable("cms_account_market_brokerage_details", "dbo");

            modelBuilder.Entity<cms_account_market_price_cap>()
                .ToTable("cms_account_market_price_caps", "dbo");

            modelBuilder.Entity<cms_account_market_rebate>()
                .ToTable("cms_account_market_rebates", "dbo");

            modelBuilder.Entity<cms_additionalinfo>()
                .ToTable("cms_additionalinfos", "dbo");

            modelBuilder.Entity<cms_client_additionalinfo>()
                .ToTable("cms_client_additionalinfos", "dbo");

            modelBuilder.Entity<cms_client_related_staff>()
                .ToTable("cms_client_related_staffs", "dbo");

            modelBuilder.Entity<cms_client_north_bound>()
                .ToTable("cms_client_north_bounds", "dbo");

            modelBuilder.Entity<cms_account_social_media>()
                .ToTable("cms_account_social_medias", "dbo");

            modelBuilder.Entity<update_cms_client>()
                .ToTable("update_cms_clients", "dbo");

            modelBuilder.Entity<update_cms_client_phone>()
                .ToTable("update_cms_client_phones", "dbo");

            modelBuilder.Entity<update_cms_client_email>()
                .ToTable("update_cms_client_emails", "dbo");

            modelBuilder.Entity<update_cms_client_address>()
                .ToTable("update_cms_client_addresses", "dbo");            

            /*modelBuilder
               .Entity<cms_bank>(eb =>
               {
                   eb.HasNoKey(); // Specify that this is a keyless entity type
                                  // Optionally, you can specify the table or view it maps to
                                  // eb.ToTable("YourTableName");
                                  // Or if it's a view:
                                  // eb.ToView("YourViewName");
               });*/

            /*modelBuilder.Entity<cms_acc_address>()
                .ToTable("cms_account_addresses", "dbo");*/

            // Configuring the composite primary key
            modelBuilder.Entity<cms_client_account>()
                .HasKey(ca => new { ca.Client_id, ca.Acc_id });

            // Configuring many-to-many relationship            
            modelBuilder.Entity<cms_client_account>()
                .HasOne(ca => ca.Cms_client)
                .WithMany(c => c.Cms_client_accounts)
                .HasForeignKey(ca => ca.Client_id);

            modelBuilder.Entity<cms_client_account>()
                .HasOne(ca => ca.Cms_account)
                .WithOne(a => a.Cms_client_account)
                .HasForeignKey<cms_client_account>(ca => ca.Acc_id);

            /*modelBuilder.Entity<cms_client>()
                .HasMany(c => c.Cms_client_addresses) // One client has many addresses
                .WithOne(a => a.Cms_client)            // Each address belongs to one client
                .HasForeignKey(a => a.Client_id);       // Foreign key in cms_client_address */

            // Configure one-to-many relationship between cms_client_address and cms_account_address
            /*modelBuilder.Entity<cms_client_address>()
                .HasMany(c => c.Cms_account_addresses)  // One cms_client_address has many cms_account_address
                .WithOne(a => a.Cms_client_address)      // Each cms_account_address belongs to one cms_client_address
                .HasForeignKey(a => a.Client_address_id); // Foreign key in cms_account_address*/

            modelBuilder.Entity<cms_account>()
                .HasMany(c => c.Cms_account_additionalinfos)
                .WithOne(a => a.Cms_account)
                .HasForeignKey(a => a.Acc_id);

            modelBuilder.Entity<cms_account>()
                .HasMany(c => c.Cms_account_limits)
                .WithOne(a => a.Cms_account)
                .HasForeignKey(a => a.Acc_id);

            modelBuilder.Entity<cms_account>()
                .HasMany(c => c.Cms_account_markets)
                .WithOne(a => a.Cms_account)
                .HasForeignKey(a => a.Acc_id);

            modelBuilder.Entity<cms_account_market>()
                .HasMany(c => c.Cms_account_market_limits)
                .WithOne(a => a.Cms_account_market)
                .HasForeignKey(a => a.Account_mkt_id);

            modelBuilder.Entity<cms_account_market>()
                .HasMany(c => c.Cms_account_market_cash_sis)
                .WithOne(a => a.Cms_account_market)
                .HasForeignKey(a => a.Account_mkt_id);

            modelBuilder.Entity<cms_account_market>()
                .HasMany(c => c.Cms_account_market_inst_sis)
                .WithOne(a => a.Cms_account_market)
                .HasForeignKey(a => a.Account_mkt_id);

            modelBuilder.Entity<cms_account_market>()
                .HasMany(c => c.Cms_account_market_confos)
                .WithOne(a => a.Cms_account_market)
                .HasForeignKey(a => a.Account_mkt_id);

            modelBuilder.Entity<cms_account_market>()
                .HasMany(c => c.Cms_account_market_inscats)
                .WithOne(a => a.Cms_account_market)
                .HasForeignKey(a => a.Account_mkt_id);

            modelBuilder.Entity<cms_account_market>()
                .HasMany(c => c.Cms_account_market_interests)
                .WithOne(a => a.Cms_account_market)
                .HasForeignKey(a => a.Account_mkt_id);

            modelBuilder.Entity<cms_account_market_interest>()
                .HasMany(c => c.Cms_account_market_interest_details)
                .WithOne(a => a.Cms_account_market_interest)
                .HasForeignKey(a => a.Account_mkt_ccy_id);

            modelBuilder.Entity<cms_account_market>()
                .HasMany(c => c.Cms_account_market_brokerages)
                .WithOne(a => a.Cms_account_market)
                .HasForeignKey(a => a.Account_mkt_id);

            modelBuilder.Entity<cms_account_market_brokerage>()
                .HasMany(c => c.Cms_account_market_brokerage_details)
                .WithOne(a => a.Cms_account_market_brokerage)
                .HasForeignKey(a => a.Account_mkt_brokerage_id);

            modelBuilder.Entity<cms_account_market>()
                .HasMany(c => c.Cms_account_market_rebates)
                .WithOne(a => a.Cms_account_market)
                .HasForeignKey(a => a.Account_mkt_id);

            modelBuilder.Entity<cms_account_market>()
                .HasMany(c => c.Cms_account_market_price_caps)
                .WithOne(a => a.Cms_account_market)
                .HasForeignKey(a => a.Account_mkt_id);

            modelBuilder.Entity<cms_account_bank>()
               .HasOne(c => c.Cms_account_market)
               .WithOne(w => w.Cms_account_bank)
               .HasForeignKey<cms_account_market>(w => w.Account_bank_id);

            modelBuilder.Entity<cms_account>()
                .HasMany(c => c.Cms_account_addresses)  // One account has many addresses
                .WithOne(a => a.Cms_account)              // Each address belongs to one account
                .HasForeignKey(a => a.Acc_id);             // Foreign key in cms_account_address

            modelBuilder.Entity<cms_account>()
                .HasMany(c => c.Cms_account_social_medias)
                .WithOne(a => a.Cms_account)              
                .HasForeignKey(a => a.Acc_id);

            modelBuilder.Entity<cms_client>()
                .HasMany(c => c.Cms_client_addresses)      // One client has many banks
                .WithOne(a => a.Cms_client)              // Each bank belongs to one client
                .HasForeignKey(a => a.Client_id);

            modelBuilder.Entity<cms_client>()
                .HasMany(c => c.Cms_client_related_staffs)
                .WithOne(a => a.Cms_client)
                .HasForeignKey(a => a.Client_id);

            modelBuilder.Entity<cms_client_address>()
                .HasMany(c => c.Cms_account_addresses)
                .WithOne(a => a.Cms_client_address)
                .HasForeignKey(a => a.Client_address_id);

            modelBuilder.Entity<cms_account>()
                .HasMany(c => c.Cms_account_banks)      // One account has many banks
                .WithOne(a => a.Cms_account)              // Each bank belongs to one account
                .HasForeignKey(a => a.Acc_id);

            modelBuilder.Entity<cms_account>()
                .HasMany(c => c.Cms_account_controlling_persons)
                .WithOne(a => a.Cms_account)
                .HasForeignKey(a => a.Acc_id);

            modelBuilder.Entity<cms_account>()
                .HasMany(c => c.Cms_account_auth_parties)
                .WithOne(a => a.Cms_account)
                .HasForeignKey(a => a.Acc_id);

            /* add */
            modelBuilder.Entity<cms_client>()
                .HasMany(c => c.Cms_client_banks)      // One client has many banks
                .WithOne(a => a.Cms_client)              // Each bank belongs to one client
                .HasForeignKey(a => a.Client_id);

            modelBuilder.Entity<cms_client_bank>()
               .HasMany(c => c.Cms_account_banks)
               .WithOne(a => a.Cms_client_bank)
               .HasForeignKey(a => a.Client_bank_id);

            modelBuilder.Entity<cms_client>()
                .HasMany(c => c.Cms_client_additionalinfos)      // One client has many banks
                .WithOne(a => a.Cms_client)              // Each bank belongs to one client
                .HasForeignKey(a => a.Client_id);

            modelBuilder.Entity<cms_additionalinfo>()
                .HasMany(c => c.Cms_client_additionalinfos)
                .WithOne(a => a.Cms_additionalinfo)
                .HasForeignKey(a => a.Addinfo_id);

            modelBuilder.Entity<cms_additionalinfo>()
                .HasMany(c => c.Cms_account_additionalinfos)
                .WithOne(a => a.Cms_additionalinfo)
                .HasForeignKey(a => a.Addinfo_id);

            modelBuilder.Entity<cms_account>()
                .HasMany(c => c.Cms_account_emails)
                .WithOne(a => a.Cms_account)
                .HasForeignKey(a => a.Acc_id);

            modelBuilder.Entity<cms_account>()
                .HasMany(c => c.Cms_account_mthsaving_plans)
                .WithOne(a => a.Cms_account)
                .HasForeignKey(a => a.Acc_id);

            modelBuilder.Entity<cms_client>()
                .HasMany(c => c.Cms_client_emails)
                .WithOne(a => a.Cms_client)
                .HasForeignKey(a => a.Client_id);

            modelBuilder.Entity<cms_client_email>()
                .HasMany(c => c.Cms_account_emails)
                .WithOne(a => a.Cms_client_email)
                .HasForeignKey(a => a.Client_email_id);


            modelBuilder.Entity<cms_account>()
                .HasMany(c => c.Cms_account_phones)
                .WithOne(a => a.Cms_account)
                .HasForeignKey(a => a.Acc_id);

            modelBuilder.Entity<cms_account_company_structure>()
                .HasMany(c => c.Cms_account_company_structure_directors)
                .WithOne(a => a.Cms_account_company_structure)
                .HasForeignKey(a => a.Comp_struct_id);

            modelBuilder.Entity<cms_account_company_structure>()
                .HasMany(c => c.Cms_account_company_structure_shareholders)
                .WithOne(a => a.Cms_account_company_structure)
                .HasForeignKey(a => a.Comp_struct_id);

            modelBuilder.Entity<cms_account_company_structure>()
                .HasMany(c => c.Cms_account_company_structure_intermediaries)
                .WithOne(a => a.Cms_account_company_structure)
                .HasForeignKey(a => a.Comp_struct_id);

            modelBuilder.Entity<cms_client>()
                .HasMany(c => c.Cms_client_phones)
                .WithOne(a => a.Cms_client)
                .HasForeignKey(a => a.Client_id);

            modelBuilder.Entity<cms_client>()
                .HasMany(c => c.Cms_client_ids)
                .WithOne(a => a.Cms_client)
                .HasForeignKey(a => a.Client_id);
            
            modelBuilder.Entity<cms_account>()
                .HasMany(c => c.Cms_account_docs)
                .WithOne(a => a.Cms_account)
                .HasForeignKey(a => a.Acc_id);

            modelBuilder.Entity<cms_client_phone>()
                .HasMany(c => c.Cms_account_phones)
                .WithOne(a => a.Cms_client_phone)
                .HasForeignKey(a => a.Client_phone_id);


            modelBuilder.Entity<cms_bank>()
                .HasMany(c => c.Cms_client_banks)
                .WithOne(a => a.Cms_bank)
                .HasForeignKey(a => a.Bank_id);

            modelBuilder.Entity<cms_bank>()
                .HasMany(c => c.Cms_account_banks)
                .WithOne(a => a.Cms_bank)
                .HasForeignKey(a => a.Client_bank_id);

            modelBuilder.Entity<cms_account>()
                .HasOne(c => c.Cms_account_w8)         // One account has one w8
                .WithOne(w => w.Cms_account)           // The W8 has one account
                .HasForeignKey<cms_account_w8>(w => w.Acc_id); // Specify the foreign key in cms_account_w8

            /*modelBuilder.Entity<cms_ae>()
                .HasOne(d => d.Cms_account)
                .WithOne(w => w.Cms_ae)
                .HasForeignKey<cms_account>(w => w.Ae_id);*/

            modelBuilder.Entity<cms_ae>()
                .HasMany(d => d.Cms_accounts)
                .WithOne(w => w.Cms_ae)
                .HasForeignKey(w => w.Ae_id);

            modelBuilder.Entity<cms_branch>()
                .HasMany(d => d.Cms_aes)
                .WithOne(w => w.Cms_branch)
                .HasForeignKey(w => w.Branch_id);

            modelBuilder.Entity<cms_account>()
                .HasOne(c => c.Cms_account_dkq)         // One account has one dkq
                .WithOne(w => w.Cms_account)           // The W8 has one account
                .HasForeignKey<cms_account_dkq>(w => w.Acc_id); // Specify the foreign key in cms_account_dkq

            modelBuilder.Entity<cms_account>()
                .HasOne(c => c.Cms_account_hkidr)         // One account has one hkidr
                .WithOne(w => w.Cms_account)           // The W8 has one account
                .HasForeignKey<cms_account_hkidr>(w => w.Acc_id); // Specify the foreign key in cms_account_hkidr

            modelBuilder.Entity<cms_account>()
                .HasOne(c => c.Cms_account_quoteservice)         // One account has one quoteservice
                .WithOne(w => w.Cms_account)           // The W8 has one account
                .HasForeignKey<cms_account_quoteservice>(w => w.Acc_id); // Specify the foreign key in cms_account_quoteservice

            modelBuilder.Entity<cms_account>()
                .HasOne(c => c.Cms_account_north_bound)
                .WithOne(w => w.Cms_account)
                .HasForeignKey<cms_account_north_bound>(w => w.Acc_id);

            modelBuilder.Entity<cms_account>()
                .HasOne(c => c.Cms_account_company_structure)
                .WithOne(w => w.Cms_account)
                .HasForeignKey<cms_account_company_structure>(w => w.Acc_id);

            modelBuilder.Entity<cms_client>()
                .HasOne(c => c.Cms_client_north_bound)
                .WithOne(w => w.Cms_client)
                .HasForeignKey<cms_client_north_bound>(w => w.Client_id);

            modelBuilder.Entity<cms_client_id>()
                .HasOne(c => c.Cms_client_north_bound)
                .WithOne(w => w.Cms_client_id)
                .HasForeignKey<cms_client_north_bound>(w => w.Id_id);

            modelBuilder.Entity<cms_client>()
                .HasMany(c => c.Cms_child_clients)
                .WithOne(w => w.Cms_parent_client)
                .HasForeignKey(a => a.Virtual_client_id);

            modelBuilder.Entity<cms_client>()
                .HasMany(c => c.Cms_virtual_clients)
                .WithOne(w => w.Cms_child_client)
                .HasForeignKey(a => a.Client_id);

            // Other configurations
            modelBuilder.Entity<SelectListItem>(entity =>
            {
                // Ignore the Group navigation property
                entity.Ignore(e => e.Group);
            });

            modelBuilder.Entity<cms_client>()
                .HasKey(r => r.Id); // Specify the primary key

            modelBuilder.Entity<cms_account>()
                .HasKey(r => r.Id); // Specify the primary key

            modelBuilder.Entity<cms_addr_comp_opt>()
                .HasKey(c => new { c.Country, c.ProvinceEn, c.StateEn, c.CityEn, c.DistrictEn });

            modelBuilder.Entity<cms_client_address>()
                .HasKey(r => r.Id); // Specify the primary key

            modelBuilder.Entity<cms_account_address>()
                .HasKey(r => r.Id); // Specify the primary key

            modelBuilder.Entity<cms_client_email>()
                .HasKey(r => r.Id); // Specify the primary key

            modelBuilder.Entity<cms_account_email>()
                .HasKey(r => r.Id); // Specify the primary key

            modelBuilder.Entity<cms_account_north_bound>()
                .HasKey(r => r.Id); // Specify the primary key

            modelBuilder.Entity<cms_account_company_structure>()
                .HasKey(r => r.Id); // Specify the primary key

            modelBuilder.Entity<cms_account_controlling_person>()
                .HasKey(r => r.Id); // Specify the primary key

            modelBuilder.Entity<cms_account_auth_party>()
                .HasKey(r => r.Id); // Specify the primary key

            /*modelBuilder.Entity<cms_account_mthsaving_plan>()
                .HasKey(r => r.Id); // Specify the primary key*/

            modelBuilder.Entity<cms_client_phone>()
                .HasKey(r => r.Id); // Specify the primary key

            modelBuilder.Entity<cms_client_id>()
                .HasKey(r => r.Id); // Specify the primary key

            modelBuilder.Entity<cms_account_phone>()
                .HasKey(r => r.Id); // Specify the primary key

            modelBuilder.Entity<cms_account_company_structure_director>()
                .HasKey(r => r.Id); // Specify the primary key

            modelBuilder.Entity<cms_account_company_structure_shareholder>()
                .HasKey(r => r.Id); // Specify the primary key

            modelBuilder.Entity<cms_account_company_structure_intermediary>()
                .HasKey(r => r.Id); // Specify the primary key
            
            /*modelBuilder.Entity<cms_acc_address>()
                .HasKey(r => r.Id); // Specify the primary key*/

            modelBuilder.Entity<cms_bank>()
                .HasKey(r => r.Id); // Specify the primary key

            /*modelBuilder.Entity<cms_account_w8>()
                .HasKey(r => r.Id); // Specify the primary key*/

            modelBuilder.Entity<cms_account_dkq>()
                .HasKey(r => r.Id); // Specify the primary key

            modelBuilder.Entity<cms_account_hkidr>()
                .HasKey(r => r.Id); // Specify the primary key

            modelBuilder.Entity<cms_account_quoteservice>()
                .HasKey(r => r.Id); // Specify the primary key

            modelBuilder.Entity<cms_virtual_client>()
                .HasKey(r => r.Id); // Specify the primary key

            modelBuilder.Entity<cms_account_additionalinfo>()
                .HasKey(r => r.Id); // Specify the primary key

            modelBuilder.Entity<cms_account_limit>(entity =>
            {
                entity.HasKey(r => r.Id); // Specify the primary key

                entity.Property(e => e.Lmt)
                    .HasColumnType("decimal(28, 2)") // Adjust precision and scale as needed
                    .HasPrecision(28, 2); // This sets precision and scale

                entity.Property(e => e.Pct)
                    .HasColumnType("decimal(28, 6)") // Adjust precision and scale as needed
                    .HasPrecision(28, 6); // This sets precision and scale
            });

            modelBuilder.Entity<cms_account_market_brokerage>(entity =>
            {
                entity.HasKey(r => r.Id); // Specify the primary key

                entity.Property(e => e.Min)
                    .HasColumnType("decimal(28, 2)") // Adjust precision and scale as needed
                    .HasPrecision(28, 2); // This sets precision and scale

                entity.Property(e => e.Max)
                    .HasColumnType("decimal(28, 2)") // Adjust precision and scale as needed
                    .HasPrecision(28, 2); // This sets precision and scale

                entity.Property(e => e.Discount)
                    .HasColumnType("decimal(28, 2)") // Adjust precision and scale as needed
                    .HasPrecision(28, 2); // This sets precision and scale

                entity.Property(e => e.Additional_discount)
                    .HasColumnType("decimal(28, 2)") // Adjust precision and scale as needed
                    .HasPrecision(28, 2); // This sets precision and scale
            });

            modelBuilder.Entity<cms_account_market_brokerage_detail>(entity =>
            {
                entity.HasKey(r => r.Id); // Specify the primary key

                entity.Property(e => e.Fm_amt)
                    .HasColumnType("decimal(28, 2)") // Adjust precision and scale as needed
                    .HasPrecision(28, 2); // This sets precision and scale

                entity.Property(e => e.To_amt)
                    .HasColumnType("decimal(28, 2)") // Adjust precision and scale as needed
                    .HasPrecision(28, 2); // This sets precision and scale

                entity.Property(e => e.Rate)
                    .HasColumnType("decimal(28, 6)") // Adjust precision and scale as needed
                    .HasPrecision(28, 6); // This sets precision and scale

                entity.Property(e => e.Additional_Amt)
                    .HasColumnType("decimal(28, 2)") // Adjust precision and scale as needed
                    .HasPrecision(28, 2); // This sets precision and scale
            });

            modelBuilder.Entity<cms_account_market_interest_detail>(entity =>
            {
                entity.HasKey(r => r.Id); // Specify the primary key

                entity.Property(e => e.Reach)
                    .HasColumnType("decimal(28, 8)") // Adjust precision and scale as needed
                    .HasPrecision(28, 8); // This sets precision and scale

                entity.Property(e => e.Adj_rate)
                    .HasColumnType("decimal(28, 6)") // Adjust precision and scale as needed
                    .HasPrecision(28, 6); // This sets precision and scale
            });

            modelBuilder.Entity<cms_account_market_price_cap>(entity =>
            {
                entity.HasKey(r => r.Id); // Specify the primary key

                entity.Property(e => e.Margin_pct)
                    .HasColumnType("decimal(28, 6)") // Adjust precision and scale as needed
                    .HasPrecision(28, 6); // This sets precision and scale

                entity.Property(e => e.Price_cap)
                    .HasColumnType("decimal(28, 6)") // Adjust precision and scale as needed
                    .HasPrecision(28, 6); // This sets precision and scale
            });

            modelBuilder.Entity<cms_account_market_rebate>(entity =>
            {
                entity.HasKey(r => r.Id); // Specify the primary key

                entity.Property(e => e.Rate)
                    .HasColumnType("decimal(28, 6)") // Adjust precision and scale as needed
                    .HasPrecision(28, 6); // This sets precision and scale
            });
            
            modelBuilder.Entity<cms_account_market_limit>(entity =>
            {
                entity.HasKey(r => r.Id); // Specify the primary key

                entity.Property(e => e.Lmt)
                    .HasColumnType("decimal(28, 2)") // Adjust precision and scale as needed
                    .HasPrecision(28, 2); // This sets precision and scale

                entity.Property(e => e.Pct)
                    .HasColumnType("decimal(28, 6)") // Adjust precision and scale as needed
                    .HasPrecision(28, 5); // This sets precision and scale
            });

            modelBuilder.Entity<cms_account_mthsaving_plan>(entity =>
            {
                entity.HasKey(r => r.Id); // Specify the primary key

                entity.Property(e => e.Invest_amt)
                    .HasColumnType("decimal(28, 2)") // Adjust precision and scale as needed
                    .HasPrecision(28, 2); // This sets precision and scale
            });

            modelBuilder.Entity<cms_account_w8>(entity =>
            {
                entity.HasKey(r => r.Id); // Specify the primary key

                entity.Property(e => e.Treaty_percent)
                    .HasColumnType("decimal(28, 2)") // Adjust precision and scale as needed
                    .HasPrecision(28, 2); // This sets precision and scale
            });

            /*modelBuilder.Entity<cms_account_limit>()
                .HasKey(r => r.Id); // Specify the primary key*/

            modelBuilder.Entity<cms_account_market>()
                .HasKey(r => r.Id); // Specify the primary key

            modelBuilder.Entity<cms_account_market_limit>()
                .HasKey(r => r.Id); // Specify the primary key

            modelBuilder.Entity<cms_account_market_cash_si>()
                .HasKey(r => r.Id); // Specify the primary key

            modelBuilder.Entity<cms_account_market_inscat>()
                .HasKey(r => r.Id); // Specify the primary key

            modelBuilder.Entity<update_cms_client>()
                .HasKey(r => r.Id); // Specify the primary key

            modelBuilder.Entity<update_cms_client_phone>()
                .HasKey(r => r.Id); // Specify the primary key

            modelBuilder.Entity<update_cms_client_email>()
                .HasKey(r => r.Id); // Specify the primary key

            modelBuilder.Entity<update_cms_client_address>()
                .HasKey(r => r.Id); // Specify the primary key

            // Triggers
            modelBuilder.Entity<cms_client_address>()
                .ToTable(tb => tb.HasTrigger("tr_cms_client_addresses_dbopr"));

            modelBuilder.Entity<cms_account_address>()
                .ToTable(tb => tb.HasTrigger("tr_cms_account_addresses_dbopr"));

            modelBuilder.Entity<cms_client_bank>()
                .ToTable(tb => tb.HasTrigger("tr_cms_client_banks_dbopr"));

            modelBuilder.Entity<cms_account_bank>()
                .ToTable(tb => tb.HasTrigger("tr_cms_account_banks_dbopr"));

            modelBuilder.Entity<cms_client_email>()
                .ToTable(tb => tb.HasTrigger("tr_cms_client_emails_dbopr"));

            modelBuilder.Entity<cms_account_email>()
                .ToTable(tb => tb.HasTrigger("tr_cms_account_emails_dbopr"));

            modelBuilder.Entity<cms_client_phone>()
                .ToTable(tb => tb.HasTrigger("tr_cms_client_phones_dbopr"));

            modelBuilder.Entity<cms_account_phone>()
                .ToTable(tb => tb.HasTrigger("tr_cms_account_phones_dbopr"));

            modelBuilder.Entity<cms_account_mthsaving_plan>()
                .ToTable(tb => tb.HasTrigger("tr_cms_account_mthsaving_plans_dbopr"));

            modelBuilder.Entity<cms_account_dkq>()
                .ToTable(tb => tb.HasTrigger("tr_cms_account_dkqs_dbopr"));

            modelBuilder.Entity<cms_account_w8>()
                .ToTable(tb => tb.HasTrigger("tr_cms_account_w8s_dbopr"));

            modelBuilder.Entity<cms_account_quoteservice>()
                .ToTable(tb => tb.HasTrigger("tr_cms_account_quoteservices_dbopr"));

            modelBuilder.Ignore<SelectListItem>(); // Ignore SelectListItem
        }

        //public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<cms_client> Cms_clients { get; set; }

        public DbSet<cms_account> Cms_accounts { get; set; }

        public DbSet<cms_bank> Cms_banks { get; set; }

        public DbSet<cms_client_account> Cms_client_accounts { get; set; }

        public DbSet<cms_addr_comp_opt> Cms_addr_comp_opts { get; set; }

        public DbSet<cms_account_address> Cms_account_addresses { get; set; }

        //public DbSet<cms_acc_address> Cms_acc_addresses { get; set; }

        public DbSet<cms_client_address> Cms_client_addresses { get; set; }

        public DbSet<cms_account_email> Cms_account_emails { get; set; }

        public DbSet<cms_account_north_bound> Cms_account_north_bounds { get; set; }

        public DbSet<cms_client_phone> Cms_client_phones { get; set; }

        public DbSet<cms_account_phone> Cms_account_phones { get; set; }

        public DbSet<cms_account_company_structure_director> Cms_account_company_structure_director { get; set; }

        public DbSet<cms_account_company_structure_shareholder> Cms_account_company_structure_shareholder { get; set; }
        public DbSet<cms_account_company_structure_intermediary> Cms_account_company_structure_intermediaries { get; set; }

        public DbSet<cms_client_email> Cms_client_emails { get; set; }

        public DbSet<cms_client_bank> Cms_client_banks { get; set; }

        public DbSet<cms_client_additionalinfo> Cms_client_additionalinfos { get; set; }

        public DbSet<cms_account_bank> Cms_account_banks { get; set; }

        public DbSet<cms_account_w8> Cms_account_w8s { get; set; }

        public DbSet<cms_ae> Cms_aes { get; set; }

        public DbSet<cms_branch> Cms_branches { get; set; }

        public DbSet<cms_account_dkq> Cms_account_dkqs { get; set; }

        public DbSet<cms_account_hkidr> Cms_account_hkidrs { get; set; }

        public DbSet<cms_account_quoteservice> Cms_account_quoteservices { get; set; }

        public DbSet<cms_account_mthsaving_plan> Cms_account_mthsaving_plans { get; set; }

        public DbSet<cms_virtual_client> Cms_virtual_clients { get; set; }

        public DbSet<cms_account_additionalinfo> Cms_account_additionalinfos { get; set; }
        public DbSet<cms_account_limit> Cms_account_limits { get; set; }

        public DbSet<cms_account_market> Cms_account_markets { get; set; }

        public DbSet<cms_account_market_limit> Cms_account_market_limits { get; set; }

        public DbSet<cms_account_market_limit> Cms_account_market_cash_sis { get; set; }

        public DbSet<cms_account_market_limit> Cms_account_market_inst_sis { get; set; }

        public DbSet<cms_account_market_limit> Cms_account_market_confos { get; set; }

        public DbSet<cms_account_market_inscat> Cms_account_market_inscats { get; set; }

        public DbSet<cms_account_market_brokerage> Cms_account_market_brokerages { get; set; }

        public DbSet<cms_account_market_brokerage_detail> Cms_account_market_brokerage_details { get; set; }

        public DbSet<cms_account_market_interest> Cms_account_market_interests { get; set; }

        public DbSet<cms_account_market_interest_detail> Cms_account_market_interest_details { get; set; }

        public DbSet<cms_account_market_price_cap> Cms_account_market_price_caps { get; set; }

        public DbSet<cms_account_market_rebate> Cms_account_market_rebates { get; set; }

        public DbSet<cms_account_social_media> Cms_account_social_medias { get; set; }

        public DbSet<update_cms_client> Update_cms_clients { get; set; }

        public DbSet<update_cms_client_phone> Update_cms_client_phones { get; set; }

        public DbSet<update_cms_client_email> Update_cms_client_emails { get; set; }

        public DbSet<update_cms_client_address> Update_cms_client_addresses { get; set; }

        public DbSet<cms_account_controlling_person> Cms_account_controling_persons { get; set; }

        public DbSet<cms_account_auth_party> Cms_account_auth_parties { get; set; }

        public DbSet<cms_account_company_structure> Cms_account_company_structures { get; set; }        
    }
}
