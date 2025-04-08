using AutoMapper;
using Microsoft.Data.SqlClient;
using SolaBid.Business.Mappings;
using SolaBid.Domain.Models.AppDbContext;
using SolaBid.Domain.Models;

namespace SolaBid.Business.Models
{
    public static class TransactionConfig
    {
        private static Mapper mapper = new Mapper(new MapperConfiguration((x) => x.AddProfile(new MappingProfile())));
        public static string SiteDatabase { get; set; }

        public static Mapper Mapper
        {
            get { return mapper; }
        }

        public static SBDbContext AppDbContext => new();

        public static SqlConnection AppDbContextManualConnection => new(Statics.ConnectionString());

        public static SqlConnection ExternalDbContext => new(Statics.ConnectionString(SiteDatabase));
    }
}