using UnityEngine;
using Grpc.Core;
using SendImage;
//using System.Threading.Tasks;

public class ClientgRPC : MonoBehaviour
{
    private Channel channel;
    private ImageService.ImageServiceClient client; // For using gRPC's generated codes
    //private Renderer shapeRenderer;
    [SerializeField]
    private Material shapesMaterial;
    [SerializeField]
    private Material balloonMaterial;


    void Start()
    {
        //shapeRenderer = GetComponent<Renderer>();
        channel = new Channel("localhost:50051", ChannelCredentials.Insecure); // It should be the same port as the server's
        client = new ImageService.ImageServiceClient(channel); // Connects the client to the server
    }

    public void ChangeMaterial(string prompt)
    {
        Debug.Log("Prompt entered in Input Field!");
        Debug.Log(prompt);
        SendImageRequest(prompt);
    }

 private void SendImageRequest(string completePrompt)
{
    var request = new ImageRequest { Name = completePrompt }; // Creates a request
    try
    {
        var response = client.SendImageAsync(request).GetAwaiter().GetResult(); // Sends the request and waits for the response
        byte[] imageData = response.ImageData.ToByteArray();
        Texture2D texture = new Texture2D(1, 1);
        if (texture.LoadImage(imageData))
        {
            shapesMaterial.mainTexture = texture;
            balloonMaterial.mainTexture = texture;

        }
        else
        {
            Debug.LogError("Failed to load image data into texture.");
        }
    }
    catch (RpcException e)
    {
        Debug.LogError($"RPC failed: {e.Status}");
    }
}


    /**
     * Shuts down the created channel upon destruction
     */
    private void OnDestroy()
    {
        channel.ShutdownAsync().Wait();
    }
}
