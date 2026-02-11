from ultralytics import YOLO

# Load model
model = YOLO("yolov8n.pt")

# Train
model.train(
    data="C:/Users/dhruv/Downloads/archive/Fire and Smoke Dataset/data.yaml",
    epochs=50,
    imgsz=640,
    batch=32,
    workers=4,
    device=0
)
