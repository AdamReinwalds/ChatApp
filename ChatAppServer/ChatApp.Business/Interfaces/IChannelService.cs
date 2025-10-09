using ChatApp.Business.DTOs;
using ChatApp.Business.Results;

namespace ChatApp.Business.Interfaces
{
    public interface IChannelService
    {
        Task<ServiceResult> CreateChannelAsync(CreateChannelDto channelDto);
        Task<ChannelDetailDto> GetChannelAsync(int id);
        Task<IEnumerable<ChannelListDto>> GetChannelListDtosAsync(int userId);
        Task<IEnumerable<ChannelListDto>> GetPublicChannelsByNameSearch(string name);
        Task<IEnumerable<ChannelListDto>> GetPublicChannelListDtosAsync(int id);
        Task<ServiceResult> JoinChannelAsync(int channelId, int userId);
    }
}