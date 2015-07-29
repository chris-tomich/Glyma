using System;

namespace Glyma.UtilityService.Proxy
{
    public interface IResult<out T> 
    {
        bool HasError { get; }

        string ErrorMessage { get; }

        T Result { get; }
    }
}
