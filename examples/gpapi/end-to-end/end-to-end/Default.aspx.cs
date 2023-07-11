using GlobalPayments.Api;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace end_to_end
{
    public partial class _Default : Page
    {
        public const string APP_ID = "UJqPrAhrDkGzzNoFInpzKqoI8vfZtGRV";
        public const string APP_KEY = "zCFrbrn0NKly9sB4";
        public static string accessToken;
        protected void Page_Load(object sender, EventArgs e)
        {
            GenerateToken();
        }

        private void GenerateToken()
        {
            GpApiConfig config = new GpApiConfig();
            config.AppId = APP_ID;
            config.AppKey = APP_KEY;
            config.Channel = Channel.CardNotPresent;
            config.Permissions = new string[] { "PMT_POST_Create_Single" };           

            var accessTokenInfo = GpApiService.GenerateTransactionKey(config);
            accessToken = accessTokenInfo.Token;
        }       

        public string GetAccessToken()
        {
            return accessToken;
        }
    }
}