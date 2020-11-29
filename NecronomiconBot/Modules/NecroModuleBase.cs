using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace NecronomiconBot.Modules
{
    public class NecroModuleBase<T> : ModuleBase<T> where T: class, ICommandContext
    {

    }
}
