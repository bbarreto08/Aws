using Amazon.KeyManagementService;
using Amazon.KeyManagementService.Model;
using Amazon.Lambda.Core;
using Simple.Test.Lambda.Models.Request;
using Simple.Test.Lambda.Models.Response;
using System.Text;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Simple.Test.Lambda;

public class Function
{
    
    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public InputResponse Handler(InputRequest input, ILambdaContext context)
    {
        // Recuperar variável de ambiente sem criptografia
        // var param01 = Environment.GetEnvironmentVariable("Param01") ?? "Default";

        // Recuperar variável de ambiente criptografada pelo AWS KMS
        // DecodeVariable(string envName)

        return new InputResponse
        {
            Output = input.Input.ToUpper()
        };
    }

    private static async Task<string> DecodeVariable(string envName)
    {
        var encryptedBase64Text = Environment.GetEnvironmentVariable(envName);
        var encryptedBytes = Convert.FromBase64String(encryptedBase64Text);
        var encryptionContext = new Dictionary<string, string>();
        encryptionContext.Add("LambdaFunctionName", Environment.GetEnvironmentVariable("AWS_LAMBDA_FUNCTION_NAME"));

        using(var client = new AmazonKeyManagementServiceClient())
        {
            var decryptRequest = new DecryptRequest
            {
                CiphertextBlob = new MemoryStream(encryptedBytes),
                EncryptionContext = encryptionContext
            };

            var response = await client.DecryptAsync(decryptRequest);

            using(var plainTextStream = response.Plaintext)
            {
                var plainTextBytes = plainTextStream.ToArray();
                var plainText = Encoding.UTF8.GetString(plainTextBytes);
                return plainText;
            }
        }

    }


}
