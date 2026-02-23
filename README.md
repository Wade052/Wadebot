Wadebot
Wadebot is a custom Discord bot built using C# and DSharpPlus. It is designed to provide fun commands, automation, and a scalable leveling system backed by a SQLite database.

Features
•	Built with DSharpPlus
•	Modular command system
•	Automatic XP and leveling system per server
•	XP cooldown to prevent spam
•	SQLite database for persistent storage
•	Environment variable configuration
•	Auto-reconnect support
•	Actively developed and expandable
Technologies Used
•	C# (.NET)
•	DSharpPlus
•	SQLite
•	CommandsNext
•	Interactivity Extension
Setup Instructions
1. Clone the Wadebot repository.
2. Set the required environment variables:
   - DISCORD_TOKEN
   - BOT_PREFIX
3. Restore dependencies and run the project using dotnet.

Leveling System
Users automatically gain experience by sending messages. XP is tracked per user and per server. A cooldown system prevents XP farming.
Author
DeAndre Wade
Started: 10/16/2025
Current Version: 1.4.5
