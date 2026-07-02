using System.Runtime.InteropServices;
using Dalamud.Game.Command;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Hooking;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

namespace Dalamud.FullscreenCutscenes
{
    public sealed class Plugin : IDalamudPlugin
    {
        private const string CommandName = "/pcutscenes";

        private delegate nint UpdateLetterboxingDelegate(nint thisPtr);

        [PluginService] private static IDalamudPluginInterface PluginInterface { get; set; } = null!;
        [PluginService] private static ICommandManager CommandManager { get; set; } = null!;
        [PluginService] private static ISigScanner SigScanner { get; set; } = null!;
        [PluginService] private static IGameInteropProvider GameInteropProvider { get; set; } = null!;
        [PluginService] private static ICondition Condition { get; set; } = null!;

        private Hook<UpdateLetterboxingDelegate>? updateLetterboxingHook;

        private Configuration Configuration { get; init; }

        public Plugin()
        {
            this.Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            this.Configuration.Initialize(PluginInterface);

            CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
            {
                HelpMessage = "Toggle ultrawide cutscene letterboxing."
            });

            if (SigScanner.TryScanText("E8 ?? ?? ?? ?? 48 8B 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? 48 8B 8B ?? ?? ?? ??", out var ptr))
            {
                this.updateLetterboxingHook = GameInteropProvider.HookFromAddress<UpdateLetterboxingDelegate>(ptr, UpdateLetterboxingDetour);
                this.updateLetterboxingHook.Enable();
            }
        }

        private unsafe nint UpdateLetterboxingDetour(nint thisptr)
        {
            bool isWatchingCutscene = Condition[ConditionFlag.OccupiedInCutSceneEvent] ||
                                      Condition[ConditionFlag.WatchingCutscene];
            if (this.Configuration.IsEnabled && isWatchingCutscene)
            {
                SomeConfig* config = (SomeConfig*)thisptr;
                config->ShouldLetterBox &= ~(1 << 5);
            }

            return this.updateLetterboxingHook!.Original(thisptr);
        }

        public void Dispose()
        {
            this.updateLetterboxingHook?.Disable();
            CommandManager.RemoveHandler(CommandName);
        }

        private void OnCommand(string command, string args)
        {
            if (!string.IsNullOrWhiteSpace(args) && bool.TryParse(args, out var val))
            {
                this.Configuration.IsEnabled = val;
            }
            else
            {
                this.Configuration.IsEnabled = !this.Configuration.IsEnabled;
            }

            this.Configuration.Save();
        }

        [StructLayout(LayoutKind.Explicit)]
        public partial struct SomeConfig
        {
            [FieldOffset(0x40)] public int ShouldLetterBox;
        }
    }
}
