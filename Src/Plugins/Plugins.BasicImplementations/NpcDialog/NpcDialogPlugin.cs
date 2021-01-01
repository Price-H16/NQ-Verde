using Autofac;
using ChickenAPI.Plugins;
using OpenNos.Core;
using OpenNos.Core.Extensions;
using OpenNos.GameObject._NpcDialog;
using System;

namespace Plugins.BasicImplementations.NpcDialog
{
    public class NpcDialogPlugin : IGamePlugin
    {
        #region Members

        private readonly IContainer _container;

        private readonly INpcDialogHandlerContainer _handlers;

        #endregion

        #region Instantiation

        public NpcDialogPlugin(INpcDialogHandlerContainer handlers, IContainer container)
        {
            _handlers = handlers;
            _container = container;
        }

        #endregion

        #region Properties

        public PluginEnableTime EnableTime => PluginEnableTime.PreContainerBuild;

        public string Name => nameof(NpcDialogPlugin);

        #endregion

        #region Methods

        public void OnDisable()
        {
        }

        public void OnEnable()
        {
            Logger.Log.InfoFormat("Loading NpcDialogs...");
            foreach (var handlerType in typeof(NpcDialogPlugin).Assembly
                .GetTypesImplementingInterface<INpcDialogAsyncHandler>())
                try
                {
                    var tmp = _container.Resolve(handlerType);
                    if (!(tmp is INpcDialogAsyncHandler real)) continue;

                    Logger.Log.Debug($"[NPC_DIALOG][ADD_HANDLER] {handlerType}");
                    _handlers.RegisterAsync(real).ConfigureAwait(false).GetAwaiter().GetResult();
                }
                catch (Exception e)
                {
                    Logger.Log.Error("[NPC_DIALOG][FAIL_ADD]", e);
                }
            Logger.Log.InfoFormat("Npc Dialogs initialized");
        }

        public void OnLoad()
        {
        }

        #endregion
    }
}