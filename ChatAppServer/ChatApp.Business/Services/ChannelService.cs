using ChatApp.Data.Interfaces;
using ChatApp.Business.DTOs;
using ChatApp.Data.Enums;
using ChatApp.Business.Interfaces;
using ChatApp.Data.Entities;
using ChatApp.Business.Results;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Business.Services;

public class ChannelService(IChannelRepository channelRepository, ILogger<ChannelService> logger) : IChannelService
{
    private readonly IChannelRepository _channelRepository = channelRepository;
    private readonly ILogger<ChannelService> _logger = logger;

    //Fetch specific channel with messages and members
    public async Task<ChannelDetailDto> GetChannelAsync(int id)
    {
        var channel = await _channelRepository.GetAsync(c => c.Id == id);
        if (channel == null)
        {
            _logger.LogWarning("Channel with ID {ChannelId} not found.", id);
            return null!;
        }
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
        {
            _logger.LogInformation("No channels found for user with ID {UserId}.", userId);
            return Enumerable.Empty<ChannelListDto>();
        }


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
        {
            _logger.LogInformation("No public channels found for user with ID {UserId}.", id);
            return Enumerable.Empty<ChannelListDto>();
        }

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
        if (string.IsNullOrWhiteSpace(name))
        {
            _logger.LogWarning("Search term is empty or whitespace.");
            return Enumerable.Empty<ChannelListDto>();
        }
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
        if (string.IsNullOrWhiteSpace(channelDto.Name))
            return new ServiceResult { Success = false, Message = "Channel name is required" };

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
        try
        {
            var result = await _channelRepository.AddAsync(channel);
            if (!result)
            {
                _logger.LogError("Failed to create channel {ChannelName} by user {UserId}", channelDto.Name, channelDto.CreatedByUserId);
                return new ServiceResult { Success = false, Message = "Failed to create channel" };
            }
            _logger.LogInformation("Channel {ChannelName} created successfully by user {UserId}", channelDto.Name, channelDto.CreatedByUserId);
            return new ServiceResult { Success = true, Message = "Channel created successfully" };
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error while creating channel {ChannelName} by user {UserId}", channelDto.Name, channelDto.CreatedByUserId);
            return new ServiceResult { Success = false, Message = "Error creating channel" };
        }
    }

    public async Task<ServiceResult> JoinChannelAsync(int channelId, int userId)
    {
        var channel = await _channelRepository.GetAsync(c => c.Id == channelId, c => c.Members);
        if (channel == null)
        {
            _logger.LogWarning("Channel with ID {ChannelId} not found for user {UserId}", channelId, userId);
            return new ServiceResult { Success = false, Message = "Channel not found" };
        }

        if (channel.Members.Any(m => m.UserId == userId && m.Status == MembershipStatus.Accepted))
        {

            _logger.LogWarning("User {UserId} is already a member of channel {ChannelId}", userId, channelId);
            return new ServiceResult { Success = false, Message = "User is already a member of the channel" };
        }

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
