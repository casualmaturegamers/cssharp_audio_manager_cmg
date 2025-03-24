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

        public override void Load(bool hotReload)
        {
            // Register the chat command !audiomanager
            AddCommand("css_audiomanager", "Opens the audio manager menu", OnAudioManagerCommand);
            // Register menu selection commands
            AddCommand("css_1", "Menu option 1", OnMenuSelection);
            AddCommand("css_2", "Menu option 2", OnMenuSelection);
            AddCommand("css_3", "Menu option 3", OnMenuSelection);
            AddCommand("css_9", "Exit menu", OnMenuSelection);
        }

        private void OnAudioManagerCommand(CCSPlayerController? player, CommandInfo command)
        {
            if (player == null || !player.IsValid) return;

            // Display the menu to the player
            player.PrintToChat(" \x0BAudio Manager Menu:");
            player.PrintToChat(" \x0A!1 - Play Hello Sound");
            player.PrintToChat(" \x0A!2 - Play Goodbye Sound");
            player.PrintToChat(" \x0A!3 - Stop All Sounds");
            player.PrintToChat(" \x0A!9 - Exit Menu");
            player.PrintToChat("Type the command to select an option!");
        }

        private void OnMenuSelection(CCSPlayerController? player, CommandInfo command)
        {
            if (player == null || !player.IsValid) return;

            string commandName = command.GetCommandString;

            switch (commandName)
            {
                case "css_1":
                    player.PrintToChat(" \x06[Audio Manager] Playing Hello World sound!");
                    // pass
                    break;

                case "css_2":
                    player.PrintToChat(" \x06[Audio Manager] Playing Goodbye sound!");
                    // pass
                    break;

                case "css_3":
                    player.PrintToChat(" \x06[Audio Manager] Stopping all sounds!");
                    // pass
                    break;

                case "css_9":
                    player.PrintToChat(" \x06[Audio Manager] Menu closed!");
                    break;

                default:
                    player.PrintToChat(" \x04[Audio Manager] Invalid selection!");
                    break;
            }
        }
    }
}