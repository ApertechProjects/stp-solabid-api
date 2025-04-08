using AutoMapper;
using SolaBid.Business.Dtos.ApiDtos;
using SolaBid.Business.Dtos.ApiDtos.ComparisonChartDtos;
using SolaBid.Business.Dtos.ApiDtos.Register;
using SolaBid.Business.Dtos.EntityDtos;
using SolaBid.Business.Models;
using SolaBid.Domain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static SolaBid.Extensions.FileExtensions;


namespace SolaBid.Business.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AppUser, AppUserDto>().ReverseMap()
               .ForMember(x => x.RegDate, x => x.MapFrom(x => DateTime.Now))
               .ForMember(x => x.IsActive, x => x.MapFrom(x => false))
               .ForMember(x => x.IsApproved, x => x.MapFrom(x => false))
               .ForMember(x => x.UserName, x => x.MapFrom(x => x.Email));

            CreateMap<AppRole, KeyValueTextBoxingDto>()
                .ForMember(m => m.Key, option => option.MapFrom(s => s.Id))
                .ForMember(m => m.Value, option => option.MapFrom(s => s.Id))
                .ForMember(m => m.Text, option => option.MapFrom(s => s.Name));

            CreateMap<ApproveRoleDto, KeyValueTextBoxingDto>()
               .ForMember(m => m.Key, option => option.MapFrom(s => s.Id))
               .ForMember(m => m.Value, option => option.MapFrom(s => s.Id))
               .ForMember(m => m.Text, option => option.MapFrom(s => s.ApproveRoleName));

            CreateMap<AppRoleDto, KeyValueTextBoxingDto>()
                .ForMember(m => m.Key, option => option.MapFrom(s => s.Id))
                .ForMember(m => m.Value, option => option.MapFrom(s => s.Id))
                .ForMember(m => m.IsSelected, option => option.MapFrom(s => s.IsSelected))
                .ForMember(m => m.Text, option => option.MapFrom(s => s.Name));

            CreateMap<BuyerDto, KeyValueTextBoxingDto>()
               .ForMember(m => m.Key, option => option.MapFrom(s => s.Id))
               .ForMember(m => m.Value, option => option.MapFrom(s => s.Id))
               .ForMember(m => m.IsSelected, option => option.MapFrom(s => s.IsSelected))
               .ForMember(m => m.Text, option => option.MapFrom(s => s.Username));

            CreateMap<Site, KeyValueTextBoxingDto>()
             .ForMember(m => m.Key, option => option.MapFrom(s => s.Id))
             .ForMember(m => m.Value, option => option.MapFrom(s => s.Id))
             .ForMember(m => m.Text, option => option.MapFrom(s => s.SiteName));

            CreateMap<Status, KeyValueTextBoxingDto>()
           .ForMember(m => m.Key, option => option.MapFrom(s => s.Id))
           .ForMember(m => m.Value, option => option.MapFrom(s => s.Id))
           .ForMember(m => m.Text, option => option.MapFrom(s => s.StatusName));

            CreateMap<AppUser, ApprovedUserListDto>()
                .ForMember(m => m.BuyerName, option => option.MapFrom(s => s.BuyerUserName))
                .ForMember(m => m.Fullname, option => option.MapFrom(s => s.FirstName + " " + s.LastName))
                .ForMember(m => m.Image, option => option.MapFrom(s => ConvertFileToBase64(s.UserImage, "appfiles", "data:image/jpeg;base64,")))
                .ForMember(m => m.Groups, option => option.MapFrom(s => new Logics.GroupLogic().GetUserGroupsByUserId(s.Id).Result));

            CreateMap<AppUser, UserListDto>()
              .ForMember(m => m.FullName, option => option.MapFrom(s => s.FirstName + " " + s.LastName))
              .ForMember(m => m.Image, option => option.MapFrom(s => ConvertFileToBase64(s.UserImage, "appfiles", "data:image/jpeg;base64,")))
              .ForMember(m => m.Groups, option => option.MapFrom(s => new Logics.GroupLogic().GetUserGroupsByUserId(s.Id).Result));

            CreateMap<UsersForPrivilegeDto, AppUser>().ReverseMap()
                .ForMember(m => m.FullName, op => op.MapFrom(s => s.FirstName + " " + s.LastName));

            CreateMap<SubMenuParentMenuDto, SubMenu>().ReverseMap()
                .ForMember(m => m.Id, op => op.MapFrom(s => s.Id))
                .ForMember(m => m.ParentId, op => op.MapFrom(s => s.ParentMenuId))
                .ForMember(m => m.Name, op => op.MapFrom(s => s.SubMenuName));

            CreateMap<UserFormDataDto, AppUser>().ReverseMap()
                .ForMember(m => m.UserImageBase64, op => op.MapFrom(s => ConvertFileToBase64(s.UserImage, "appfiles", "data:image/jpeg;base64,")))
                .ForMember(m => m.UserSignatureBase64, op => op.MapFrom(s => ConvertFileToBase64(s.UserSignature, "appfiles", "data:image/jpeg;base64,")));

            CreateMap<ParentMenu, ParentMenuDto>().ReverseMap();
            CreateMap<SubMenu, SubMenuDto>().ReverseMap();
            CreateMap<GroupMenu, GroupMenuDto>().ReverseMap();
            CreateMap<AppRole, AppRoleDto>().ReverseMap();
            CreateMap<ApproveRole, ApproveRoleDto>().ReverseMap();
            CreateMap<ApproveStageDetail, ApproveStageDetailDto>().ReverseMap();
            CreateMap<ApproveStageDetailDto, ApproveStageDetailDto>().ReverseMap();
            CreateMap<ApproveStageMain, ApproveStageMainDto>().ReverseMap();
            CreateMap<LoginResult, ApiResult>().ReverseMap();
            CreateMap<Site, SiteDto>().ReverseMap();
            CreateMap<ComparisonChart, ComparisonChartDto>().ReverseMap();
            CreateMap<DiscountType, DiscountTypeDto>().ReverseMap();
            // CreateMap<SiteDto, SiteDto>().ReverseMap().ForMember(x => x.GroupSiteWarehouses, opt => opt.Ignore());
            CreateMap<AdditionalPrivilege, AdditionalPrivilegeDto>().ReverseMap();
            CreateMap<Vendor, VendorDto>().ForMember(m=>m.hasAttachment,op=>op.MapFrom(mf=>mf.VendorAttachments.Count>0)).ReverseMap();
            CreateMap<VendorAttachment, VendorAttachmentDto>().ReverseMap();
            // CreateMap<VendorDto, VendorDto>().ReverseMap().ForMember(m=>m.BIDReferances,op=>op.Ignore());
            CreateMap<VendorEditDto, Vendor>().ReverseMap();
            CreateMap<BIDRequestDto, BIDRequest>().ReverseMap();
            CreateMap<BIDRequestDto, BIDRequestDto>().ReverseMap().ForMember(m=>m.BIDComparisons,op=>op.Ignore());
            CreateMap<BIDReferanceDto, BIDReferance>().ReverseMap();
            CreateMap<BIDComparisonDto, BIDComparison>().ReverseMap();
            CreateMap<BIDComparisonDto, BIDComparisonDto>().ReverseMap().ForMember(m => m.BIDReferances, op => op.Ignore()) ;
            CreateMap<RELComparisonRequestItemDto, RELComparisonRequestItem>().ReverseMap();
            CreateMap<StatusDto, Status>().ReverseMap();
            // CreateMap<StatusDto, StatusDto>().ReverseMap().ForMember(x => x.BIDReferances, opt => opt.Ignore());
            CreateMap< WonStatusDto, WonStatus>().ReverseMap();
            // CreateMap<WonStatusDto, WonStatusDto>().ReverseMap().ForMember(x => x.BIDReferances, opt => opt.Ignore());
            CreateMap<RELComparisonRequestItemDto, RELComparisonRequestItemDto>().ReverseMap().ForMember(x => x.BIDReferance, opt => opt.Ignore());
            CreateMap<BIDAttachmentDto, BIDAttachment>().ReverseMap();
            CreateMap<BIDAttachmentDto, BIDAttachmentDto>().ReverseMap().ForMember(x => x.BIDReferance, opt => opt.Ignore());
            CreateMap<ComparisonChartSingleSourceReasonDto, ComparisonChartSingleSourceReason>().ReverseMap();
            CreateMap<ComparisonChartMainListData, ComparisonChartMainListData>().ReverseMap();
            CreateMap<PurchaseRegisterDto, SourceRegisterDto>().ReverseMap();
            CreateMap<AllRegisterDto, SourceRegisterDto>().ReverseMap();
            CreateMap<AllRegisterDto, PurchaseRegisterDto>().ReverseMap();

        }
    }
}
