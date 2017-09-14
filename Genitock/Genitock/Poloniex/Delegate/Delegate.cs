using System;
using Genitock.Entity.Poloniex;

namespace Genitock.Poloniex.Delegate
{
    /// <summary>
    /// event raised when a tick come from poloniex
    /// </summary>
    public delegate void OnTick(object source, TickerArg e);
}
