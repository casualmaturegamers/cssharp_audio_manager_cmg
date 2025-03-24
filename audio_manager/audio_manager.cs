using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;

namespace audio_manager
{
    public class AudioManagerPlugin : BasePlugin
    {
        public override string ModuleName => "Audio Manager";
        public override string ModuleVersion => "1.0.0";
        public override string ModuleAuthor => "Andrew Mathews";
        public override string ModuleDescription => "Allows the player to control in game audio levels.";

        private enum MenuState
        {
            None,
            MainMenu,
            InGameVolume,
            PlayerVoiceVolume
        }

        private Dictionary<ulong, MenuState> _playerMenuStates = new Dictionary<ulong, MenuState>();
        private Dictionary<ulong, float> _playerVoiceVolumes = new Dictionary<ulong, float>();

        public override void Load(bool hotReload)
        {
            AddCommand("css_audiomanager", "Opens the audio manager menu", OnAudioManagerCommand);
            AddCommand("css_am", "Shortcut to open the audio manager menu", OnAudioManagerCommand);
            AddCommand("css_1", "Menu option 1", OnMenuSelection);
            AddCommand("css_2", "Menu option 2", OnMenuSelection);
            AddCommand("css_3", "Menu option 3", OnMenuSelection);
            AddCommand("css_4", "Menu option 4", OnMenuSelection);
            AddCommand("css_9", "Exit or back", OnMenuSelection);
        }

        private void OnAudioManagerCommand(CCSPlayerController? player, CommandInfo command)
        {
            if (player == null || !player.IsValid || player.Connected != PlayerConnectedState.PlayerConnected)
            {
                return;
            }

            _playerMenuStates[player.SteamID] = MenuState.MainMenu;
            ShowMainMenu(player);
        }

        private void ShowMainMenu(CCSPlayerController player)
        {
            player.PrintToChat(" \x0BAudio Manager Menu:");
            player.PrintToChat(" \x0A!1 - In-Game Volume (excludes voice chat)");
            player.PrintToChat(" \x0A!2 - Player Voice Volume");
            player.PrintToChat(" \x0A!9 - Exit");
            player.PrintToChat("Type the command to select an option!");
        }

        private void ShowVolumeMenu(CCSPlayerController player, string menuTitle)
        {
            player.PrintToChat($" \x0B{menuTitle}:");
            player.PrintToChat(" \x0A!1 - Volume 10%");
            player.PrintToChat(" \x0A!2 - Volume 50%");
            player.PrintToChat(" \x0A!3 - Volume 75%");
            player.PrintToChat(" \x0A!4 - Volume 100%");
            player.PrintToChat(" \x0A!9 - Back to Main Menu");
            player.PrintToChat("Type the command to select an option!");
        }

        private void OnMenuSelection(CCSPlayerController? player, CommandInfo command)
        {
            if (player == null || !player.IsValid || player.Connected != PlayerConnectedState.PlayerConnected)
            {
                return;
            }

            ulong steamId = player.SteamID;
            if (!_playerMenuStates.ContainsKey(steamId) || _playerMenuStates[steamId] == MenuState.None)
            {
                player.PrintToChat(" \x04[Audio Manager] Please open the menu first with !audiomanager or !am!");
                return;
            }

            string commandName = command.GetCommandString;
            MenuState currentState = _playerMenuStates[steamId];

            switch (currentState)
            {
                case MenuState.MainMenu:
                    HandleMainMenu(player, commandName);
                    break;
                case MenuState.InGameVolume:
                    HandleVolumeMenu(player, commandName, "In-Game Volume", SetInGameVolume);
                    break;
                case MenuState.PlayerVoiceVolume:
                    HandleVolumeMenu(player, commandName, "Player Voice Volume", SetPlayerVoiceVolume);
                    break;
            }
        }

        private void HandleMainMenu(CCSPlayerController player, string commandName)
        {
            ulong steamId = player.SteamID;
            switch (commandName)
            {
                case "css_1":
                    _playerMenuStates[steamId] = MenuState.InGameVolume;
                    ShowVolumeMenu(player, "In-Game Volume");
                    break;
                case "css_2":
                    _playerMenuStates[steamId] = MenuState.PlayerVoiceVolume;
                    ShowVolumeMenu(player, "Player Voice Volume");
                    break;
                case "css_9":
                    player.PrintToChat(" \x06[Audio Manager] Menu closed!");
                    _playerMenuStates[steamId] = MenuState.None;
                    break;
                default:
                    player.PrintToChat(" \x04[Audio Manager] Invalid selection! Use !1, !2, or !9.");
                    break;
            }
        }

        private void HandleVolumeMenu(CCSPlayerController player, string commandName, string menuTitle, Action<CCSPlayerController, float> setVolume)
        {
            ulong steamId = player.SteamID;
            switch (commandName)
            {
                case "css_1":
                    setVolume(player, 0.1f);
                    break;
                case "css_2":
                    setVolume(player, 0.5f);
                    break;
                case "css_3":
                    setVolume(player, 0.75f);
                    break;
                case "css_4":
                    setVolume(player, 1.0f);
                    break;
                case "css_9":
                    _playerMenuStates[steamId] = MenuState.MainMenu;
                    ShowMainMenu(player);
                    break;
                default:
                    player.PrintToChat(" \x04[Audio Manager] Invalid selection! Use !1, !2, !3, !4, or !9.");
                    break;
            }
        }

        private void SetInGameVolume(CCSPlayerController player, float volume)
        {
            ulong steamId = player.SteamID;
            float currentVoiceScale = _playerVoiceVolumes.ContainsKey(steamId) ? _playerVoiceVolumes[steamId] : 1.0f;

            Server.PrintToConsole($"Prompting {player.PlayerName} to set in-game volume to {volume}, preserving voice_scale {currentVoiceScale}");

            // Show the command in chat for the player to copy
            string command = $"volume {volume}; voice_scale {currentVoiceScale}";
            player.PrintToChat($" \x06[Audio Manager] Set In-Game Volume to {(volume * 100)}%:");
            player.PrintToChat($" \x0AOpen console (~) and paste: {command}");
            player.PrintToChat(" \x0APress Enter to apply.");
        }

        private void SetPlayerVoiceVolume(CCSPlayerController player, float volume)
        {
            player.ExecuteClientCommand($"voice_scale {volume}");
            _playerVoiceVolumes[player.SteamID] = volume;
            player.PrintToChat($" \x06[Audio Manager] Player Voice Volume set to {(volume * 100)}%!");
        }
    }
}