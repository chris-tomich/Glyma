using System;

namespace Glyma.UtilityService.Export.IBIS.Common.Model.Interface
{
    public interface IGlymaObject
    {
        Guid Id { get; }

        string OrginalId { get; }
    }
}
