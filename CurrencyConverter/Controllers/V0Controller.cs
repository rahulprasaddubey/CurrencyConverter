using CurrencyConverter.Constants;
using CurrencyConverter.Models;
using System;
using System.Linq;
using System.Web.Http;

namespace CurrencyConverter.Controllers
{
    public class V0Controller : ApiController
    {
        [HttpPost]
        public object Rate(RateModel rates)
        {
            if (rates == null)
            {
                rates.Amount = 1;
                rates.Currency = Currency.USD;
            }
            if (!Currency.SupportedCurrency.Contains(rates.Currency))
            {
                return new
                {
                    err = rates.Currency + " is currently not supported for rate conversion",
                    returnCode = -1
                };
            }
            return GetConversionDetails(rates);
        }

        [OutputCacheFilter(120)]
        private object GetConversionDetails(RateModel rates)
        {
            using (RateExchangeContainer exchangeContainer = new RateExchangeContainer())
            {
                var exchangeRates = exchangeContainer.CurrencyExchangeRates.Where(x => x.Currency == rates.Currency).Select(r => new { r.RateInINR, r.RowUpdatedDate }).FirstOrDefault();
                double totalAmount = Math.Round(rates.Amount * exchangeRates.RateInINR, 2);
                double conversionRate = Math.Round(exchangeRates.RateInINR, 2);
                long timestamp = exchangeRates.RowUpdatedDate.Ticks;

                return new
                {
                    SourceCurrency = rates.Currency,
                    ConversionRate = conversionRate,
                    Amount = rates.Amount,
                    Total = totalAmount,
                    returncode = 1,
                    err = "success",
                    timestamp
                };
            }
        }
    }
}
