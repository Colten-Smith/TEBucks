using Microsoft.IdentityModel.JsonWebTokens;
using RestSharp;
using RestSharp.Authenticators;
using System.Linq.Expressions;
using TEbucksServer.DTOModels;
using TEbucksServer.Models;
using ReturnUser = TEbucksServer.DTOModels.ReturnUser;

namespace TEbucksServer.Services
{
    public class LogTransferService
    {
        private DTOModels.LoginUser model = new DTOModels.LoginUser("ColtenEmelieTEBucks", "CE2207");
        private RestClient client = new RestClient();
        private string TEARSAPI = "https://te-pgh-api.azurewebsites.net/";
        public DTOModels.ReturnUser returnModel;
        public LogTransferService()
        {
            RestRequest request = new RestRequest(TEARSAPI + "api/Login");
            request.AddJsonBody(model);
            IRestResponse<ReturnUser> response = client.Post<ReturnUser>(request);
            if (response.ResponseStatus == ResponseStatus.Completed && response.IsSuccessful)
            {
                returnModel = response.Data;
                client.Authenticator = new JwtAuthenticator(returnModel.token);
            }
            else
            {
                throw new System.Exception();
            }
        }
        public bool LogTransfer(Transfer transferToLog)
        {
            try
            {
                RestRequest request = new RestRequest(TEARSAPI + "api/TxLog");
                request.AddJsonBody(new TxLogDTO(transferToLog));
                IRestResponse<DTOModels.TxLog> response = client.Post<DTOModels.TxLog>(request);
                if (response.ResponseStatus == ResponseStatus.Completed && response.IsSuccessful)
                {
                    return true;
                }
            }
            catch
            {
                throw new System.Exception();
            }
            return false;
        }
    }
}
