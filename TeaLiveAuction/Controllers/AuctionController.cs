using Dapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System.Buffers;
using System.Collections.Generic;
using System.Data;
using TeaLiveAuction.DBConnection;
using TeaLiveAuction.Hubs;
using TeaLiveAuction.Models;

namespace TeaLiveAuction.Controllers
{
    public class AuctionController : Controller
    {
        private IConfiguration _configuration;
        private readonly DapperContext _dapperContext;
        private readonly IHubContext<AuctionHub> _hubContext;
        public AuctionController(IConfiguration configuration, DapperContext dapperContext, IHubContext<AuctionHub> hubContext)
        {
            _configuration = configuration;
            _dapperContext = dapperContext;
            _hubContext = hubContext;
        }
        public async Task<IActionResult> Index(string searchLotNo)
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                try
                {
                    searchLotNo = string.IsNullOrEmpty(searchLotNo) ? TempData["SearchLotNo"] != null ? TempData["SearchLotNo"].ToString(): "" : searchLotNo;

                    var parameters1 = new DynamicParameters();
                    parameters1.Add("@flag", 2);
                    //var lotNos = await connection.QueryAsync<string>("SP_Get_DashboardDetails_Demo", commandType: CommandType.StoredProcedure, param: parameters1);
                    var lotNos = await connection.QueryAsync<string>("[dbo].[SP_Get_DashboardDetails_Dev_Nandkishan]", commandType: CommandType.StoredProcedure, param: parameters1);
                    if (lotNos == null || !lotNos.Any())
                    {
                        ViewData["lotNos"] = new List<string>(); // Empty list to prevent null error
                    }
                    else
                    {
                        ViewData["lotNos"] = lotNos.ToList(); // Assign the result to ViewData["lotNos"]
                    }

                    var parameters2 = new DynamicParameters();
                    string[] parts = searchLotNo.Split('-');
                    string lotNoPart = parts.Length > 0 ? parts[0] : searchLotNo;
                    parameters2.Add("@searchLotNo", lotNoPart);
                    parameters2.Add("@flag", 1);
                    //var lotDetails = await connection.QueryAsync<BidderLotDetailRequest>("SP_Get_DashboardDetails_Demo", commandType: CommandType.StoredProcedure, param: parameters2);
                    var lotDetails = await connection.QueryAsync<BidderLotDetailRequest>("[dbo].[SP_Get_DashboardDetails_Dev_Nandkishan]", commandType: CommandType.StoredProcedure, param: parameters2);
                    return View(lotDetails);
                }
                catch (Exception ex)
                {
                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
        }
        [HttpPost]
        public async Task<IActionResult> ConfirmBid(BidderLotDetailRequest rowData)
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                try
                {
                    TempData["SearchLotNo"] = rowData.LotNo;
                    var parameters = new DynamicParameters();
                    parameters.Add("@auctionLotDetailId", rowData.auctionLotDetailId);
                    parameters.Add("@buyerUserId", rowData.buyerUserId);
                    parameters.Add("@buyerCode", rowData.buyerCode);
                    parameters.Add("@BQ", rowData.buyingQty);
                    parameters.Add("@autoBidLimit", rowData.AutoBidLimit);
                    parameters.Add("@limitPrice", rowData.LimitPrice);
                    parameters.Add("@BP", rowData.bidPrice);
                    parameters.Add("@auctionCenterId", 10100);

                    var execution = "EXEC [dbo].[SP_Set_BidderRank_Dev_Nandkishan]";

                    foreach (var item in parameters.ParameterNames)
                    {
                        var value = parameters.Get<dynamic>(item);

                        string formattedValue = value is string ? $"'{value}'" : value?.ToString() ?? "NULL";

                        execution += $@" {item}={formattedValue},";
                    }

                    execution = execution.TrimEnd(',');

                    var lotDetails = await connection.QueryAsync("[dbo].[SP_Set_BidderRank_Dev_Nandkishan]", commandType: CommandType.StoredProcedure, param: parameters);
                    await _hubContext.Clients.All.SendAsync("ReceiveHBPUpdate");
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
            return RedirectToAction("Index"); // Or return a view/JSON as needed
        }
        [HttpPost]
        public async Task<IActionResult> ClearData(string SearchLotNo)
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                try
                {
                    TempData["SearchLotNo"] = SearchLotNo;
                    var parameters1 = new DynamicParameters();
                    parameters1.Add("@flag", 3);
                    //var clearData = await connection.QueryAsync<string>("SP_Get_DashboardDetails_Demo", commandType: CommandType.StoredProcedure, param: parameters1);
                    var clearData = await connection.QueryAsync<string>("[dbo].[SP_Get_DashboardDetails_Dev_Nandkishan]", commandType: CommandType.StoredProcedure, param: parameters1);
                }
                catch (Exception ex)
                {
                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
            return RedirectToAction("Index"); // Or return a view/JSON as needed
        }
        [HttpGet]
        public async Task<IActionResult> gettest()
        {
            using (var connection = _dapperContext.CreateConnection())
            {
                try
                {
                    await _hubContext.Clients.All.SendAsync("ReceiveHBPUpdate");
                    return Ok();
                }
                catch (Exception ex)
                {
                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
        }
    }
}
