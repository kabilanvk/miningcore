using System;
using System.Collections.Generic;
using System.Text;

namespace Miningcore.Api.WebSocketNotifications
{
    public enum WsNotificationType
    {
        Greeting,
        BlockFound,
        NewChainHeight,
        NetworkBlock,
        Payment,
        BlockUnlocked,
        BlockUnlockProgress,
        HashrateUpdated
    }
}
