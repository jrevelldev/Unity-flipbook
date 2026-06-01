# Interactive Flipbook (40th Birthday Photo Album)

A beautiful, interactive 3D flipbook built in Unity to showcase a collection of photos from a 40th birthday album. This project provides a tactile, immersive way to browse through memories, featuring realistic page-turning mechanics, background music, smooth micro-animations, and an interactive zoom functionality.

Based on the [Unity Book-Page Curl asset](https://www.youtube.com/watch?v=XZLUGbbuHP0).

---

## 🌟 Key Features

- 📖 **Interactive 3D Flipbook**: Realistic page-bending and turning physics simulation that mimics a physical photo album.
- 📸 **40th Birthday Photo Album**: A curated digital photo gallery presenting high-quality memories from the celebration.
- 🎵 **Atmospheric Music**: Smooth background music playback that enhances the nostalgic browsing experience.
- 💫 **Micro-Animations**: Dynamic transitions and hover effects to make the book feel alive and premium.
- 🔍 **Detail Zoom**: A dedicated zoom/inspection feature to let users focus on and view individual photos in detail.
- 🎮 **Modern Input Handling**: Fully compatible with the New Input System and legacy input systems (configured for Both).

---

## 🛠️ Technical Stack & Configuration

- **Engine**: Unity (2023+ / URP Template)
- **Render Pipeline**: Universal Render Pipeline (URP)
- **Input Handling**: Configured to support **Both** the new *Input System Package* (`com.unity.inputsystem`) and legacy *Input Manager* (`UnityEngine.Input`), enabling support for custom input devices as well as legacy asset packs.
- **UI Framework**: Unity UI (uGUI) with `EventSystem`.

---

## 🚀 Getting Started

### Prerequisites
- **Unity Editor**: Ensure you have Unity installed (matching the version specified in the project).
- **Git LFS**: (Recommended) Installed for handling textures and larger assets.

### Installation
1. Clone this repository to your local machine:
   ```bash
   git clone https://github.com/jrevelldev/Unity-flipbook.git
   ```
2. Open the project folder in **Unity Hub**.
3. Let Unity import assets and compile package dependencies (such as the Universal Render Pipeline and the Input System).

---

## 📂 Project Structure

- `Assets/Book-Page Curl/` — Core page curl scripts and assets package.
  - [Book.cs](file:///Users/johnrevell/Git/Unity-flipbook/Assets/Book-Page%20Curl/scripts/Book.cs) — Handles book page layout and curl mathematics.
  - [AutoFlip.cs](file:///Users/johnrevell/Git/Unity-flipbook/Assets/Book-Page%20Curl/scripts/AutoFlip.cs) — Automated page-flipping sequences.
- `Assets/Scenes/` — Main interactive scenes.
- `Assets/Settings/` — URP renderer and Input System configuration settings.

---

## 📈 Roadmap & Upcoming Tasks

- [x] Configure Input Settings for hybrid compatibility (New Input System + legacy `Input.mousePosition` APIs).
- [ ] Implement background music controller with volume controls.
- [ ] Add the 40th birthday photo album textures and assign them to book pages.
- [ ] Create UI/UX animations for page selection and detail transitions.
- [ ] Build the interactive Zoom/Detail inspection overlay.
