from ultralytics import YOLO
import cvzone
import cv2
import math

# Running from video (you can change to webcam by using cap = cv2.VideoCapture(0))
cap = cv2.VideoCapture("D:/GUARDIAN/Human Detection/Videos/Test.mp4")

# Load your trained Human Detection model
model = YOLO(r"D:/GUARDIAN/Human Detection/results/runs/detect/train/weights/best.pt")

# Classes (if you only trained for humans, keep ['person'])
classnames = ['person']

while True:
    ret, frame = cap.read()
    if not ret:
        break  # stop if video ends

    frame = cv2.resize(frame, (640, 480))
    results = model(frame, stream=True)

    for info in results:
        boxes = info.boxes
        for box in boxes:
            confidence = float(box.conf[0])
            confidence = math.ceil(confidence * 100)
            cls = int(box.cls[0])

            if confidence > 50:  # Only accept detections above 50%
                x1, y1, x2, y2 = box.xyxy[0]
                x1, y1, x2, y2 = int(x1), int(y1), int(x2), int(y2)

                # Draw bounding box
                cv2.rectangle(frame, (x1, y1), (x2, y2), (0, 255, 0), 3)
                cvzone.putTextRect(frame, f'{classnames[cls]} {confidence}%',
                                   [x1 + 5, y1 - 10],
                                   scale=1.5, thickness=2, colorT=(255,255,255),
                                   colorR=(0, 128, 0), colorB=(0, 128, 0))

    cv2.imshow("Human Detection", frame)
    if cv2.waitKey(1) & 0xFF == ord('q'):
        break

cap.release()
cv2.destroyAllWindows()