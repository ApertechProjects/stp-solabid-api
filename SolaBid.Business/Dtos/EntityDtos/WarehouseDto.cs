using System;
using System.Collections.Generic;
using System.Text;

namespace SolaBid.Business.Dtos.EntityDtos
{
    public class WarehouseDto
    {
        public int Id { get; set; } //This prop only used React.This is not valid Id
        public string Warehouse { get; set; } //this prop is unique and is Id
        public string Name { get; set; }
        public string Site { get; set; }
        public int SiteId { get; set; }
        public bool IsSelected { get; set; }
    }
}
