using ChatApp.Data.Interfaces;
using ChatApp.Business.DTOs;
using ChatApp.Data.Enums;
using ChatApp.Business.Interfaces;
using ChatApp.Data.Entities;
using ChatApp.Business.Results;

namespace ChatApp.Business.Services;

public class ChannelService(IChannelRepository channelRepository) : IChannelService
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

        if(channels == null || !channels.Any())
            return Enumerable.Empty<ChannelListDto>();


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


    //TODO : Create new method in repository to make sure no need to fetch all users and let ef handle count query
    //Fetch all public channels for searchbar
    
    public async Task<IEnumerable<ChannelListDto>> GetPublicChannelListDtosAsync(int id)
    {
        var channels = await _channelRepository.GetAllAsync(
            filterBy: c => !c.IsPrivate && !c.Members.Any(m => m.UserId == id),
            orderByDescending: true,
            sortBy: c => c.CreatedAt,
            includes: c => c.Members,
            take: 20
            );
        if (channels == null || !channels.Any())
            return Enumerable.Empty<ChannelListDto>();

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
    public async Task<IEnumerable<ChannelListDto>> GetPublicChannelsByNameSearch(string name)
    {
        var channels = await _channelRepository.GetAllAsync(
            filterBy: c => !c.IsPrivate && c.Name.Contains(name),
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

    public async Task<ServiceResult> CreateChannelAsync(CreateChannelDto channelDto)
    {
        var channel = new TextChannelEntity
        {
            Name = channelDto.Name,
            Description = channelDto.Description,
            IsPrivate = channelDto.IsPrivate,
            CreatedByUserId = channelDto.CreatedByUserId,
            Members = new List<ChannelMemberEntity>()
            {
                new ChannelMemberEntity
                {
                    UserId = channelDto.CreatedByUserId,
                    Status = MembershipStatus.Accepted,
                    Role = ChannelRole.Admin
                }
            }
        };

        var result = await _channelRepository.AddAsync(channel);

        if (result == false)
            return new ServiceResult { Success = false, Message = "Failed to create channel" };

        return new ServiceResult { Success = true, Message = "Channel created successfully" };
    }

    public async Task<ServiceResult> JoinChannelAsync(int channelId, int userId)
    {
        var channel = await _channelRepository.GetAsync(c => c.Id == channelId, c => c.Members);
        if (channel == null)
            return new ServiceResult { Success = false, Message = "Channel not found" };

        if (channel.Members.Any(m => m.UserId == userId && m.Status == MembershipStatus.Accepted))
            return new ServiceResult { Success = false, Message = "User is already a member of the channel" };

        var newMember = new ChannelMemberEntity
        {
            UserId = userId,
            Status = MembershipStatus.Accepted,
            Role = ChannelRole.Member
        };
        channel.Members.Add(newMember);
        var updateResult = await _channelRepository.UpdateAsync(channel);
        if (!updateResult)
            return new ServiceResult { Success = false, Message = "Failed to join channel" };
        return new ServiceResult { Success = true, Message = "Joined channel successfully" };
    }

}
