using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SolaBid.Business.ConnectableEntityExtensions;
using SolaBid.Business.Dtos.ApiDtos;
using SolaBid.Business.Dtos.EntityDtos;
using SolaBid.Business.Models;
using SolaBid.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SolaBid.Business.Logics.CommonLogic.GetData;

namespace SolaBid.Business.Logics
{
    public class SiteLineDbLogic
    {
        public SiteLineDbLogic(string siteDatabase)
        {
            TransactionConfig.SiteDatabase = siteDatabase;
        }

        public List<BuyerDto> GetBuyers()
        {
            var buyerDtoList = new List<BuyerDto>();
            var buyerTable = FromQuery("select UserId Id,Username from dbo.UserNames where WorkstationLogin='PROCUREMENT'");
            buyerDtoList = buyerTable.AsEnumerable()
                .Select(row => new BuyerDto
                {
                    Id = row.Field<decimal>("Id"),
                    Username = row.Field<string>("Username")
                }).ToList();
            return buyerDtoList;
        }

        public List<KeyValueTextBoxingDto> GetCountries(string siteName)
        {
            var countryList = new List<KeyValueTextBoxingDto>();
            var countryTable = FromQuery($"SELECT country FROM country_mst = '{siteName}'");
            for (int i = 0; i < countryTable.Rows.Count; i++)
            {
                countryList.Add(new KeyValueTextBoxingDto
                {
                    Key = i.ToString(),
                    Value = countryTable.Rows[i]["country"].ToString(),
                    Text = countryTable.Rows[i]["country"].ToString()
                });
            }
            return countryList;
        }

        public List<KeyValueTextBoxingDto> GetDeliveryTerms(string siteName)
        {
            var delTermList = new List<KeyValueTextBoxingDto>();
            var delTermTable = FromQuery($"SELECT delterm,description FROM del_term_mst where site_ref = '{siteName}'");
            for (int i = 0; i < delTermTable.Rows.Count; i++)
            {
                var rowData = delTermTable.Rows[i];
                delTermList.Add(new KeyValueTextBoxingDto
                {
                    Key = i.ToString(),
                    Value = rowData["delterm"].ToString(),
                    Text = $"{rowData["delterm"]} - {rowData["description"]}"
                });
            }
            return delTermList;
        }

        public List<KeyValueTextBoxingDto> GetPaymentTerms(string siteName)
        {
            var payTermList = new List<KeyValueTextBoxingDto>();
            var payTermTable = FromQuery($"SELECT terms_code,description FROM terms_mst where site_ref = '{siteName}'");
            for (int i = 0; i < payTermTable.Rows.Count; i++)
            {
                var rowData = payTermTable.Rows[i];
                payTermList.Add(new KeyValueTextBoxingDto
                {
                    Key = i.ToString(),
                    Value = rowData["terms_code"].ToString(),
                    Text = $"{rowData["terms_code"]} - {rowData["description"]}"
                });
            }
            return payTermList;
        }

        public List<KeyValueTextBoxingForUOMDto> Get_UOM_Items(string rowPointer, string UOM, string siteName)
        {
            var payTermList = new List<KeyValueTextBoxingForUOMDto>();
            var payTermTable = FromQuery($"Exec dbo.SP_ITEM_UOM @RowPointer = '{rowPointer}',@from_uom ='{UOM}', @warehouse = '{siteName}'", false);
            for (int i = 0; i < payTermTable.Rows.Count; i++)
            {
                var rowData = payTermTable.Rows[i];
                payTermList.Add(new KeyValueTextBoxingForUOMDto
                {
                    Key = i.ToString(),
                    Value = rowData["u_m"].ToString().Trim(),
                    Text = $"{rowData["u_m"].ToString().Trim()}     {rowData["description"].ToString().Trim()} {rowData["conv_factor"].ToString().Trim()}",
                    Conv = decimal.Parse(rowData["conv_factor"].ToString())
                });
            }
            return payTermList;
        }

        public bool CheckValutaIsFilled(string siteName)
        {
            var valuta = GetCurrencyConvertingAZN("AZN", 10, default, siteName);
            var res = valuta._USD != 0;
            return res;
        }

        public List<KeyValueTextBoxingDto> GetTaxCode(string siteName)
        {
            var taxCodeList = new List<KeyValueTextBoxingDto>();
            var taxCodeTable = FromQuery($"SELECT tm.tax_code,tm.description FROM taxcode_mst tm where site_ref = '{siteName}'");
            for (int i = 0; i < taxCodeTable.Rows.Count; i++)
            {
                var rowData = taxCodeTable.Rows[i];
                taxCodeList.Add(new KeyValueTextBoxingDto
                {
                    Key = i.ToString(),
                    Value = rowData["tax_code"].ToString(),
                    Text = $"{rowData["tax_code"]} - {rowData["description"]}"
                });
            }
            return taxCodeList;
        }

        public List<KeyValueTextBoxingDto> GetCurrency(string siteName)
        {
            var currencyList = new List<KeyValueTextBoxingDto>();
            var currencyTable = FromQuery($"SELECT curr_code FROM currency_mst where site_ref = '{siteName}'");
            for (int i = 0; i < currencyTable.Rows.Count; i++)
            {
                var rowData = currencyTable.Rows[i];
                currencyList.Add(new KeyValueTextBoxingDto
                {
                    Key = i.ToString(),
                    Value = rowData["curr_code"].ToString(),
                    Text = $"{rowData["curr_code"]}"
                });
            }
            return currencyList;
        }

        public List<KeyValueTextBoxingDto> GetBankCode(string currency, string siteName)
        {
            var bankCodeList = new List<KeyValueTextBoxingDto>();
            var bankCodeTable = FromQuery($"Select bank_code from bank_hdr_mst where curr_code = '{currency}' and site_ref = '{siteName}'");
            for (int i = 0; i < bankCodeTable.Rows.Count; i++)
            {
                var rowData = bankCodeTable.Rows[i];
                bankCodeList.Add(new KeyValueTextBoxingDto
                {
                    Key = i.ToString(),
                    Value = rowData["bank_code"].ToString(),
                    Text = $"{rowData["bank_code"]}"
                });
            }
            return bankCodeList;
        }

        public async Task<List<BuyerDto>> GetGroupBuyersWithAll(string groupId)
        {
            var buyerDtoList = GetBuyers();
            using (var context = TransactionConfig.AppDbContext)
            {
                var groupBuyers = await context.GroupBuyers.Where(m => m.AppRoleId == groupId).ToListAsync();
                if (groupBuyers.Count > 0)
                {
                    foreach (var buyer in buyerDtoList)
                    {
                        if (groupBuyers.Any(m => m.BuyerId == buyer.Id))
                        {
                            buyer.IsSelected = true;
                        }
                    }
                }
            }
            return buyerDtoList;
        }

        public async Task<List<BuyerDto>> GetUserBuyersWithAll(string userId)
        {
            var buyerDtoList = GetBuyers();
            using (var context = TransactionConfig.AppDbContext)
            {
                var userBuyerEntity = await context.Users.FindAsync(userId);
                if (userBuyerEntity.BuyerId != null && userBuyerEntity.BuyerId.Length > 0)
                {
                    foreach (var buyer in buyerDtoList)
                    {
                        if (buyer.Id.ToString() == userBuyerEntity.BuyerId)
                        {
                            buyer.IsSelected = true;
                        }
                    }
                }
            }
            return buyerDtoList;
        }

        public List<WarehouseDto> GetWarehousesBySite(string siteName, int siteId = 0)//if site contains exist QQZ return whse like QQZ , otherwise return all whse without QQZ 
        {
            List<WarehouseDto> warehouseDtos = new List<WarehouseDto>();
            DataTable warehouseTable = new DataTable();

            warehouseTable = FromQuery($"SELECT whse AS Warehouse, name AS Name FROM dbo.whse_mst WHERE site_ref = '{siteName}'");

            // if (siteName.Contains("QQZ"))
            // {
            //     warehouseTable = FromQuery("SELECT whse AS Warehouse, name AS Name FROM dbo.whse_mst WHERE whse LIKE N'QQZ%'");
            // }
            // else
            // {
            //     warehouseTable = FromQuery("SELECT whse AS Warehouse, name AS Name FROM dbo.whse_mst WHERE whse NOT LIKE N'QQZ%'");
            // }

            for (int i = 0; i < warehouseTable.Rows.Count; i++)
            {
                warehouseDtos.Add(new WarehouseDto
                {
                    Id = i,
                    Name = warehouseTable.Rows[i].Field<string>("Name"),
                    Site = siteName,
                    Warehouse = warehouseTable.Rows[i].Field<string>("Warehouse"),
                    SiteId = siteId
                });
            }
            return warehouseDtos;
        }

        public List<VendorDto> GetSiteLineVendors(string userId, string siteName)
        {
            string query = $"select vd.vend_num, va.name,vd.UfVendorsBlackList, va.addr##1 ,va.addr##2,va.addr##3,vd.contact, vd.phone, va.external_email_addr, va.country,vd.tax_reg_num1, tax_code1, vd.delterm,vd.terms_code, vd.curr_code, vd.bank_code from vendor_mst vd inner join vendaddr_mst va on vd.vend_num = va.vend_num and vd.site_ref=va.site_ref where vd.site_ref='{siteName}'";
            var siteLineVendors = new List<VendorDto>();
            var siteLineVendorTable = FromQuery(query);

            siteLineVendors = siteLineVendorTable.AsEnumerable().Select(m => new VendorDto
            {
                VendorCode = m.Field<string>("vend_num").Trim(),
                VendorName = m.Field<string>("name") ?? "-",
                Address1 = m.Field<string>("addr##1") ?? "-",
                Address2 = m.Field<string>("addr##2") ?? "-",
                Address3 = m.Field<string>("addr##3") ?? "-",
                BankCode = m.Field<string>("bank_code") ?? "",
                Contact = m.Field<string>("contact") ?? "-",
                Country = m.Field<string>("country") ?? "-",
                Currency = m.Field<string>("curr_code") ?? "",
                DeliveryTerm = m.Field<string>("delterm") ?? "",
                ExternalEmail = m.Field<string>("external_email_addr") ?? "-",
                PaymentTerm = m.Field<string>("terms_code") ?? "",
                Phone = m.Field<string>("phone") ?? "-",
                TaxCode = m.Field<string>("tax_code1") ?? "",
                TaxId = m.Field<string>("tax_reg_num1") ?? "",
                VendorBlackList = Convert.ToBoolean(m.Field<byte?>("UfVendorsBlackList")),
                CreatedBy = userId,
                LastUpdateBy = userId,
                CreatedDate = DateTime.Now,
                EditDate = DateTime.Now
            }).ToList();


            return siteLineVendors;
        }

        public List<string> GetSiteLineVendorCodes(string siteName)
        {
            string query = $"EXEC dbo.SP_VendorList '{siteName}'";
            var siteLineVendorCodes = new List<string>();
            var siteLineVendorTable = FromQuery(query);

            for (int i = 0; i < siteLineVendorTable.Rows.Count; i++)
            {
                siteLineVendorCodes.Add(siteLineVendorTable.Rows[i]["vend_num"].ToString());
            }

            return siteLineVendorCodes;
        }
        //BID Informations Start
        public List<string> GetRequestNumbers(string siteName, string userId)
        {
            var numberList = new List<string>();
            DataTable requestNumberTable = FromQuery($"Exec dbo.SP_RequestsBySite @UserId = '{userId}',@BU='{siteName}'", false);
            for (int i = 0; i < requestNumberTable.Rows.Count; i++)
            {
                numberList.Add(requestNumberTable.Rows[i].Field<string>("req_num"));
            }
            return numberList;
        }

        public string GetPONumberByBIDReference(string bidReference, string siteName)
        {
            var resultTable = new DataTable();
            var result = string.Empty;
            resultTable = FromQuery($"Select po_num from po_mst where site_ref= '{siteName}' AND UfBIDReference = '{bidReference}'");
            result = resultTable.Rows[0][0].ToString();
            return result;
        }

        public string GetRequester(string siteName, string reqNumber)
        {
            var resultTable = new DataTable();
             resultTable = FromQuery($"Select requester from preq_mst WHERE req_num= '{reqNumber} and site_ref = '{siteName}'");
            // if (siteName.Contains("QQZ"))
            // {
            //     resultTable = FromQuery($"Select requester from preq_mst WHERE req_num= '{reqNumber}' AND whse LIKE N'QQZ%'");
            // }
            // else
            // {
            //     resultTable = FromQuery($"Select requester from preq_mst WHERE req_num= '{reqNumber}' AND whse NOT LIKE N'QQZ%'");
            // }
            try
            {
                return resultTable.Rows[0][0].ToString();
            }
            catch (Exception ex)
            {
                return " ";
            }
        }

        public List<KeyValueTextBoxingDto> GetRequestersAsKeyValue(string siteName)
        {
            var resultTable = FromQuery($"SELECT distinct requester from preq_mst WHERE requester IS NOT NULL AND site_ref='{siteName}'");
            return resultTable.AsEnumerable().Select(m => new KeyValueTextBoxingDto
            {
                Key = m.Field<string>("requester"),
                Text = m.Field<string>("requester"),
                Value = m.Field<string>("requester")
            }).ToList();
        }

        public string GetRequestDate(string siteName, string reqNumber)
        {
            var resultTable = new DataTable();
            var result = string.Empty;
            resultTable = FromQuery($"Select req_date from preq_mst WHERE req_num = '{reqNumber}' AND site_ref='{siteName}'");
            // if (siteName.Contains("QQZ"))
            // {
            //     resultTable = FromQuery($"Select req_date from preq_mst WHERE req_num = '{reqNumber}' AND whse LIKE N'QQZ%'");
            // }
            // else
            // {
            //     resultTable = FromQuery($"Select req_date from preq_mst WHERE req_num = '{reqNumber}' AND whse NOT LIKE N'QQZ%'");
            // }
            try
            {
                result = resultTable.Rows[0][0].ToString();
                return result;
            }
            catch (Exception)
            {
                return null;
            }

        }

        public string GetDestination(string siteName, string reqNumber)
        {
            var resultTable = new DataTable();
            var result = string.Empty;
            resultTable = FromQuery($"SELECT DISTINCT CASE WHEN REF_TYPE = 'I' THEN 'Inventory' WHEN REF_TYPE = 'J' THEN 'JOB' + ' - ' + ref_num WHEN REF_TYPE = 'O' THEN 'ORDER' + ' - ' + ref_num END Destination FROM preqitem_mst pm WHERE req_num = '{reqNumber}' AND pm.site_ref='{siteName}'");
            
            // if (siteName.Contains("QQZ"))
            // {
            // }
            // else
            // {
            //     resultTable = FromQuery($"SELECT DISTINCT CASE WHEN REF_TYPE = 'I' THEN 'Inventory' WHEN REF_TYPE = 'J' THEN 'JOB' + ' - ' + ref_num WHEN REF_TYPE = 'O' THEN 'ORDER' + ' - ' + ref_num END Destination FROM preqitem_mst pm WHERE req_num = '{reqNumber}' AND whse <> 'QQZ'");
            // }
            try
            {
                result = resultTable.Rows[0][0].ToString();
                return result;
            }
            catch (Exception)
            {
                return " ";
            }
        }

        public string GetOR(string siteName, string destination)
        {
            var resultTable = new DataTable();

            if (destination.ToLower().Contains("job"))
            {

                resultTable = FromQuery($"SELECT TOP 1 cm.co_num FROM co_mst cm WHERE cm.site_ref='{siteName}' AND cm.est_num='{destination.Split("-")[1].Trim()}'");
            }
            else
            {
                return " ";
            }
            try
            {
                return resultTable.Rows[0][0].ToString();

            }
            catch (Exception)
            {
                return " ";
            }
        }

        public List<BIDRequestItemDto> GetRequestLines(string siteName, string reqNumber, int getAllItems = 1) //0 = Only 'A' items ; 1=AllItems
        {
            var resultList = new List<BIDRequestItemDto>();
            DataTable requestNumberTable = FromQuery($"Exec dbo.SP_RequestLinesForBid @req_num = '{reqNumber}',@warehouse ='{siteName}',@status = {getAllItems}", false);
            try
            {
                resultList = requestNumberTable.AsEnumerable().Select(m => new BIDRequestItemDto
                {
                    Budget = m["budget"] == DBNull.Value ? 0 : m.Field<decimal>("budget"),
                    ItemCode = m["item"] == DBNull.Value ? " " : m.Field<string>("item"),
                    RowPointer = m.Field<Guid>("RowPointer").ToString(),
                    RefType = m.Field<string>("ref_type"),
                    ItemName = m["ItemName"] == DBNull.Value ? " " : m.Field<string>("ItemName"),
                    Quantity = m["Quantity"] == DBNull.Value ? 0 : m.Field<decimal>("Quantity"),
                    RequestLine = m["req_line"] == DBNull.Value ? short.Parse("0") : m.Field<Int16>("req_line"),
                    UOM = m.Field<string>("UOM"),
                    UOMItems = Get_UOM_Items(m.Field<Guid>("RowPointer").ToString(), m.Field<string>("UOM"), siteName)
                }).ToList();

                for (int i = 0; i < resultList.Count; i++)
                {
                    resultList[i].UOMItems.Insert(0, new KeyValueTextBoxingForUOMDto { Key = Guid.NewGuid().ToString(), Text = resultList[i].UOM, Value = resultList[i].UOM, Conv = 1 });
                    resultList[i].BidLine = i + 1;
                }
            }
            catch (Exception ex)
            {
                _ = ex.ErrorLog();
            }
            return resultList;
        }

        public string GetProjectWarehouse(string siteName, string reqNumber)
        {
            var resultTable = new DataTable();
            var result = string.Empty;
            resultTable = FromQuery($"Select whse + ' - ' + (Select name from whse_mst where whse = (Select whse from preq_mst where req_num = '{reqNumber}' and site_ref = '{siteName}') and site_ref = '{siteName}' )  from preq_mst where req_num = '{reqNumber}'");
            // if (siteName.Contains("QQZ"))
            // {
            //     resultTable = FromQuery($"Select whse + ' - ' + (Select name from whse_mst where whse = (Select whse from preq_mst where req_num = '{reqNumber}'))  from preq_mst where req_num = '{reqNumber}' AND whse LIKE N'QQZ%'");
            // }
            // else
            // {
            //     resultTable = FromQuery($"Select whse + ' - ' + (Select name from whse_mst where whse = (Select whse from preq_mst where req_num = '{reqNumber}'))  from preq_mst where req_num = '{reqNumber}' AND whse NOT LIKE N'QQZ%'");
            // }
            try
            {
                result = resultTable.Rows[0][0].ToString();
                return result;
            }
            catch (Exception)
            {
                return " ";
            }

        }

        public ValConvertorDto GetCurrencyConvertingAZN(string currency, decimal value, string date, string siteName)
        {
            var result = new ValConvertorDto();
            if (string.IsNullOrEmpty(currency))
            {
                return result;
            }
            var formattedDate = string.IsNullOrEmpty(date) ? DateTime.Now.ToString("yyyy-MM-dd") : Utility.ConvertHyphenStringToDatetime(date).GetValueOrDefault().ToString("yyyy-MM-dd");
            try
            {
                if (currency == "AZN")
                {
                    result._AZN = value;
                }
                else
                {
                    var resultTableAZN = FromQuery($"SELECT dbo.SF_DailyRate ('{formattedDate}','{currency}', '{siteName}')");
                    var gettedDataAZN = resultTableAZN.Rows[0][0].ToString();
                    var convertedValueAZN = decimal.Parse(gettedDataAZN);

                    if (currency == "RUB")
                        convertedValueAZN /= 100;

					result._AZN = value * convertedValueAZN;
                }


                var resultTableUSD = FromQuery($"SELECT dbo.SF_DailyRate ('{formattedDate}','USD', '{siteName}')");
                var gettedDataUSD = resultTableUSD.Rows[0][0].ToString();
                var convertedValueUSD = decimal.Parse(gettedDataUSD);
                result._USD = result._AZN / convertedValueUSD;
                return result;
            }
            catch (Exception ex)
            {
                _ = ex.ErrorLog();
                return result;
            }

        }
    }
}
