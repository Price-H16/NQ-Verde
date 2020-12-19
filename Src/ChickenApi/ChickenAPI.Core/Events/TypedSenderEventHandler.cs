// WingsEmu
// 
// Developed by NosWings Team

using System;

namespace ChickenAPI.Core.Events
{
    public delegate void TypedSenderEventHandler<in TSender, in TEventArgs>(TSender sender, TEventArgs e)
    where TSender : class
    where TEventArgs : EventArgs;
}