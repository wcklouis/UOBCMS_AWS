using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UOBCMS.Models.IBO
{
    public class Instrument
    {
        public string Market { get; set; }

        public string Instr { get; set; }

        public string Name { get; set; }


        public string ShortName { get; set; }

        public string CName { get; set; }

        public string CShortName { get; set; }

        public string InstrumentClass { get; set; }

        public string ProdCode { get; set; }

        public decimal LotSize { get; set; }

        public decimal Price { get; set; }

        public decimal TransactionValueLimit { get; set; }

        public decimal TransactionQtyLimit { get; set; }

        public decimal DailyValueLimit { get; set; }

        public decimal DailyQtyLimit { get; set; }

        public DateTime? ExpiryDate { get; set; }
        public string Active { get; set; }

        public decimal MarginPercent { get; set; }

        public string Series { get; set; }

        public decimal? PriceCap { get; set; }
        public decimal CalculationPrice { get; set; }

        public string Suspended { get; set; }

        public string TradingCCY { get; set; }

        public string ExchangeCode { get; set; }

        public string ExchangeInternalCode { get; set; }

        public string? Bloomberg { get; set; }
        public string Reuter { get; set; }

        public string ISIN { get; set; }

        public string Sedol { get; set; }

        public DateTime? FirstDealingDate { get; set; }
        public DateTime? LastDealingDate { get; set; }
        public string ElectronicClearing { get; set; }

        public string ShortSell { get; set; }

        public decimal ParValue { get; set; }

        public string? Fee1 { get; set; }
        public string? Fee2 { get; set; }
        public string? Fee3 { get; set; }
        public string? Fee4 { get; set; }
        public string? Fee5 { get; set; }
        public string? Fee6 { get; set; }
        public string? Fee7 { get; set; }
        public string? Fee8 { get; set; }
        public string? Fee9 { get; set; }
        public string? Fee10 { get; set; }
        public string Category { get; set; }
        public string SpreadTableCode { get; set; }
        public string StatusFlag { get; set; }
        public byte[]? TimeStamp { get; set; }
        public string InstrumentType { get; set; }
        public DateTime? DateOfListed { get; set; }
        public string? IlliquidCollateralCalculation { get; set; }
        public DateTime? SuspendFrom { get; set; }
        public decimal? LastSuspendPrice { get; set; }
        public string? UnderStock { get; set; }
    }
}
