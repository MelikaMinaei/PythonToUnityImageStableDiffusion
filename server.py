import grpc
from concurrent import futures
import sendImage_pb2
import sendImage_pb2_grpc
import torch
from diffusers import StableDiffusionPipeline
import random
import numpy as np


PATH = ".\\Images\\"


class ImageServiceServicer(sendImage_pb2_grpc.ImageServiceServicer):
    def SendImage(self, request, context):
        ### Initializing the seed variable to reproduce the same results
        seed = 1
        torch.manual_seed(seed)
        torch.cuda.manual_seed(seed)
        random.seed(seed)
        np.random.seed(seed)

        ### The diffusion model version
        model_id = "stabilityai/stable-diffusion-2-1"

        ### Loading the diffusion and sampler pipeline
        pipe = StableDiffusionPipeline.from_pretrained(model_id, torch_dtype=torch.float32)
        pipe = pipe.to("cuda")
        
        prompt = request.name
        image_path = PATH+prompt+".png"

        try:
            ### Generate the image based on a textual prompt
            image = pipe(prompt).images[0]
            image.save(image_path)
            with open(image_path, 'rb') as f:
                image_data = f.read()
            return sendImage_pb2.ImageResponse(image_data=image_data)
 
        except FileNotFoundError:
            context.set_details(f'File {image_path} not found.')
            context.set_code(grpc.StatusCode.NOT_FOUND)


def serve():
    server = grpc.server(futures.ThreadPoolExecutor(max_workers=10))
    sendImage_pb2_grpc.add_ImageServiceServicer_to_server(ImageServiceServicer(), server)
    server.add_insecure_port('localhost:50051')
    server.start()
    print("Server started on port 50051.")
    try:
        server.wait_for_termination()
    except KeyboardInterrupt:
        server.stop(0)
        print("Server stopped.")


if __name__ == '__main__':
    serve()
