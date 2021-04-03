using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;


// TODO: Add retry logic to the unitywebrequests to add reliability to the payment sending on bad
// network connections. However, do checking to ensure we can't charge multiple times.
namespace GameDevHQ.Scripts.Payment
{
    [Serializable]
    public class PaypalAuthorizationResponse
    {
        public string access_token;
    }

    public class PaypalManager : MonoBehaviour
    {
        [SerializeField] private string _paypalPaymentSandboxAddress =
            "https://api-m.sandbox.paypal.com/v1/payments/payment";

        [SerializeField] private string _paypalTokenSandboxAddress =
            "https://api-m.sandbox.paypal.com/v1/oauth2/token";

        [Header("PayPal payment API information")] [SerializeField]
        private PaypalAuthorizationResponse _paypalAuthResponse;

        [SerializeField] private PaypalPaymentRequest _paypalPaymentRequest;
        [SerializeField] private PaypalPaymentResponse _paypalPaymentResponse;
        [SerializeField] private PaypalExecutePaymentResponse _paypalExecutePaymentResponse;

        [SerializeField] private string _clientID = (
            "AdaowpqzbOR2wA0Nl4xgNjBHq89aAYWjuoBRdY95oy2AAyw70ENw7Cc9a4ppIaGhUVKPFu1shQqwneUZ");

        [SerializeField] private string _clientSecret = (
            // ReSharper disable once StringLiteralTypo
            "EC-ljrMH9KWjqqLYlOmf03SnTxh79L9W-8SLHQN6GjsS4TQePHyWnNZi43BOqTsEZkfK92d48h3y5Y9w");

        private string clientIdAndSecret;
        private string _base64ClientIdAndSecret;



        // TODO:
        // Add ability to add warfunds with paypal payment.
        // Have UI menu pop-up when warfunds are zero to ask if they want to buy more warfunds
        // if they click yes, execute payment for buyer
        // once you confirm it went through add 1000 warfunds to player

        private void Awake()
        {
            clientIdAndSecret = $"{_clientID}:{_clientSecret}";
            _base64ClientIdAndSecret = Convert.ToBase64String(
                Encoding.ASCII.GetBytes(clientIdAndSecret));
        }

        private void Start()
        {
            // TODO: Move to UI method.
            GetPaymentFromUser(new decimal(2.0f));
        }

        public void GetPaymentFromUser(Decimal cost)
        {
            Decimal paymentCost = System.Math.Round(cost, 2); // Dollars and cents only
            RequestPaymentFromPaypal(paymentCost);
        }

        private async void RequestPaymentFromPaypal(Decimal paymentCost)
        {
            // TODO: error checking and handling.
            string accessToken = await GetAccessToken();
            // TODO: check created state on payment request response
            PaypalPaymentResponse payment = await CreatePayment(paymentCost, accessToken);
            string payerId = await GetPaymentApproval(payment.links, accessToken);
            if (string.IsNullOrEmpty(payerId))
            {
                Debug.LogError("Payer ID couldn't be retrieved from payment approval.");
                return;
            }
            Debug.Log($"payerID: {payerId}");

            // string executePaymentUri = payment.links.Find(l => l.rel == "execute").href;
            // if (await ExecutePayment(executePaymentUri, payerId, accessToken))
            // {
            //     // TODO: display payment complete unity UI and add to warfunds.
            //     Debug.Log($"Execute payment successful.");
            //     return;
            // }
            //
            // Debug.LogError("Executing payment failed.");
        }

        #region SendUnityWebRequest overloads

        private async Task<T> SendUnityWebRequest<T>(
            string uri, Dictionary<string, string> formKeysValues, PaypalPaymentRequest jsonObj,
            Dictionary<string, string> requestHeaderKeysValues) where T : new()
        {
            bool isPost = false;
            WWWForm formData = new WWWForm();
            if (formKeysValues != null)
            {
                foreach (var formKeysValue in formKeysValues)
                {
                    formData.AddField($"{formKeysValue.Key}", $"{formKeysValue.Value}");
                }

                isPost = true;
            }

            UnityWebRequest request;

            if (isPost)
            {
                request = UnityWebRequest.Post(uri, formData);
            }
            else if (jsonObj != null)
            {
                string jsonString = JsonUtility.ToJson(jsonObj);
                byte[] jsonBodyRaw = Encoding.UTF8.GetBytes(jsonString);
                request = new UnityWebRequest(uri, "POST")
                {
                    uploadHandler = new UploadHandlerRaw(jsonBodyRaw),
                    downloadHandler = new DownloadHandlerBuffer()
                };
                request.SetRequestHeader("Content-Type", "application/json");
            }
            else // Assume GET request.
            {
                request = UnityWebRequest.Get(uri);
            }

            foreach (var headerKeysValue in requestHeaderKeysValues)
            {
                request.SetRequestHeader(
                    $"{headerKeysValue.Key}", $"{headerKeysValue.Value}");
            }

            await request.SendWebRequest();

            T response;
            if (request.isNetworkError || request.isHttpError)
            {
                Debug.LogError($"Error sending request to: {uri}, error: {request.error}");
                response = new T();
            }
            else
            {
                Debug.Log(
                    $"Form upload complete! Response: {request.downloadHandler.text}");
                response = JsonUtility.FromJson<T>(request.downloadHandler.text);
            }

            return response;
        }

        private async Task<T> SendUnityWebRequest<T>(
            string uri, Dictionary<string, string> formKeysValues,
            PaypalExecutePaymentRequest jsonObj,
            Dictionary<string, string> requestHeaderKeysValues) where T : new()
        {
            bool isPost = false;
            WWWForm formData = new WWWForm();
            if (formKeysValues != null)
            {
                foreach (var formKeysValue in formKeysValues)
                {
                    formData.AddField($"{formKeysValue.Key}", $"{formKeysValue.Value}");
                }

                isPost = true;
            }

            UnityWebRequest request;

            if (isPost)
            {
                request = UnityWebRequest.Post(uri, formData);
            }
            else if (jsonObj != null)
            {
                string jsonString = JsonUtility.ToJson(jsonObj);
                byte[] jsonBodyRaw = Encoding.UTF8.GetBytes(jsonString);
                request = new UnityWebRequest(uri, "POST")
                {
                    uploadHandler = new UploadHandlerRaw(jsonBodyRaw),
                    downloadHandler = new DownloadHandlerBuffer()
                };
                request.SetRequestHeader("Content-Type", "application/json");
            }
            else // Assume GET request.
            {
                request = UnityWebRequest.Get(uri);
            }

            foreach (var headerKeysValue in requestHeaderKeysValues)
            {
                request.SetRequestHeader(
                    $"{headerKeysValue.Key}", $"{headerKeysValue.Value}");
            }

            await request.SendWebRequest();

            T response;
            if (request.isNetworkError || request.isHttpError)
            {
                Debug.LogError($"Error sending request to: {uri}, error: {request.error}");
                response = new T();
            }
            else
            {
                Debug.Log(
                    $"Form upload complete! Response: {request.downloadHandler.text}");
                response = JsonUtility.FromJson<T>(request.downloadHandler.text);
            }

            return response;
        }

        private async Task<T> SendUnityWebRequest<T>(
            string uri, Dictionary<string, string> formKeysValues,
            Dictionary<string, string> requestHeaderKeysValues) where T : new()
        {
            bool isPost = false;
            WWWForm formData = new WWWForm();
            if (formKeysValues != null)
            {
                foreach (var formKeysValue in formKeysValues)
                {
                    formData.AddField($"{formKeysValue.Key}", $"{formKeysValue.Value}");
                }

                isPost = true;
            }

            UnityWebRequest request;

            if (isPost)
            {
                request = UnityWebRequest.Post(uri, formData);
            }
            else // Assume GET request.
            {
                request = UnityWebRequest.Get(uri);
            }

            foreach (var headerKeysValue in requestHeaderKeysValues)
            {
                request.SetRequestHeader(
                    $"{headerKeysValue.Key}", $"{headerKeysValue.Value}");
            }

            await request.SendWebRequest();

            T response;
            if (request.isNetworkError || request.isHttpError)
            {
                Debug.LogError($"Error sending request to: {uri}, error: {request.error}");
                response = new T();
            }
            else
            {
                Debug.Log(
                    $"Form upload complete! Response: {request.downloadHandler.text}");
                response = JsonUtility.FromJson<T>(request.downloadHandler.text);
            }

            return response;
        }

        #endregion
        
        private async Task<string> GetAccessToken()
        {
            Dictionary<string, string> formDataKeyValues = new Dictionary<string, string>()
            {
                {"grant_type", "client_credentials"},
            };

            Dictionary<string, string> _requestHeaderKeyValues = new Dictionary<string, string>()
            {
                {"Accept", "application/json"},
                {"Accept-Language", "en_US"},
                {"Authorization", $"Basic {_base64ClientIdAndSecret}"},
            };

            PaypalAuthorizationResponse authResponse = await
                SendUnityWebRequest<PaypalAuthorizationResponse>(
                    _paypalTokenSandboxAddress, formDataKeyValues, _requestHeaderKeyValues);
            _paypalAuthResponse = authResponse;
            return authResponse.access_token;
        }

        // TODO: error checking that if accessToken in invalid request another and then just do that
        // every time rather than assume the old one is invalid.
        private async Task<PaypalPaymentResponse> CreatePayment(
            Decimal paymentCost, string accessToken)
        {
            Transaction transaction = new Transaction();
            string paymentCostString = paymentCost.ToString("N");
            transaction.amount = new Amount()
            {
                // ReSharper disable once SpecifyACultureInStringConversionExplicitly
                // total = paymentCostString,
                total = paymentCostString,
                currency = "USD",
                details = new Details()
                {
                    subtotal = paymentCostString,
                    tax = "0.00",
                    shipping = "0.00",
                    handling_fee = "0.00",
                    shipping_discount = "0.00",
                    insurance = "0.00"
                },
            };
            transaction.payment_options = new PaymentOptions()
                {allowed_payment_method = "INSTANT_FUNDING_SOURCE"};
            transaction.description = "This is a microtransaction from EpicCoursePro for WarFunds.";
            transaction.item_list = new ItemList()
            {
                items = new List<Item>()
                {
                    new Item()
                    {
                        name = "WarFunds",
                        quantity = "1",
                        // ReSharper disable once SpecifyACultureInStringConversionExplicitly
                        price = paymentCostString,
                        currency = "USD",
                        sku = "123"
                    },
                },
                shipping_address = new ShippingAddress()
                {
                    recipient_name = "Justin",
                    line1 = "1600 Ampitheatre Pkwy",
                    city = "Mountain View",
                    country_code = "US",
                    postal_code = "94043",
                    phone = "011862212345678",
                    state = "CA"
                }
            };

            PaypalPaymentRequest paymentRequest = new PaypalPaymentRequest()
            {
                intent = "sale",
                payer = new Payer()
                {
                    payment_method = "paypal"
                },
                redirect_urls = new RedirectUrls()
                {
                    return_url = "www.gamdevhq.com",
                    cancel_url = "www.gamdevhq.com"
                },
                transactions = new List<Transaction>() {transaction},
                note_to_payer = "Good luck!"
            };
            _paypalPaymentRequest = paymentRequest;

            Dictionary<string, string> _requestHeaderKeyValues = new Dictionary<string, string>()
            {
                {"Authorization", $"Bearer {accessToken}"},
            };

            PaypalPaymentResponse paymentResponse = await
                SendUnityWebRequest<PaypalPaymentResponse>(
                    _paypalPaymentSandboxAddress, null, paymentRequest,
                    _requestHeaderKeyValues);
            _paypalPaymentResponse = paymentResponse;
            return paymentResponse;
        }


        private async Task<string> GetPaymentApproval(List<Link> paymentLinks, string accessToken)
        {
            // load the approval URL in a webpage
            // Pause for the player since we're going to have them go to a window to pay.
            GameManager.Instance.PauseGameSpeed();
            string paymentApprovalWebpage = paymentLinks.Find(
                link => link.rel == "approval_url").href;
            Application.OpenURL(paymentApprovalWebpage);

            // TODO: Add escape hatch logic if user never approves payment request.
            // Check if the user approved the payment.
            string checkPaymentApprovedURI = paymentLinks.Find(l => l.rel == "self").href;
            Dictionary<string, string> _requestHeaderKeyValues = new Dictionary<string, string>()
            {
                {"Authorization", $"Basic {_base64ClientIdAndSecret}"}
            };
            PaypalPaymentApprovalResponse authResponse = new PaypalPaymentApprovalResponse();
            while (!authResponse.payer.status.Equals("VERIFIED"))
            {
                Debug.Log("Haven't found approved payment by user yet.");
                await WaitBetweenApprovalCheckRetries();
                authResponse = await
                    SendUnityWebRequest<PaypalPaymentApprovalResponse>(
                        checkPaymentApprovedURI, null, _requestHeaderKeyValues);
            }

            Debug.Log("user approved payment");
            return authResponse.payer.payer_info.payer_id;
        }

        private async Task WaitBetweenApprovalCheckRetries()
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
        }

        private async Task<bool> ExecutePayment(string payerId, string executePaymentUri,
            string accessToken)
        {
            PaypalExecutePaymentRequest paymentRequest = new PaypalExecutePaymentRequest()
            {
                payer_id = payerId
            };
            Dictionary<string, string> _requestHeaderKeyValues = new Dictionary<string, string>()
            {
                {"Content-Type", "application/json"},
                {"Authorization", $"Bearer {accessToken}"},
            };
            PaypalExecutePaymentResponse executePaymentResponse = await
                SendUnityWebRequest<PaypalExecutePaymentResponse>(
                    executePaymentUri, null, paymentRequest, _requestHeaderKeyValues);

            _paypalExecutePaymentResponse = executePaymentResponse;

            Transaction approvedTransaction = null;
            foreach (var transaction in executePaymentResponse.transactions)
            {
                foreach (var relatedResource in transaction.related_resources)
                {
                    if (relatedResource.sale.state == "completed")
                    {
                        approvedTransaction = transaction;
                    }
                }
            }

            return approvedTransaction != null;
        }
    }
}
