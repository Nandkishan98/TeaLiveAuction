using System.Numerics;

namespace TeaLiveAuction.Models
{
    public class BidderLotDetailRequest
    {
        public long auctionLotDetailId { get; set; }
        public string LotNo { get; set; }
        public decimal? basePrice { get; set; }
        public decimal? reservePrice { get; set; }
        public long? totalPackages { get; set; }
        public int? noOfDivisibleBuyers { get; set; }
        public string buyerCode { get; set; }
        public long buyerUserId { get; set; }
        public long buyingQty { get; set; }
        public decimal? AutoBidLimit { get; set; }
        public decimal? LimitPrice { get; set; }
        public decimal? bidPrice { get; set; }
        public string bidderRank { get; set; }
        public decimal highestBidPrice { get; set; }
        public long soldPackages { get; set; }
        public string indicatedColor { get; set; }
        public string bidSubmissionTime { get; set; }
        public string status { get; set; }
        public long auctionCenterId { get; set; }
    }
}
