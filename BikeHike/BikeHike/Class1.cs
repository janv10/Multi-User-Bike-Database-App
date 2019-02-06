using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeHike
{
    class RentalCart
    {
        public string customerID { get; }
        public string bikeID { get; }
        public string hours { get; }

        public RentalCart(string c, string b, string h)
        {
            customerID = c;
            bikeID = b;
            hours = h; 
        }

    }

}
