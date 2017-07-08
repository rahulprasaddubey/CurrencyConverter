using CurrencyConverter.Constants;
using HtmlAgilityPack;
using Quartz;
using RestSharp;
using System;
using System.Linq;

namespace CurrencyConverter
{
    public class RateExchangeJob:IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                double amount = 1;
                foreach (var currency in Currency.SupportedCurrency)
                {
                    double rate = GetExchangeRates(currency, amount);
                    StoreExchangeRates(currency, rate);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private float GetExchangeRates(string currency, double amount)
        {
            string BASE_URL = "https://www.google.com/finance/";
            string requestUrl = string.Format(BASE_URL);
            RestClient client = new RestClient(requestUrl);
            RestRequest req = new RestRequest("converter", Method.GET);
            req.AddParameter("a", amount);
            req.AddParameter("from", currency);
            req.AddParameter("to", "INR");
            IRestResponse response = client.Execute(req);
            var content = response.Content;
            string rate = GetRateExchangeValue(content);
            return (float.Parse(rate.Split(' ')[0].ToString()));
        }
        private string GetRateExchangeValue(string htmlString)
        {
            try
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(htmlString);
                var result = doc.DocumentNode.SelectNodes("//span[(@class='bld')]").FirstOrDefault().InnerText;
                return result;
            }
            catch
            {
                return string.Empty;
            }
        }

        private void StoreExchangeRates(string currencyCode, double rate)
        {
            try
            {
                using (RateExchangeContainer db = new RateExchangeContainer())
                {
                    CurrencyExchangeRate rates = db.CurrencyExchangeRates.Where(x => x.Currency.Equals(currencyCode)).FirstOrDefault();
                    if (rates == null)
                    {
                        rates = new CurrencyExchangeRate();
                        rates.Currency = currencyCode;
                        rates.RateInINR = rate;
                        rates.RowUpdatedDate = DateTime.UtcNow;
                        db.CurrencyExchangeRates.Add(rates);
                    }
                    else
                    {
                        rates.Currency = currencyCode;
                        rates.RateInINR = rate;
                        rates.RowUpdatedDate = DateTime.UtcNow;
                    }
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}