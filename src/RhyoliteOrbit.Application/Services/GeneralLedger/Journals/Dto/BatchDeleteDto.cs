using System;
using System.Collections.Generic;

namespace RhyoliteOrbit.Services.GeneralLedger.Journals.Dto;

public class BatchDeleteDto
{
    public List<Guid> Ids  { get; set; }
}