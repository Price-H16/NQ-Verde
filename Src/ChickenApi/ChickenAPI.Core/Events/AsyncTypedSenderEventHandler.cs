// WingsEmu
// 
// Developed by NosWings Team

using System;
using System.Threading.Tasks;

namespace ChickenAPI.Core.Events
{
    public delegate Task AsyncTypedSenderEventHandler<in TSender, in TEventArgs>(TSender sender, TEventArgs e)
    where TSender : class
    where TEventArgs : EventArgs;
}