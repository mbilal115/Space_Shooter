# 🚀 Space Shooter Game – C# Windows Forms (GDI)

A classic 2D Space Shooter game developed using **C#**, **Windows Forms**, and **GDI+ graphics rendering**. This project demonstrates the implementation of real-time gameplay mechanics, keyboard input handling, collision detection, sprite rendering, and game loop management within a desktop application environment.

The game was designed as a learning project to explore the fundamentals of game development using the .NET Framework and traditional Windows desktop technologies.

---

# 📌 Overview

The player controls a spaceship and must survive against waves of incoming enemies while shooting targets to gain points. The gameplay includes smooth movement, projectile mechanics, enemy spawning systems, score tracking, and game-over handling.

Graphics are rendered using the `System.Drawing` namespace and GDI+, making the project lightweight while still providing responsive gameplay.

This project also focuses on software engineering concepts such as:

* Object-Oriented Programming (OOP)
* Event-driven programming
* Real-time rendering
* Collision detection
* State management
* Dynamic object handling
* Game loop implementation

---

# 🎮 Features

## Gameplay Features

* Real-time 2D space shooter gameplay
* Smooth player movement
* Shooting and projectile mechanics
* Enemy spawning system
* Collision detection between bullets and enemies
* Score tracking system
* Health/life system *(if implemented)*
* Game over screen
* Restart functionality
* Keyboard-based controls

## Technical Features

* Built using Windows Forms
* Graphics rendering with GDI+
* Timer-based game loop
* Dynamic object management
* Lightweight desktop application
* Modular and maintainable code structure

---

# 🛠 Technologies Used

| Technology            | Purpose                       |
| --------------------- | ----------------------------- |
| C#                    | Core programming language     |
| Windows Forms         | Desktop application framework |
| GDI+ / System.Drawing | Graphics rendering            |
| .NET Framework        | Application runtime           |
| Visual Studio         | Development environment       |

---

# 🧠 Concepts Implemented

This project demonstrates several important programming and game development concepts:

## Object-Oriented Programming

The game logic is organized using reusable classes for:

* Player
* Enemy
* Bullet
* Game objects
* Collision handling

## Event-Driven Programming

Keyboard input and game updates are handled through WinForms events and timers.

## Collision Detection

The game checks intersections between:

* Bullets and enemies
* Enemies and player
* Player boundaries

## Real-Time Rendering

Graphics are updated continuously using GDI+ rendering methods for smooth gameplay.

---

# 🎯 Gameplay

The player controls a spaceship and must destroy incoming enemies while avoiding collisions.

### Objective

* Eliminate enemies
* Survive as long as possible
* Achieve the highest score

### Controls

| Key               | Action                    |
| ----------------- | ------------------------- |
| Arrow Keys / WASD | Move spaceship            |
| Spacebar          | Shoot                     |
| R                 | Restart Game *(optional)* |
| Esc               | Exit *(optional)*         |

---

# 📂 Project Structure

```text
SpaceShooter/
│
├── Assets/
│   ├── Images/
│   ├── Sounds/
│
├── Forms/
│   ├── MainForm.cs
│
├── Classes/
│   ├── Player.cs
│   ├── Enemy.cs
│   ├── Bullet.cs
│   ├── GameManager.cs
│
├── Program.cs
├── README.md
└── SpaceShooter.sln
```

---

# ▶️ How to Run

## Requirements

* Visual Studio
* .NET Framework installed

## Steps

1. Clone the repository:

```bash
git clone https://github.com/your-username/space-shooter-game.git
```

2. Open the solution file in Visual Studio.

3. Build the project.

4. Run the application.

---

# 📸 Screenshots

Add gameplay screenshots here.

```text
Example:
- Main Menu
- Gameplay
- Game Over Screen
```

---

# 🚧 Future Improvements

Planned future enhancements include:

* Multiple enemy types
* Boss fights
* Power-ups and upgrades
* Sound effects and background music
* Difficulty progression
* Better animations
* Particle effects
* Pause menu
* High score saving system
* Leaderboard functionality
* Multiplayer mode

---

# 🧪 Learning Outcomes

Through this project, the following skills were practiced and improved:

* Desktop application development
* Game logic implementation
* Real-time rendering techniques
* Input handling
* Problem solving
* Debugging and optimization
* Object-oriented design
* Game architecture fundamentals

---

# 🤝 Contribution

Contributions, suggestions, and improvements are welcome.

If you would like to contribute:

1. Fork the repository
2. Create a new branch
3. Commit your changes
4. Open a pull request

---

# 📄 License

This project is open-source and available under the MIT License.

---

# 👨‍💻 Author

Developed by **[Your Name]**.

If you like this project, consider giving it a ⭐ on GitHub.
