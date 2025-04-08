using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SolaBid.Domain.EntityConfigurations;
using SolaBid.Domain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using SolaBid.Domain.Models;

namespace SolaBid.Domain.Models.AppDbContext
{
    public class SBDbContext : IdentityDbContext<AppUser, AppRole, string,
        IdentityUserClaim<string>, AspNetUserRoles, IdentityUserLogin<string>,
        IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        public SBDbContext()
        {
        }

        public SBDbContext(DbContextOptions<SBDbContext> options) : base(options)
        {
        }

        #region Setters

        public DbSet<ParentMenu> ParentMenus { get; set; }
        public DbSet<SubMenu> SubMenus { get; set; }
        public DbSet<GroupMenu> GroupMenus { get; set; }
        public DbSet<Site> Sites { get; set; }
        public DbSet<Sequence> Sequences { get; set; }
        public DbSet<GroupSiteWarehouse> GroupSiteWarehouses { get; set; }
        public DbSet<ApproveRole> ApproveRoles { get; set; }
        public DbSet<ApproveStageDetail> ApproveStageDetails { get; set; }
        public DbSet<ApproveStageMain> ApproveStageMains { get; set; }
        public DbSet<GroupBuyer> GroupBuyers { get; set; }
        public DbSet<GroupApproveRole> GroupApproveRoles { get; set; }
        public DbSet<ApproveRoleApproveStageDetail> ApproveRoleApproveStageDetails { get; set; }
        public DbSet<AdditionalPrivilege> AdditionalPrivileges { get; set; }
        public DbSet<GroupAdditionalPrivilege> GroupAdditionalPrivileges { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<ApproveStatus> ApproveStatuses { get; set; }
        public DbSet<WonStatus> WonStatuses { get; set; }
        public DbSet<BIDRequest> BIDRequests { get; set; }
        public DbSet<BIDComparison> BIDComparisons { get; set; }
        public DbSet<RELComparisonRequestItem> RELComparisonRequestItems { get; set; }
        public DbSet<BIDReferance> BIDReferances { get; set; }
        public DbSet<BIDAttachment> BIDAttachments { get; set; }
        public DbSet<VendorAttachment> VendorAttachments { get; set; }
        public DbSet<ComparisonChart> ComparisonCharts { get; set; }
        public DbSet<ComparisonChartSingleSourceReason> ComparisonChartSingleSourceReasons { get; set; }
        public DbSet<ComparisonChartApproveStage> ComparisonChartApproveStages { get; set; }
        public DbSet<ComparisonChartApprovalBaseInfos> ComparisonChartApprovalBaseInfos { get; set; }
        public DbSet<RELComparisonChartSingleSource> RELComparisonChartSingleSources { get; set; }
        public DbSet<ComparisonChartReject> ComparisonChartRejects { get; set; }
        public DbSet<ComparisonChartHold> ComparisonChartHolds { get; set; }
        public DbSet<DiscountType> DiscountTypes { get; set; }
        public DbSet<ErrorLog> ErrorLogs { get; set; }
        public DbSet<ComparisonChartChat> ComparisonChartChats { get; set; }
        public DbSet<ComparisonChartChatUserLastView> ComparisonChartChatUserLastViews { get; set; }
        public DbSet<Layout> Layouts { get; set; }

        #endregion

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(Statics.ConnectionString());
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            new BuildConfigurations().ApplyConfigurationsFromAssembly(builder);
            new BuildConfigurations().ApplyManyToManyRelations(builder);
        }
    }
}