using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserLes.Model
{
    class Deal
    {
        public Deal(string dealNumber, string sellerName, string sellerInn, string buyerName, string buyerInn, DateTimeOffset? dealDate, float? woodVolumeBuyer, float? woodVolumeSeller)
        {
            DealNumber = dealNumber;
            SellerName = sellerName;
            SellerInn = sellerInn;
            BuyerName = buyerName;
            BuyerInn = buyerInn;
            DealDate = dealDate;
            WoodVolumeBuyer = woodVolumeBuyer;
            WoodVolumeSeller = woodVolumeSeller;
        }

        public string DealNumber { get; set; }
        public string SellerName { get; set; }
        public string SellerInn { get; set; }
        public string BuyerName { get; set; }
        public string BuyerInn { get; set; }
        public DateTimeOffset? DealDate { get; set; }
        public float? WoodVolumeBuyer { get; set; }
        public float? WoodVolumeSeller { get; set; }
        
    }


    class AllDeals
    {
        public List<Deal> content { get; set; }

    }

    class JSONData
    {
        public int ID { get; set; }
        public string Data { get; set; }
    }

    


}
