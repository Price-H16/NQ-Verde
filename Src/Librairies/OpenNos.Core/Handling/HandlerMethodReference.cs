using System;
using OpenNos.Domain;

namespace OpenNos.Core.Handling
{
    public class HandlerMethodReference
    {
        #region Instantiation

        public HandlerMethodReference(Action<object, object> handlerMethod, IPacketHandler parentHandler,
            PacketAttribute handlerMethodAttribute)
        {
            HandlerMethod = handlerMethod;
            ParentHandler = parentHandler;
            HandlerMethodAttribute = handlerMethodAttribute;
            Identification = HandlerMethodAttribute.Header;
            PassNonParseablePacket = false;
            Authorities = new[] { AuthorityType.User };
        }

        public HandlerMethodReference(Action<object, object> handlerMethod, IPacketHandler parentHandler,
            Type packetBaseParameterType)
        {
            HandlerMethod = handlerMethod;
            ParentHandler = parentHandler;
            PacketDefinitionParameterType = packetBaseParameterType;
            var headerAttribute = (PacketHeaderAttribute)Array.Find(
                PacketDefinitionParameterType.GetCustomAttributes(true),
                ca => ca.GetType().Equals(typeof(PacketHeaderAttribute)));
            Identification = headerAttribute?.Identification;
            PassNonParseablePacket = headerAttribute?.PassNonParseablePacket ?? false;
            Authorities = headerAttribute?.Authorities ?? new[] { AuthorityType.User };
            IsCharScreen = headerAttribute?.IsCharScreen ?? false;
            Amount = headerAttribute?.Amount ?? 1;
        }

        #endregion

        #region Properties

        public int Amount { get; }

        public AuthorityType[] Authorities { get; }

        public Action<object, object> HandlerMethod { get; }

        public PacketAttribute HandlerMethodAttribute { get; }

        /// <summary>
        ///     String identification of the Packet by Header
        /// </summary>
        public string[] Identification { get; }

        public bool IsCharScreen { get; }

        public Type PacketDefinitionParameterType { get; }

        public IPacketHandler ParentHandler { get; }

        public bool PassNonParseablePacket { get; }

        #endregion
    }
}