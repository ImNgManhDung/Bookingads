using System.Configuration;
using System.Data.SqlClient;

namespace BookingAds.Modules
{
    public static class ConnectDB
    {
        public static SqlConnection BookingAdsDB()
        {
            return new SqlConnection(ConfigurationManager.ConnectionStrings["BookingAdsDB"].ConnectionString);
        }
    }
}