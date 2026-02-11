import torch
print(torch.cuda.is_available())  # should print True
print(torch.cuda.get_device_name(0))  # should show NVIDIA GeForce GTX 1650 Ti