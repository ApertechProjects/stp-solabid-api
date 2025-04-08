using Microsoft.EntityFrameworkCore;
using SolaBid.Domain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using static SolaBid.Domain.EntityConfigurations.BIDRequestConf;

namespace SolaBid.Domain.EntityConfigurations
{
    public class BuildConfigurations
    {
        public void ApplyConfigurationsFromAssembly(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(typeof(AppUserConfs).Assembly); 
            builder.ApplyConfigurationsFromAssembly(typeof(LayoutConfs).Assembly); 
            builder.ApplyConfigurationsFromAssembly(typeof(AppRoleConfs).Assembly);
            builder.ApplyConfigurationsFromAssembly(typeof(ParentMenuConfs).Assembly);
            builder.ApplyConfigurationsFromAssembly(typeof(SubMenuConfs).Assembly);
            builder.ApplyConfigurationsFromAssembly(typeof(SiteConfs).Assembly);
            builder.ApplyConfigurationsFromAssembly(typeof(ApproveRoleConfs).Assembly);
            builder.ApplyConfigurationsFromAssembly(typeof(ApproveStageDetailConfs).Assembly);
            builder.ApplyConfigurationsFromAssembly(typeof(GroupSiteWarehouseConfs).Assembly);
            builder.ApplyConfigurationsFromAssembly(typeof(ApproveStageMainConfs).Assembly);
            builder.ApplyConfigurationsFromAssembly(typeof(AdditionalPrivilegeConfs).Assembly);
            builder.ApplyConfigurationsFromAssembly(typeof(VendorConfs).Assembly);
            builder.ApplyConfigurationsFromAssembly(typeof(StatusConf).Assembly);
            builder.ApplyConfigurationsFromAssembly(typeof(ApproveStatusConf).Assembly);
            builder.ApplyConfigurationsFromAssembly(typeof(WonStatusConf).Assembly);
            builder.ApplyConfigurationsFromAssembly(typeof(BIDRequestConf).Assembly);
            builder.ApplyConfigurationsFromAssembly(typeof(BIDReferanceConf).Assembly);
            builder.ApplyConfigurationsFromAssembly(typeof(BIDComparisonConf).Assembly);
            builder.ApplyConfigurationsFromAssembly(typeof(RELComparisonRequestItemConf).Assembly);
            builder.ApplyConfigurationsFromAssembly(typeof(BIDAttachmentConfs).Assembly);
            builder.ApplyConfigurationsFromAssembly(typeof(VendorAttachmentConfs).Assembly);
            builder.ApplyConfigurationsFromAssembly(typeof(ComparisonChartConf).Assembly);
            builder.ApplyConfigurationsFromAssembly(typeof(ComparisonChartSingleSourceReasonConf).Assembly);
            builder.ApplyConfigurationsFromAssembly(typeof(ComparisonChartApproveStagesConfs).Assembly);
            builder.ApplyConfigurationsFromAssembly(typeof(ComparisonChartApprovalBaseInfosConfs).Assembly);
            builder.ApplyConfigurationsFromAssembly(typeof(ComparisonChartChatUserLastViewConf).Assembly);
            builder.ApplyConfigurationsFromAssembly(typeof(ComparisonChartRejectConfs).Assembly);
            builder.ApplyConfigurationsFromAssembly(typeof(DiscountTypeConfs).Assembly);
            builder.ApplyConfigurationsFromAssembly(typeof(ErrorLogConfs).Assembly);
        }


        public void ApplyManyToManyRelations(ModelBuilder builder)
        {
            builder.Entity<GroupApproveRole>()
                .HasKey(bc => new { bc.AppRoleId, bc.ApproveRoleId });

            builder.Entity<GroupApproveRole>()
                .HasOne(bc => bc.AppRole)
                .WithMany(b => b.GroupApproveRoles)
                .HasForeignKey(bc => bc.AppRoleId);

            builder.Entity<GroupApproveRole>()
                .HasOne(bc => bc.ApproveRole)
                .WithMany(c => c.GroupApproveRoles)
                .HasForeignKey(bc => bc.ApproveRoleId);


            builder.Entity<GroupAdditionalPrivilege>()
                .HasKey(m => new { m.AdditionalPrivilegeId, m.AppRoleId });
            builder.Entity<GroupAdditionalPrivilege>()
                .HasOne(ap => ap.AdditionalPrivilege)
                .WithMany(ap => ap.GroupAdditionalPrivileges)
                .HasForeignKey(ap => ap.AdditionalPrivilegeId);
            builder.Entity<GroupAdditionalPrivilege>()
                .HasOne(ar => ar.AppRole)
                .WithMany(ar => ar.GroupAdditionalPrivileges)
                .HasForeignKey(ar => ar.AppRoleId);


            builder.Entity<ApproveRoleApproveStageDetail>()
                .HasKey(aras => aras.Id);
            builder.Entity<ApproveRoleApproveStageDetail>()
                .HasOne(ar => ar.ApproveRole)
                .WithMany(ar => ar.ApproveRoleApproveStageDetails)
                .HasForeignKey(ar => ar.ApproveRoleId);
            builder.Entity<ApproveRoleApproveStageDetail>()
                .HasOne(aps => aps.ApproveStageDetail)
                .WithMany(aps => aps.ApproveRoleApproveStageDetails)
                .HasForeignKey(aps => aps.ApproveStageDetailId);
        }
    }
}
