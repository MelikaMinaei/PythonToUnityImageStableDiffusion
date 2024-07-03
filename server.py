import grpc
from concurrent import futures
import PROTO_FILE_pb2
import PROTO_FILE_pb2_grpc
import torch
from diffusers import StableDiffusionPipeline
import random
import numpy as np
import os


PATH = ".\\Images\\"


class UnityCommunicationServicer(PROTO_FILE_pb2_grpc.UnityCommunicationServicer):
    def UnityCommunicationRPC(self, request, context):
        ### Initializing the seed variable to reproduce the same results
        seed = 1
        torch.manual_seed(seed)
        torch.cuda.manual_seed(seed)
        random.seed(seed)
        np.random.seed(seed)

        ### The diffusion model version
        #model_id = "stabilityai/stable-diffusion-2-1"
        model_id = "CompVis/stable-diffusion-v1-4"

        ### Loading the diffusion and sampler pipeline
        pipe = StableDiffusionPipeline.from_pretrained(model_id, torch_dtype=torch.float32)
        pipe = pipe.to("cuda")
        
        prompt = request.prompt.strip()
        print(f"PROMPT: {prompt}")
        print(len(prompt))
        

        image_path = os.path.join(PATH, f"{prompt}.png")
        print(f"IMAGE PATH: {image_path}")

        try:
            ### Generate the image based on a textual prompt
            image = pipe(prompt).images
            image[0].save(image_path)
            with open(image_path, 'rb') as f:
                image_data = f.read()
            return PROTO_FILE_pb2.ServerResponse(image=image_data)
 
        except FileNotFoundError:
            context.set_details(f'File {image_path} not found.')
            context.set_code(grpc.StatusCode.NOT_FOUND)


def serve():
    server = grpc.server(futures.ThreadPoolExecutor(max_workers=10))
    PROTO_FILE_pb2_grpc.add_UnityCommunicationServicer_to_server(UnityCommunicationServicer(), server)
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