# 🍳 Yes Chef!

## Short Description

**Yes Chef!** is a 3D single-player cooking game developed in **Unity using C#** as part of the **Tentworks Interactive Developer Test**.

The player controls a chef working in a compact kitchen and must collect ingredients, prepare food, and complete customer orders before the **3-minute game timer** expires.

The project focuses on responsive player movement, proximity-based interactions, ingredient preparation, randomized customer orders, scoring, and clean component-based gameplay architecture.

---

## 🎮 Gameplay

The objective is to complete as many customer orders as possible within **3 minutes**.

The core gameplay loop is:

1. Collect an ingredient from the refrigerator.
2. Prepare the ingredient if necessary.
3. Carry the prepared ingredient to a customer window.
4. Deliver the ingredient to a matching order.
5. Complete all ingredients required by the order.
6. Earn points based on the ingredients and completion time.
7. Continue completing orders until the game timer reaches zero.

The chef can carry a maximum of **one item at a time**.

At the beginning of the game, four customer orders are active.

When an order is completed, a new order is generated for that customer window after **5 seconds**.

The game ends when the 3-minute timer reaches `00:00`.

---

## 🕹️ Controls

| Input | Action |
|---|---|
| `W` / `↑` | Move Forward |
| `S` / `↓` | Move Backward |
| `A` / `←` | Move Left |
| `D` / `→` | Move Right |
| Mouse | UI interaction |

Kitchen interactions are primarily **proximity-based**.

The player approaches kitchen stations to interact with them. Ingredient selection at the refrigerator is performed using UI buttons.

The game also includes a **Controls / Start screen** before gameplay begins.

---

## 🍽️ Features / Kitchen Stations

### 🧊 Refrigerator

The refrigerator contains an unlimited supply of:

- 🥩 Meat
- 🥕 Vegetable
- 🧀 Cheese

When the chef approaches the refrigerator, ingredient selection buttons become available.

The player can select an ingredient when the chef's hand is empty.

Once selected, the ingredient is placed in the chef's hand.

---

### 🔪 Chopping Table

Raw vegetables must be prepared at the chopping table before they can be delivered to customers.

When the chef approaches the table while holding a raw vegetable:

1. The vegetable is removed from the chef's hand.
2. The vegetable is placed on the chopping area.
3. The chef uses the knife.
4. A **2-second preparation timer** is displayed.
5. The vegetable becomes a prepared/chopped vegetable.
6. The prepared vegetable can then be transported to a customer.

Raw vegetables cannot satisfy customer orders.

---

### 🔥 Stove

Raw meat must be cooked before it can be delivered.

The stove contains **two cooking slots**, allowing multiple pieces of meat to be prepared.

Each piece of meat requires **6 seconds** to cook.

Each stove position independently keeps track of:

- Whether the slot is occupied
- The meat being cooked
- Cooking progress
- Whether the meat has finished cooking

The chef can move away from the stove while the meat continues cooking.

Cooked meat can only be collected after its cooking timer has completed.

---

### 🗑️ Trash

The trash is used to discard the item currently being carried by the chef.

This allows unwanted ingredients to be removed so the player can collect another ingredient.

---

### ⏱️ Game Timer

Each game lasts:

**3 minutes**

The timer counts down:

`03:00 → 02:59 → 02:58 → ... → 00:00`

When the timer reaches zero:

- Gameplay ends.
- Player movement stops.
- Orders stop accepting ingredients.
- The final score is displayed.
- The player can restart or quit.

---

### ⏸️ Pause / Resume

The game can be paused during gameplay.

While paused:

- Player movement stops.
- Game timer stops.
- Order timers stop.
- Cooking timers stop.
- Chopping timers stop.
- Gameplay interactions are suspended.

The player can resume gameplay from the Pause menu.

---

## 🧾 Order System

The kitchen contains **four customer windows**.

Each customer window independently manages its own order.

At the start of gameplay, all four customer windows receive an order.

Each order has:

- **50% chance of containing 2 ingredients**
- **50% chance of containing 3 ingredients**

Ingredients are selected randomly.

Duplicate ingredients are allowed.

Example orders:

`Meat + Cheese`

`Vegetable + Meat + Cheese`

`Meat + Meat + Vegetable`

`Cheese + Cheese`

Each customer window displays:

- Required ingredient icons
- Current order duration

Only correctly prepared ingredients can satisfy an order.

For example:

`Raw Vegetable` ❌  
`Chopped Vegetable` ✅

`Raw Meat` ❌  
`Cooked Meat` ✅

`Cheese` ✅

When the chef enters a customer window interaction area while holding a valid required ingredient:

1. The ingredient is removed from the chef's hand.
2. One matching ingredient requirement is removed from the order.
3. The remaining ingredient icons are updated.

If the customer does not require the ingredient, it remains in the chef's hand.

Duplicate requirements are handled individually.

For example:

`Meat + Meat + Cheese`

Delivering one cooked meat changes the order to:

`Meat + Cheese`

When all required ingredients have been delivered:

1. The order is completed.
2. The order score is calculated.
3. The score earned is displayed near the customer window.
4. The window becomes temporarily inactive.
5. After **5 seconds**, a new randomized order is generated.

Each of the four customer windows operates independently.

---

## 🏆 Scoring

Each ingredient has a base score:

| Ingredient | Preparation | Score |
|---|---|---:|
| 🥕 Vegetable | Chopped | 20 |
| 🧀 Cheese | None | 10 |
| 🥩 Meat | Cooked | 30 |

The final score for an order is calculated using:

`Order Score = Total Ingredient Score - Whole Seconds Elapsed`

### Example

Order:

`Cheese + Meat`

Base ingredient score:

`10 + 30 = 40`

Order completed after:

`14 seconds`

Final score:

`40 - 14 = 26`

Therefore:

`Order Score = +26`

Order scores can also become negative if the player takes too long to complete an order.

The game maintains:

- **Current Score**
- **High Score**

The high score is stored using Unity's **PlayerPrefs**, allowing it to persist between game sessions.

---

## 🏗️ Project Architecture

The project uses a **component-based architecture**, with major gameplay responsibilities separated into dedicated C# scripts.

### GameManager

Responsible for global game state, including:

- 3-minute game timer
- Current score
- Persistent high score
- Start / Controls screen
- Game Over state
- Pause / Resume
- Restart
- Quit

### ChefMove

Responsible for:

- Player movement
- Keyboard / gamepad input
- Character rotation
- Rigidbody-based movement

### ChefInventory

Responsible for:

- Tracking the currently held item
- Picking up ingredients
- Moving existing objects into the chef's hand
- Removing held ingredients
- Returning objects to kitchen positions

The chef can hold only one item at a time.

### FridgeInteraction

Responsible for:

- Detecting the chef near the refrigerator
- Displaying ingredient-selection UI
- Providing Meat, Vegetable, and Cheese
- Preventing pickup when the chef already holds an item

### TableInteract

Responsible for:

- Detecting raw vegetables
- Moving vegetables to the chopping area
- Handling the chopping timer
- Managing the knife during preparation
- Returning the prepared vegetable to the player

### StoveInteract

Responsible for:

- Detecting raw meat
- Managing two cooking positions
- Running independent cooking timers
- Converting raw meat into cooked meat
- Preventing unfinished meat from being collected
- Allowing cooked meat to be collected

### CustomerWindow

Responsible for:

- Random order generation
- Required ingredient tracking
- Ingredient icons
- Order timer
- Ingredient validation
- Duplicate ingredient requirements
- Order completion
- Score calculation
- 5-second order respawn

The customer window is designed as a reusable component/prefab so that all four windows can run their orders independently.

### TrashInteraction

Responsible for:

- Detecting the chef
- Checking the currently held item
- Removing unwanted ingredients

### Overall Architecture

The main gameplay flow is:

`GameManager`
↓  
`Chef Movement + Inventory`
↓  
`Kitchen Stations`
↓  
`Ingredient Preparation`
↓  
`Customer Windows`
↓  
`Order Completion`
↓  
`Score`
↓  
`New Order`

Separating these responsibilities keeps the gameplay systems easier to maintain, debug, and extend.

---

## 🛠️ Technologies

The project was developed using:

- **Unity**
- **C#**
- **Unity Input System**
- **Unity UI**
- **TextMeshPro**
- **Unity Physics / Rigidbody**
- **Coroutines**
- **PlayerPrefs**
- **Git**
- **GitHub**

### Unity Concepts Used

The project demonstrates practical use of:

- GameObjects and Components
- Prefabs
- Colliders and Trigger Colliders
- Rigidbody movement
- Serialized Inspector references
- Coroutines
- Enums
- UI Buttons
- World-space UI
- Screen-space UI
- TextMeshPro
- Randomized gameplay logic
- Persistent data
- Scene management
- Component-based gameplay architecture

---

## 🚀 How to Run

### Requirements

Install:

- **Unity Hub**
- The Unity Editor version specified by the project

### Clone the Repository

Clone the repository using Git:

`git clone <YOUR-REPOSITORY-URL>`

Alternatively, download the repository as a ZIP file from GitHub and extract it.

### Open in Unity

1. Open **Unity Hub**.
2. Select **Add → Add project from disk**.
3. Select the cloned/extracted project folder.
4. Allow Unity to import the project.
5. Open the main gameplay scene from the project's `Assets/Scenes` folder.
6. Press **Play**.

### Project Files

The repository contains the important Unity source directories:

`Assets/`

`Packages/`

`ProjectSettings/`

Unity-generated directories such as `Library`, `Temp`, `Logs`, and `UserSettings` are excluded from source control using `.gitignore`.

---

## 👨‍💻 Developer

**George V Joy**

Developed using **Unity and C#** as part of the **Tentworks Interactive Developer Test – Yes Chef!**