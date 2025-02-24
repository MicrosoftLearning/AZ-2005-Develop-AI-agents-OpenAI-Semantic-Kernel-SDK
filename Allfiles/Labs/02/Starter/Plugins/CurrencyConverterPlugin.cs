using System.ComponentModel;
using Microsoft.SemanticKernel;

class CurrencyConverterPlugin
{
    // A dictionary that stores exchange rates for demonstration
    private static Dictionary<string, decimal> exchangeRates = new Dictionary<string, decimal>
    {
        { "USD-EUR", 0.85m },
        { "EUR-USD", 1.18m },
        { "USD-GBP", 0.75m },
        { "GBP-USD", 1.33m },
        { "USD-JPY", 110.50m },
        { "JPY-USD", 1 / 110.50m },
        { "USD-HKD", 7.77m },
        { "HKD-USD", 1 / 7.77m }
    };

    public static decimal GetExchangeRate(string fromCurrency, string toCurrency)
    {
        string key = $"{fromCurrency}-{toCurrency}";
        if (exchangeRates.ContainsKey(key))
        {
            return exchangeRates[key];
        }
        else
        {
            throw new Exception("Exchange rate not available for this currency pair.");
        }
    }
}