# DrillToGlory 🌀💎

**DrillToGlory** is a Rogue-like DeckBuilder game built in Unity, heavily inspired by the high-octane energy and themes of the animated series *Gurren Lagann* and the top-selling game Slay The Spire. Players must climb, fight, and upgrade their way through challenging encounters using a deck of unique cards.

## 🚀 Features

- **Rogue-like Mechanics**: Each run is unique, featuring procedural elements and permanent upgrades.
- **Dynamic Deck**: No two runs are alike, test your luck and see how you fare.
- **Spiral Power Energy**: Visuals and mechanics inspired by *Gurren Lagann*, including drill-themed abilities and evolving power scales.
- **Enemy System**: Battle through rooms of unique enemies.
- **Cloud Integration**: Features Firebase authentication and Firestore cloud saving to keep your progress synced.
- **Visual Polish**: DOTween animations for a high-impact combat experience.
- **Challenge your Friends**: See the top 3 

## 🛠 Tech Stack

- **Engine**: Unity 2022+
- **Languages**: C# (Core Gameplay), ShaderLab & HLSL (Visual Effects)
- **Backend**: Firebase (Auth & Firestore)
- **Rendering**: Universal Render Pipeline (URP)
- **Tools**: 
  - [DOTween](http://dotween.demigiant.com/) for animations.
  - TextMesh Pro for high-quality UI text.
  - Unity Input System for modern control support.

## 📂 Project Structure

- `Assets/_Scripts/GameplayMechanics`: Core game logic including `DeckManager`, `TurnController`, `Player`, and `Enemy` systems.
- `Assets/_Scripts/Visuals`: UI management, `AuthManager` for Firebase, and card visual data.
- `Assets/Prefabs`: Reusable game objects for cards, enemies, and UI elements.
- `Assets/Settings`: Configuration for URP and Input Actions.

## 🏗 Installation & Setup

1. **Clone the repository**:
   ```bash
   git clone https://github.com/FarisElbaz/DrillToGlory.git
   ```
2. **Open in Unity**:
   - Open the project using Unity Hub (recommended version: 2022.x).
3. **Firebase Setup**:
   - Since this project uses Firebase, you will need to provide your own `google-services.json` (Android) or `GoogleService-Info.plist` (iOS) in the `Assets/` directory if you wish to run the authentication and cloud-saving features in your own environment.
4. **Play**:
   - Load the main scene from `Assets/Scenes/` and press **Play**.

## 🎮 How to Play

1. **Draw Cards**: Each turn you draw a set of cards from your deck.
2. **Strategic Combat**: Use your energy to play offensive or defensive cards.
3. **Drill Deeper**: Defeat enemies to earn upgrades and new cards for your deck.
4. **Evolve**: Combine abilities to reach "glory" and overcome the ultimate challenges.

## 📜 License

This project is created by [FarisElbaz](https://github.com/FarisElbaz). Please check the repository for specific license details.

---
*"Don't believe in yourself. Believe in me! Believe in the me who believes in you!"*
