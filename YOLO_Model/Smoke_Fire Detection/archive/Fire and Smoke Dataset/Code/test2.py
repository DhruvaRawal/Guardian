from ultralytics import YOLO
import cv2

# Path to your trained model
MODEL_PATH = "C:/Users/dhruv/Downloads/runs/runs/detect/train/weights/best.pt"

# Load the trained model
model = YOLO(MODEL_PATH)

# --- Options ---
USE_CAMERA = False   # Set False if you want to run on video/image
VIDEO_PATH = "C:/Users/dhruv/Downloads/test.mp4"
IMAGE_PATH = "C:/Users/dhruv/OneDrive/Desktop/GUARDIAN/Fire Detection/Images/test.jpg"

# Open webcam or video
if USE_CAMERA:
    cap = cv2.VideoCapture(0)  # Webcam
else:
    cap = cv2.VideoCapture(VIDEO_PATH)  # Video

while True:
    ret, frame = cap.read()
    if not ret:
        break

    # Run YOLO detection
    results = model.predict(frame, conf=0.5)  # Adjust conf threshold as needed

    # Annotate detections
    annotated_frame = results[0].plot()

    # Show output
    cv2.imshow("Fire & Smoke Detection", annotated_frame)

    # Press 'q' to exit
    if cv2.waitKey(1) & 0xFF == ord('q'):
        break

cap.release()
cv2.destroyAllWindows()

# --- Run on single image (optional) ---
# results = model.predict(IMAGE_PATH, conf=0.5, save=True, show=True)
