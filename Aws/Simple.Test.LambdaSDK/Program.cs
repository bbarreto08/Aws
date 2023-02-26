using Amazon.Lambda;
using Amazon.Lambda.Model;

AmazonLambdaClient client = new AmazonLambdaClient();

string input = "{ \"Input\": \"hello word!!!\"}";

var request = new InvokeRequest()
{
    FunctionName = "SimpleTestLambda",
    Payload = input
};

var response = client.InvokeAsync(request).Result;

Console.WriteLine($"Result => {new StreamReader(response.Payload).ReadToEnd()}");

Console.ReadKey();