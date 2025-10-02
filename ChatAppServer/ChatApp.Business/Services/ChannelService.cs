using ChatApp.Data.Interfaces;
using ChatApp.Business.DTOs;
using ChatApp.Data.Enums;

namespace ChatApp.Business.Services;

public class ChannelService(IChannelRepository channelRepository)
{
    private readonly IChannelRepository _channelRepository = channelRepository;

    //Fetch specific channel with messages and members
    public async Task<ChannelDetailDto> GetChannelAsync(int id)
    {
        var channel = await _channelRepository.GetAsync(c => c.Id == id);
        var channelDto = new ChannelDetailDto
        {
            Id = channel.Id,
            Name = channel.Name,
            Description = channel.Description,
            Messages = channel.Messages,
            Members = channel.Members,
            IsPrivate = channel.IsPrivate
        };
        return channelDto;
    }

    //Fetch channels where user is a member for sidebar
    public async Task<IEnumerable<ChannelListDto>> GetChannelListDtosAsync(int userId)
    {
        var channels = await _channelRepository.GetAllAsync(
            filterBy: c => c.Members.Any(m => m.UserId == userId && m.Status == MembershipStatus.Accepted),
            includes: c => c.Members
            );


        var channelListDtos = new List<ChannelListDto>();
        foreach (var channel in channels)
        {
            var channelListDto = new ChannelListDto
            {
                Id = channel.Id,
                Name = channel.Name,
                IsPrivate = channel.IsPrivate,
                MemberCount = channel.Members.Count
            };
            channelListDtos.Add(channelListDto);
        }
        return channelListDtos;
    }

    //Fetch all public channels for searchbar
    public async Task<IEnumerable<ChannelListDto>> GetPublicChannelListDtosAsync()
    {
        var channels = await _channelRepository.GetAllAsync(
            filterBy: c => !c.IsPrivate,
            includes: c => c.Members,
            orderByDescending: true,
            sortBy: c => c.CreatedAt,
            take: 20
            );

        var channelListDtos = new List<ChannelListDto>();
        foreach (var channel in channels)
        {
            var channelListDto = new ChannelListDto
            {
                Id = channel.Id,
                Name = channel.Name,
                IsPrivate = channel.IsPrivate,
                MemberCount = channel.Members.Count
            };
            channelListDtos.Add(channelListDto);
        }
        return channelListDtos;
    }

    //Get channels by name search
    public async Task<IEnumerable<ChannelListDto>> GetPublicChannelByNameSearch(string name)
    {
        var channels = await _channelRepository.GetAllAsync(
            filterBy: c => !c.IsPrivate && c.Name.Contains(name),
            includes: c => c.Members,
            orderByDescending: true,
            sortBy: c => c.CreatedAt
            );
        var channelListDtos = new List<ChannelListDto>();
        foreach(var channel in channels)
        {
            var channelListDto = new ChannelListDto
            {
                Id = channel.Id,
                Name = channel.Name,
                IsPrivate = channel.IsPrivate,
                MemberCount = channel.Members.Count
            };
            channelListDtos.Add(channelListDto);
        }
        return channelListDtos;
    }
}
