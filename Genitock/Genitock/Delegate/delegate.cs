using System;
using Genitock.Entity.Live;
using Genitock.Entity.Poloniex;

namespace Genitock.Delegate
{
   public delegate void OnTick(object source, TickerArgument e);
}
