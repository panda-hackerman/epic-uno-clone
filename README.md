# Une - Cross-Platform Uno Clone

## 📚 Table of Contents

- [Features](#-features)
- [How to Build](#-how-to-build)
  - [Server](#-how-to-server)
  - [Client](#-how-to-client)
- [Known Issues](#%EF%B8%8F-known-issues)
- [Credits](#-credits)
- [License](#-license)

## 🎮 Features

A cross-platform multiplayer Uno clone for Windows and MacOSX. 

If you know the traditional [Uno](https://en.wikipedia.org/wiki/Uno_(card_game)) rules then you already know how to play. There are only a few changes, namely there's no need to declare "Uno" (although you're more than welcome to enforce this rule manually when you play with friends), and you start with 8 cards instead of 7.

Matches use a lobby-based system where the "host" is given a 6-digit ID, and other players can join based on that code. The host isn't the server, any client can be the host of a lobby. You can start a game with any number of players between one (though it's a bit lonely) and theoretically infinity but practically around sixty four.
When the host starts the match, the game starts for everyone and ends when one player gets rid of all their cards (or everyone leaves the lobby). The host and the players can choose to remain in the same lobby for another game, or leave. If the host leaves during the match, the new host is whoever joined most recently.

To learn more about the development of this game, see [this article](https://michaud.dev/projects/une).

| Lobby     | In game   |
| :-------: | :-------: |
| ![Une lobby](https://github.com/user-attachments/assets/4d2dede0-217a-40bb-8ccd-6138a5498d0c) | ![Une gameplay](https://github.com/user-attachments/assets/b06ffe23-0c44-4d59-b4bc-a3b7aa16f5d7) |

## 🚀 How to Build

You can build the game for both MacOSX using Unity, just make sure you have the right build target.

### 📻 How to Server

The server is responsible for managing every lobby that's currently open and syncing the state between clients. In this repo the server address is `localhost` by default, but if you don't want to play with yourself, you'll have to either:
1. Let your friends connect to your computer, or
2. Host an instance of the game on a cloud service (Microsoft Azure, Google Cloud, etc.)

It's worth noting that we used a Windows build for the server. Running the server on MacOS *should* work, but it's untested.

> [!IMPORTANT]
> Make sure "Server Build" is checked. This creates a headless build and lets the game know to run the server scripts.
>
> ![image](https://github.com/user-attachments/assets/ea65958c-c924-4727-898e-749f6c6a10b1)

### 👂 How to Client

Before you build the client, make sure you set the correct IP address for the server you want to connect to. To do so:
1. In the "Offline" scene (Assets/Scenes/Offline), locate the NetworkManager object
2. In the "Une Network Manager" script locate the `Network Address` field.
3. Set the Network Address to the desired IP address.
4. Build the client and distribute to your many friends.

## 🕷️ Known Issues

- If the host of a lobby leaves during the results screen, the button to continue won't appear for the new host, forcing the remaining players to create a new lobby to continue the game.
- In the "host local server" option is chosen from the offline debug options, the game will appear to also be a valid client—but treating it as such causes unexpected behavior (e.g., the lobby screen will show all players that are connected to the server, even if they haven't joined a game or are in a different lobby).

## 🌟 Credits

Lead Programmers:
- Eli Michaud ([GitHub](https://github.com/panda-hackerman) | [Portfolio](https://michaud.dev/))
- Fabian Hernandez-Angel ([GitHub](https://github.com/SomeGreenDude))

Music:
[Audionautix.com](https://audionautix.com/)

## 📜 License

This project is licensed under the Unlicense (See [LICENSE](LICENSE)). Do whatever you want, it's yours.

This project uses third-party libraries:
- [Mirror Networking](https://github.com/MirrorNetworking/Mirror/tree/master) ([MIT License](https://github.com/MirrorNetworking/Mirror/blob/master/LICENSE))
- [Unity Standalone File Browser](https://github.com/gkngkc/UnityStandaloneFileBrowser) ([MIT License](https://github.com/gkngkc/UnityStandaloneFileBrowser/blob/master/LICENSE.txt))
- [ParrelSync](https://github.com/VeriorPies/ParrelSync) ([MIT License](https://github.com/VeriorPies/ParrelSync/blob/master/LICENSE.md))
