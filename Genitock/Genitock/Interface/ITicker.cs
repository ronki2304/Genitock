using System;
using Genitock.Delegate;

namespace Genitock.Interface
{
    public interface ITicker
    {
        event OnTick onTick;
    }
}
