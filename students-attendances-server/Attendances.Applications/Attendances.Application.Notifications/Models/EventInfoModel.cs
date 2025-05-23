using Attendances.Domain.Core.MessageBus;
using Attendances.Domain.Messages.Entities;
using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Attendances.Application.Notifications.Models;

public class EventInfoModel
{
    public Guid EventUuid { get; set; } = Guid.NewGuid();
    public required string EventType { get; set; }
    public required long TimeStamp { get; set; }

    public bool IsHandled { get; set; } = false;
    public required JToken Payload { get; set; }
}

public class EventInfoModelProfile : Profile
{
    public EventInfoModelProfile()
    {
        CreateMap<EventInfo, EventInfoModel>()
            .ForMember(dest => dest.EventUuid, opt => opt.MapFrom(src => src.Uuid))
            .ReverseMap();
        CreateMap<EventInfoModel, MessageBase>();
    }
}