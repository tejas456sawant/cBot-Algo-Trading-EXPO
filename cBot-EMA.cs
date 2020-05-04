using System;
using System.Linq;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using cAlgo.Indicators;

namespace cAlgo.Robots
{
    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class SAR : Robot
    {
        [Parameter("Quantity (Lots)", Group = "Volume", DefaultValue = 0.1)]
        public double Quantity { get; set; }

        [Parameter("Data Source")]
        public DataSeries Price { get; set; }

        [Parameter("Slow Period", DefaultValue = 5)]
        public int SmaPeriod5 { get; set; }

        [Parameter("Medium Period", DefaultValue = 12)]
        public int SmaPeriod12 { get; set; }

        [Parameter("Fast Period", DefaultValue = 20)]
        public int SmaPeriod20 { get; set; }

        private ExponentialMovingAverage sma5, sma12, sma20;

        protected override void OnStart()
        {
            sma5 = Indicators.ExponentialMovingAverage(Price, SmaPeriod5);
            sma12 = Indicators.ExponentialMovingAverage(Price, SmaPeriod12);
            sma20 = Indicators.ExponentialMovingAverage(Price, SmaPeriod20);
        }

        protected override void OnTick()
        {
            var volumeInUnits = Symbol.QuantityToVolumeInUnits(Quantity);
            Print(volumeInUnits);
            var position = Positions.Find("SAR", SymbolName);
            if (sma20.Result.HasCrossedAbove(sma5.Result.Last(0), 0) && sma20.Result.HasCrossedAbove(sma12.Result.Last(0), 0))
            {
                ExecuteMarketOrder(TradeType.Buy, SymbolName, volumeInUnits, "SAR");

            }
            if (sma20.Result.HasCrossedBelow(sma5.Result.Last(0), 0) && sma20.Result.HasCrossedBelow(sma12.Result.Last(0), 0))
            {
                ExecuteMarketOrder(TradeType.Sell, SymbolName, volumeInUnits, "SAR");

            }
        }

        protected override void OnStop()
        {
            foreach (var position in Positions)
            {
                ClosePosition(position);
            }
        }
    }
}
