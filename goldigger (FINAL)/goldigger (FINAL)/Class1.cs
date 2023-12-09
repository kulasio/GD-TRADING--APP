using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GD_TRADING_APP_beta_
{
    // Define CryptocurrencyInfo class
    public class CryptocurrencyInfo
    {
        public int cryptocurrency_id { get; set; }
        public string CoinSymbol { get; set; }
        public double CostPerCoin { get; set; }
    }


    // UserManager.cs

    public class UserManager
    {
        

        private static UserManager _instance;

        private UserManager() { }

        public static UserManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new UserManager();
                }
                return _instance;
            }
        }

        public int CurrentUserID { get; private set; }

        public void SetCurrentUser(int userID)
        {
            CurrentUserID = userID;
        }
    }

}
