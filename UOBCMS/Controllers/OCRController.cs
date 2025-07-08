using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using TencentCloud.Common;
using TencentCloud.Common.Profile;
using TencentCloud.Ocr.V20181119;
using TencentCloud.Ocr.V20181119.Models;
using UOBCMS.Classes;

namespace UOBCMS.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class AddressLines
    {
        public string Address_Line_1 { get; set; }
        public string Address_Line_2 { get; set; }
        public string Address_Line_3 { get; set; }
        public string Address_Line_4 { get; set; }
    }

    public class BankAccInfo
    {
        public string AccountNo { get; set; }
        public string AccountName { get; set; }
    }

    public class OCRController : Controller
    {
        /*public IActionResult Index()
        {
            return View();
        }*/

        private string ConvertImagePathToBase64(string imagePath)
        {
            // Use System.IO.File to read the image file into a byte array
            byte[] imageBytes = System.IO.File.ReadAllBytes(imagePath);

            // Convert the byte array to a Base64 string
            string base64String = Convert.ToBase64String(imageBytes);

            return base64String;
        }

        private string ConvertImageToBase64(IFormFile file)
        {
            using (var memoryStream = new MemoryStream())
            {
                // Copy the uploaded file to the memory stream
                file.CopyTo(memoryStream);

                // Get the byte array from the memory stream
                byte[] imageBytes = memoryStream.ToArray();

                // Convert the byte array to a Base64 string
                string base64String = Convert.ToBase64String(imageBytes);

                return base64String;
            }
        }

        [HttpPost("OCR/api/getBankAccInfo")]
        public async Task<IActionResult> getBankAccInfo(IFormFile image)
        {
            try
            {
                // Required steps:
                // Instantiate an authentication object. The Tencent Cloud account key pair `secretId` and `secretKey` need to be passed in as the input parameters
                // This example uses the way to read from the environment variable, so you need to set these two values in the environment variable in advance
                // You can  also write the key pair directly into the code, but be careful not to copy, upload, or share the code to others
                // Query the CAM key: https://console.tencentcloud.com/capi
                TencentCloud.Common.Credential cred = new TencentCloud.Common.Credential
                {
                    SecretId = "IKIDBbotmaAWfbJn273pRtAFm9Or7IVZQ8e2",
                    SecretKey = "JTRDzd4axCF8qLdnmjSClANh1LXVIrHV"
                };

                // Optional steps:
                // Instantiate a client configuration object. You can specify the timeout period and other configuration items
                ClientProfile clientProfile = new ClientProfile();

                // Instantiate an HTTP option (optional; skip if there are no special requirements)
                HttpProfile httpProfile = new HttpProfile();
                httpProfile.Endpoint = ("ocr.intl.tencentcloudapi.com");
                clientProfile.HttpProfile = httpProfile;

                // Instantiate an client object
                // The second parameter is the region information. You can directly enter the string "ap-guangzhou" or import the preset constant
                OcrClient client = new OcrClient(cred, "", clientProfile);
                // Instantiate a request object. You can further set the request parameters according to the API called and actual conditions
                SmartStructuralProRequest req = new SmartStructuralProRequest();
                //req.ImageUrl = "https://uobhk-1345032046.cos.ap-hongkong.myqcloud.com/electricity_bill/OCR_Electricity_Bill.jpg";
                //req.ImageUrl = "https://uobhk-1345032046.cos.ap-hongkong.myqcloud.com/electricity_bill/OCR_Electricity_Bill_2.jpg";
                //req.ImageUrl = "https://uobhk-1345032046.cos.ap-hongkong.myqcloud.com/electricity_bill/OCR_Electricity_Bill_2.jpg?q-sign-algorithm=sha1&q-ak=AKIDUDIGZ0jhxsvHQs2BdYRVWGsudLTQ2hR7oO0fZhesK_Xgg6ElF9xMz-PwLfT6JQAZ&q-sign-time=1741936874;1741940474&q-key-time=1741936874;1741940474&q-header-list=host&q-url-param-list=ci-process&q-signature=714bb2d9e996177a12d35c5d730d4455be3eaec2&x-cos-security-token=gNh2vUXn7j6zVO2uPuMIPnp2RjSm9C1ad9ce7ec1e9eac6ce823e1008c930f2fdhZX9Z2MNE5taxVWqxiVFcaRV2YaKrEBTFBcKSU9TnTqh0DP3WGztl1BB4Cb5E5wslw_-l66YedWdfiEV-SuV_iCaDniWttWxRk4DUdOxrEiuGZu3RlsRWVEfV4j6PQNaMSH65vwpkloB_AbkU6lYHO6LShmkRfjzuw86IRqWJbiZD6hqAW7Itcbhi0n2PDjn&ci-process=originImage";
                //req.ImageUrl = "https://uobhk-1345032046.cos.ap-hongkong.myqcloud.com/water_bill/OCR_Water_Bill.jpg?q-sign-algorithm=sha1&q-ak=AKIDKK2J8_YxuCRu8Z_9aO7NSMU8-pjTG0n7_sq85rcC0NrnuXOEsUs6WhqkX5v_1nGW&q-sign-time=1741938412;1741942012&q-key-time=1741938412;1741942012&q-header-list=host&q-url-param-list=ci-process&q-signature=2ac4c805408f3c9255c6374f6f2394f071bcd4a5&x-cos-security-token=gNh2vUXn7j6zVO2uPuMIPnp2RjSm9C1aa97dcbd82837cb675806fdf703e80e42hZX9Z2MNE5taxVWqxiVFceTs1mx6WTxoXjy1cpp5KhXnE0TsgjkzE8QZPbk4TgX_DAgujPDdwqj8IdtMsW7JqOdTNl-dueX6Jk6_LH7aIJ_6t0n8tomN2sSR-amj5Cy7d3hqndRNwkLQ_d2av9j3QSU0LiuS-emcSaswBpcB8bKOT_mAxtd039OnoxQ_Z3U7&ci-process=originImage";

                // pass as an url or imagebase64
                //req.ImageUrl = "https://uobhk-1345032046.cos.ap-hongkong.myqcloud.com/electricity_bill/OCR_Electricity_Bill.jpg";

                //string imagePath = @"C:\doc\Cloud\Tencent\OCR_Electricity_Bill.jpg"; // Specify the path to your JPG image
                //string base64String = ConvertImagePathToBase64(imagePath);

                if (image == null || image.Length == 0)
                {
                    return BadRequest("No file uploaded.");
                }

                string base64String = ConvertImageToBase64(image);

                req.ImageBase64 = base64String;

                // The returned "resp" is an instance of the SmartStructuralProResponse class which corresponds to the request object
                SmartStructuralProResponse resp = client.SmartStructuralProSync(req);

                BankAccInfo bankAccInfo = new BankAccInfo();

                for (int i = 0; i <= resp.StructuralList.Length - 1; i++)
                {
                    for (int j = 0; j <= resp.StructuralList[i].Groups.Length - 1; j++)
                    {
                        for (int k = 0; k <= resp.StructuralList[i].Groups[j].Lines.Length - 1; k++)
                        {
                            Console.WriteLine("AutoName" + " " + i.ToString() + "-" + resp.StructuralList[i].Groups[j].Lines[k].Key.AutoName);
                            Console.WriteLine("Value" + " " + i.ToString() + "-" + resp.StructuralList[i].Groups[j].Lines[k].Value.AutoContent);

                            if (resp.StructuralList[i].Groups[j].Lines[k].Key.AutoName == "号码")
                            {
                                Console.WriteLine(resp.StructuralList[i].Groups[j].Lines[k].Key.AutoName + ":" + resp.StructuralList[i].Groups[j].Lines[k].Value.AutoContent);
                                bankAccInfo.AccountNo = resp.StructuralList[i].Groups[j].Lines[k].Value.AutoContent;                                
                            }
                            else if (resp.StructuralList[i].Groups[j].Lines[k].Key.AutoName == "姓名")
                            {
                                Console.WriteLine(resp.StructuralList[i].Groups[j].Lines[k].Key.AutoName + ":" + resp.StructuralList[i].Groups[j].Lines[k].Value.AutoContent);
                                bankAccInfo.AccountName = resp.StructuralList[i].Groups[j].Lines[k].Value.AutoContent;
                            }
                        }
                    }
                }

                // Return the address lines as JSON
                return Ok(bankAccInfo);                
            }
            catch (Exception ex)
            {
                // Log the exception and return a 500 Internal Server Error response
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost("OCR/api/getAddress")]
        public async Task<IActionResult> GetAddress(IFormFile image)
        {
            try
            {
                return null;
                /*string json_address_text1 = "";
                using (var stream = new MemoryStream())
                {
                    image.CopyTo(stream);
                    var apiCloudswayClient = new CloudswayApiClient("DpAdDqy2u1Lc0junNRr1");
                    json_address_text1 = await apiCloudswayClient.ExtractAddressLinesFromBill(stream.ToArray());
                }*/

                // Required steps:
                // Instantiate an authentication object. The Tencent Cloud account key pair `secretId` and `secretKey` need to be passed in as the input parameters
                // This example uses the way to read from the environment variable, so you need to set these two values in the environment variable in advance
                // You can  also write the key pair directly into the code, but be careful not to copy, upload, or share the code to others
                // Query the CAM key: https://console.tencentcloud.com/capi
                TencentCloud.Common.Credential cred = new TencentCloud.Common.Credential
                {
                    SecretId = "IKIDBbotmaAWfbJn273pRtAFm9Or7IVZQ8e2",
                    SecretKey = "JTRDzd4axCF8qLdnmjSClANh1LXVIrHV"
                };

                // Optional steps:
                // Instantiate a client configuration object. You can specify the timeout period and other configuration items
                ClientProfile clientProfile = new ClientProfile();

                // Instantiate an HTTP option (optional; skip if there are no special requirements)
                HttpProfile httpProfile = new HttpProfile();
                httpProfile.Endpoint = ("ocr.intl.tencentcloudapi.com");
                clientProfile.HttpProfile = httpProfile;

                // Instantiate an client object
                // The second parameter is the region information. You can directly enter the string "ap-guangzhou" or import the preset constant
                OcrClient client = new OcrClient(cred, "", clientProfile);
                // Instantiate a request object. You can further set the request parameters according to the API called and actual conditions
                SmartStructuralProRequest req = new SmartStructuralProRequest();
                //req.ImageUrl = "https://uobhk-1345032046.cos.ap-hongkong.myqcloud.com/electricity_bill/OCR_Electricity_Bill.jpg";
                //req.ImageUrl = "https://uobhk-1345032046.cos.ap-hongkong.myqcloud.com/electricity_bill/OCR_Electricity_Bill_2.jpg";
                //req.ImageUrl = "https://uobhk-1345032046.cos.ap-hongkong.myqcloud.com/electricity_bill/OCR_Electricity_Bill_2.jpg?q-sign-algorithm=sha1&q-ak=AKIDUDIGZ0jhxsvHQs2BdYRVWGsudLTQ2hR7oO0fZhesK_Xgg6ElF9xMz-PwLfT6JQAZ&q-sign-time=1741936874;1741940474&q-key-time=1741936874;1741940474&q-header-list=host&q-url-param-list=ci-process&q-signature=714bb2d9e996177a12d35c5d730d4455be3eaec2&x-cos-security-token=gNh2vUXn7j6zVO2uPuMIPnp2RjSm9C1ad9ce7ec1e9eac6ce823e1008c930f2fdhZX9Z2MNE5taxVWqxiVFcaRV2YaKrEBTFBcKSU9TnTqh0DP3WGztl1BB4Cb5E5wslw_-l66YedWdfiEV-SuV_iCaDniWttWxRk4DUdOxrEiuGZu3RlsRWVEfV4j6PQNaMSH65vwpkloB_AbkU6lYHO6LShmkRfjzuw86IRqWJbiZD6hqAW7Itcbhi0n2PDjn&ci-process=originImage";
                //req.ImageUrl = "https://uobhk-1345032046.cos.ap-hongkong.myqcloud.com/water_bill/OCR_Water_Bill.jpg?q-sign-algorithm=sha1&q-ak=AKIDKK2J8_YxuCRu8Z_9aO7NSMU8-pjTG0n7_sq85rcC0NrnuXOEsUs6WhqkX5v_1nGW&q-sign-time=1741938412;1741942012&q-key-time=1741938412;1741942012&q-header-list=host&q-url-param-list=ci-process&q-signature=2ac4c805408f3c9255c6374f6f2394f071bcd4a5&x-cos-security-token=gNh2vUXn7j6zVO2uPuMIPnp2RjSm9C1aa97dcbd82837cb675806fdf703e80e42hZX9Z2MNE5taxVWqxiVFceTs1mx6WTxoXjy1cpp5KhXnE0TsgjkzE8QZPbk4TgX_DAgujPDdwqj8IdtMsW7JqOdTNl-dueX6Jk6_LH7aIJ_6t0n8tomN2sSR-amj5Cy7d3hqndRNwkLQ_d2av9j3QSU0LiuS-emcSaswBpcB8bKOT_mAxtd039OnoxQ_Z3U7&ci-process=originImage";

                // pass as an url or imagebase64
                //req.ImageUrl = "https://uobhk-1345032046.cos.ap-hongkong.myqcloud.com/electricity_bill/OCR_Electricity_Bill.jpg";

                //string imagePath = @"C:\doc\Cloud\Tencent\OCR_Electricity_Bill.jpg"; // Specify the path to your JPG image
                //string base64String = ConvertImagePathToBase64(imagePath);

                if (image == null || image.Length == 0)
                {
                    return BadRequest("No file uploaded.");
                }

                string base64String = ConvertImageToBase64(image);

                req.ImageBase64 = base64String;

                // The returned "resp" is an instance of the SmartStructuralProResponse class which corresponds to the request object
                SmartStructuralProResponse resp = client.SmartStructuralProSync(req);

                AddressLines addressLines = null;

                for (int i = 0; i <= resp.StructuralList.Length - 1; i++)
                {
                    for (int j = 0; j <= resp.StructuralList[i].Groups.Length - 1; j++)
                    {
                        for (int k = 0; k <= resp.StructuralList[i].Groups[j].Lines.Length - 1; k++)
                        {
                            Console.WriteLine("AutoName" + " " + i.ToString() + "-" + resp.StructuralList[i].Groups[j].Lines[k].Key.AutoName);
                            Console.WriteLine("Value" + " " + i.ToString() + "-" + resp.StructuralList[i].Groups[j].Lines[k].Value.AutoContent);
                            
                            if (resp.StructuralList[i].Groups[j].Lines[k].Key.AutoName == "地址")
                            {
                                Console.WriteLine(resp.StructuralList[i].Groups[j].Lines[k].Key.AutoName + ":" + resp.StructuralList[i].Groups[j].Lines[k].Value.AutoContent);
                                Console.WriteLine("");
                                Console.WriteLine("");
                                Console.WriteLine("***** Change Address to four lines *****");
                                var apiCloudswayClient = new CloudswayApiClient("DpAdDqy2u1Lc0junNRr1");
                                string json_address_text = apiCloudswayClient.ChangeAddressTo4LinesInJson(resp.StructuralList[i].Groups[j].Lines[k].Value.AutoContent).Result;
                                Console.WriteLine("Response JSON Content: " + json_address_text);

                                // Parse the JSON response
                                var jsonObject = JObject.Parse(json_address_text);
                                var messageContent = jsonObject["choices"][0]["message"]["content"].ToString();

                                // Extract JSON from the message content
                                var jsonStart = messageContent.IndexOf("{");
                                var jsonEnd = messageContent.IndexOf("}", jsonStart) + 1;
                                var addressJson = messageContent.Substring(jsonStart, jsonEnd - jsonStart);

                                // Deserialize the address JSON
                                addressLines = JsonConvert.DeserializeObject<AddressLines>(addressJson);

                                // Output the address lines
                                Console.WriteLine("Address Line 1: " + addressLines.Address_Line_1);
                                Console.WriteLine("Address Line 2: " + addressLines.Address_Line_2);
                                Console.WriteLine("Address Line 3: " + addressLines.Address_Line_3);
                                Console.WriteLine("Address Line 4: " + addressLines.Address_Line_4);
                                
                            }
                        }
                    }
                }

                // Return the address lines as JSON
                return Ok(addressLines);

                // If no address is found, return a 404 Not Found response
                return NotFound("Address not found in the provided image.");
            }
            catch (Exception ex)
            {
                // Log the exception and return a 500 Internal Server Error response
                Console.WriteLine($"An error occurred: {ex.Message}");
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
