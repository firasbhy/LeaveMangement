using AutoMapper;
using LeaveManagement.DTOs;

namespace LeaveManagement.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<LeaveRequest,LeaveRequestDto>();
            CreateMap<CreateLeaveRequestDto,LeaveRequest>();

        }
    }
}