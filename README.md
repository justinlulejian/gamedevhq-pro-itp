# Tower Defense Game for GameDevHQ Intensive Training Program

[![tower defense](https://markdown-videos.deta.dev/youtube/7oQGycp6c0I)](https://youtu.be/7oQGycp6c0I)


In 2021 I had an interest in game development and I took a course called the Intensive Training Program (ITP) by [GameDevHQ](https://gamedevhq.com/) while still working at Google. The ITP is an 8 week 20+/hour a week program where you develop a game concept from scratch (with art/UI assets provided) to get a breadth of understanding of developing game features in Unity. The concept of the game was a mech tower defense. 

Some of the features I developed were:

* A NavMesh for the mechs to path in-between the rocks without having to manually code the movement logic
* A health system for the enemies that was visible with a healthbar
* A spawn system that pooled enemy game objects for efficiency and utilized easily modifiable ScriptableObjects to define the waves
* A delegate system for broadcasting events between mechs and manager objects for elegantly cleaning up spawns and updating the UI and monetary system
* A custom debug Unity editor window that allows easier debugging of common game issues (give extra money, slow down gameplay, reset tower placement, etc.)
* Camera movement and zoom 
* Tower placement and particle effects to indicate placement location
* Tower tracking of enemies and damage logic/effects
* Customizing a dissolve shader for enemy destruction
* A purchase/upgrade/dismantle system (and UI logic) for buying new towers, upgrading them to more powerful types, and selling them to recover cash (war funds)
* A UI system for pausing/playing/and fast-forwarding the game
* A mock Paypal-based microtransaction system for purchasing more money (war funds) in the game
