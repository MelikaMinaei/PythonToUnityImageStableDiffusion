using UnityEngine;
using Grpc.Core;
using PROTOFILE;
//using System.Threading.Tasks;

public class Client : MonoBehaviour
{
    private Channel channel;
    private UnityCommunication.UnityCommunicationClient client; // For using gRPC's generated codes
    private Renderer shapeRenderer;

    void Start()
    {
        shapeRenderer = GetComponent<Renderer>();
        channel = new Channel("localhost:50051", ChannelCredentials.Insecure); // It should be the same port as the server's
        client = new UnityCommunication.UnityCommunicationClient(channel); // Connects the client to the server
    }

    public void ChangeMaterial(string prompt)
    {
        Debug.Log("Prompt entered in Input Field!");
        SendImageRequest("Tileable image without global structure of " + prompt + "");
    }

    private async void SendImageRequest(string completePrompt)
    {
        var request = new UnityRequest {  Prompt = completePrompt };// Creates a request
        try
        {
            var response = await client.UnityCommunicationRPCAsync(request); // Sends the request and waits for the response
            byte[] imageData = response.Image.ToByteArray();
            Texture2D texture = new Texture2D(1, 1);
            if (texture.LoadImage(imageData))
            {
                shapeRenderer.sharedMaterial.mainTexture = texture;
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