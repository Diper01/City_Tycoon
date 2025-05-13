# City_Tycoon
---

## 🎮 City Tycoon — System Overview

### 🕹️ Camera Controls

* **Pan:** Arrow keys or WASD
* **Drag:** Middle mouse button
* **Zoom:** Mouse scroll wheel
* **Rotate:** Q / E or right mouse + drag

> Controlled via `TycoonCameraController`
> **Cancel placement:** `X`
> **Rotate building:** `R`

---

### 🏗️ Building Placement

* `ZoneMapManager` handles valid build zones.
* Uses **Tilemap** to determine buildable cells.
* Checks area occupancy before placement.

> `BuildController` — previews the building before placement
> `BuildManager` — handles final placement

---

### 🛒 Shop System

* UI is generated dynamically from `Resources/Data/Buildings`
* Interactive **3D previews** of buildings with cost and income info.

> `BuildingShopManager` — manages the shop
> `ShopItem3D` — handles 3D preview logic

---

### 💾 Saving System

* `MoneyManager` tracks balance, earnings per second, expenses, and autosaves.
* `GameProgressManager` saves and restores buildings and player progress.

---

### 🧩 UI State Pattern

* `UIStateManager` switches between screens using state logic.
* `UpgradeMenuUI` shows level, income, and upgrade cost.

> Classes used: `IUIState`, `UIState`, `UIStateManager`

---

### 🏘️ Buildings & Upgrades

* `BuildingData` stores building metadata (cost, footprint, stats).
* `BuildingInstance` keeps track of current progress.
* `GameProgressManager` restores buildings upon loading.

---

### 🚗 AI Traffic System

* Uses **waypoint graphs** for car navigation.
* `WaypointNavigator` chooses the next route point.
* `ParkingBehavior` adds random stop behavior.
* `CarSpawner` spawns vehicles with `CarAI` logic.

> 🧠 Requires **NavMesh baking** for proper pathfinding.

---

### 🛠️ Building Composer (Editor Tools)

* `AssetSelectionPanel`: Load and create building data.
* `FootprintEditorPanel`: Configure footprint using Tilemap.
* `PreviewEditorPanel`: Preview models and set parameters.
* `UpgradesEditorPanel`: Define building upgrade levels.

> Related classes: `BuildingWindow`, `BuildingUtility`, `FootprintManager`

---
