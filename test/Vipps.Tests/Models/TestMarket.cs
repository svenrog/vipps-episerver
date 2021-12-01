using Mediachase.Commerce;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;

namespace Vipps.Test.Models
{
    public class TestMarket : IMarket
    {
        private readonly string _marketId;
        private readonly Currency _currency;
        private readonly CultureInfo _culture;
        private readonly string _country;

        public TestMarket(string marketId)
        {
            _marketId = marketId;
            _currency = Currency.USD;
            _culture = Thread.CurrentThread.CurrentCulture;
            _country = "USA";
        }

        public MarketId MarketId => new MarketId(_marketId);

        public bool IsEnabled => true;

        public bool PricesIncludeTax => true;

        public string MarketName => _marketId;

        public string MarketDescription => string.Empty;

        public CultureInfo DefaultLanguage => _culture;

        public Currency DefaultCurrency => _currency;

        public IEnumerable<CultureInfo> Languages => new[] { _culture };

        public IEnumerable<Currency> Currencies => new [] { _currency };

        public IEnumerable<string> Countries => new [] { _country };
    }
}
