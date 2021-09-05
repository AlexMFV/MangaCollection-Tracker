using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MangaTrackerDesktop
{
    public class Volume
    {
        private int id;
        private double volPrice;    //Volume Price
        private double shipPrice;   //Shipping Price
        private double addCosts;    //Additional Costs
        private DateTime buyDate;
        private DateTime arrivalDate;
        private string status;

        public Volume()
        {

        }

        public int Id { get => id; set => id = value; }
        public double VolPrice { get => volPrice; set => volPrice = value; }
        public double ShipPrice { get => shipPrice; set => shipPrice = value; }
        public double AddCosts { get => addCosts; set => addCosts = value; }
        public DateTime BuyDate { get => buyDate; set => buyDate = value; }
        public DateTime ArrivalDate { get => arrivalDate; set => arrivalDate = value; }
        public string Status { get => status; set => status = value; }
        //Eventually add bought from {url}
    }
}
