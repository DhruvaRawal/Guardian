from ultralytics import YOLO
import cv2
import cvzone

# Load your trained YOLO models
fire_smoke_model = YOLO("D:/GUARDIAN/HumanAndFireSmoke/SmokeAndFire/best.pt")
person_model = YOLO("D:/GUARDIAN/Human Detection/results/runs/detect/train/weights/best.pt")

# Open video file or webcam (use 0 for webcam)
cap = cv2.VideoCapture("C:/Users/dhruv/Downloads/testvideo.mp4")

# Define class names
fire_smoke_classes = ["fire", "smoke"]   # update if order differs in your dataset.yaml
person_classes = ["person"]              # only one class in second model

while True:
    ret, frame = cap.read()
    if not ret:
        break

    # Run both models
    results_fire_smoke = fire_smoke_model(frame, stream=True)
    results_person = person_model(frame, stream=True)

    # --- Fire/Smoke Detections ---
    for r in results_fire_smoke:
        for box in r.boxes:
            x1, y1, x2, y2 = map(int, box.xyxy[0])   # bounding box
            conf = float(box.conf[0])                # confidence
            cls = int(box.cls[0])                    # class id
            label = f"{fire_smoke_classes[cls]} {conf:.2f}"

            # Draw in RED/ORANGE
            cvzone.cornerRect(frame, (x1, y1, x2 - x1, y2 - y1), l=8, rt=2, colorC=(0, 0, 255))
            cvzone.putTextRect(frame, label, (x1, y1 - 10), scale=1, thickness=1, colorR=(0, 0, 255))

    # --- Person Detections ---
    for r in results_person:
        for box in r.boxes:
            x1, y1, x2, y2 = map(int, box.xyxy[0])
            conf = float(box.conf[0])
            cls = int(box.cls[0])
            label = f"{person_classes[cls]} {conf:.2f}"

            # Draw in GREEN
            cvzone.cornerRect(frame, (x1, y1, x2 - x1, y2 - y1), l=8, rt=2, colorC=(0, 255, 0))
            cvzone.putTextRect(frame, label, (x1, y1 - 10), scale=1, thickness=1, colorR=(0, 255, 0))

    # Show result
    cv2.imshow("YOLO Fire/Smoke + Person Detection", frame)
    if cv2.waitKey(1) & 0xFF == ord("q"):
        break

cap.release()
cv2.destroyAllWindows()