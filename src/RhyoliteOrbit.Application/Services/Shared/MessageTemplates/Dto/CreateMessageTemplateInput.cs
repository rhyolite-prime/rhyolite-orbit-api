using System;
using Abp.Application.Services.Dto;

namespace RhyoliteOrbit.Services.Shared.MessageTemplates.Dto;

public class CreateMessageTemplateInput
{
    public string MessageContent { get; set; }
    public string MessageSubject { get; set; }
    public string Alias { get; set; }
}

public class UpdateMessageTemplateInput: EntityDto<Guid>
{
    public string MessageContent { get; set; }
    public string MessageSubject { get; set; }
    public string Alias { get; set; }
}