using System.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;
using Swordfish.Networking.Attributes;
using Swordfish.Networking.Interfaces;

namespace Swordfish.Networking
{
    public class PacketManager
    {
        private static PacketManager s_Instance;
        private static PacketManager Instance => s_Instance ?? (s_Instance = Initialize());

        private Dictionary<int, List<MethodInfo>> Handlers;
        private Dictionary<Type, int> PacketIDs;
        private Dictionary<int, Type> PacketTypes;

        public static PacketManager Initialize()
        {
            if (s_Instance != null)
            {
                Console.WriteLine("Tried to re-initialize PacketManager while an instance already exists.");
                return s_Instance;
            }

            s_Instance = new PacketManager();
            RegisterPackets();
            RegisterHandlers();
            return s_Instance;
        }

        private static void RegisterHandlers()
        {
            Console.WriteLine("Registering packet handlers...");
            Dictionary<int, List<MethodInfo>> handlers = new Dictionary<int, List<MethodInfo>>();

            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            foreach (MethodInfo method in type.GetMethods())
            {
                PacketHandlerAttribute packetHandlerAttribute = method.GetCustomAttribute<PacketHandlerAttribute>();
                if (packetHandlerAttribute != null)
                {
                    if (IsValidHandlerParameters(method.GetParameters()))
                    {
                        if (handlers.TryGetValue(GetPacketId(packetHandlerAttribute.PacketType), out List<MethodInfo> packetHandlers))
                            packetHandlers.Add(method);
                        else
                            handlers.Add(GetPacketId(packetHandlerAttribute.PacketType), new List<MethodInfo> { method });
                        
                        Console.WriteLine($"Registered '{type.Name}.{method.Name} : {packetHandlerAttribute.PacketType.Name}'");
                    }
                    else
                    {
                        Console.WriteLine($"Ignored '{type}.{method.Name}' decorated as a PacketHandler with invalid signature.");
                    }
                }
            }

            Instance.Handlers = handlers;
        }

        private static bool IsValidHandlerParameters(ParameterInfo[] parameters)
        {
            return parameters.Length == 2 && typeof(IPacket).IsAssignableFrom(parameters[0].ParameterType) && parameters[1].ParameterType == typeof(ClientConnection);
        }

        private static void RegisterPackets()
        {
            Console.WriteLine("Registering packets...");
            Dictionary<Type, int> packetIDs = new Dictionary<Type, int>();
            Dictionary<int, Type> packetTypes = new Dictionary<int, Type>();

            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                PacketAttribute packetAttribute = type.GetCustomAttribute<PacketAttribute>();
                if (packetAttribute != null)
                {
                    if (typeof(IPacket).IsAssignableFrom(type))
                    {
                        int id = packetIDs.Count;
                        packetIDs.Add(type, id);
                        packetTypes.Add(id, type);
                        Console.WriteLine($"Registered '{id} : {type}'");
                    }
                    else
                    {
                        Console.WriteLine($"Ignored '{type}' decorated as a packet but does not implement IPacket");
                    }
                }
            }

            Instance.PacketIDs = packetIDs;
            Instance.PacketTypes = packetTypes;
        }

        public static int GetPacketId<T>() => GetPacketId(typeof(T));

        public static int GetPacketId(object packet) => GetPacketId(packet.GetType());

        public static int GetPacketId(Type type) => Instance.PacketIDs[type];

        public static Type GetPacketType(int id) => Instance.PacketTypes[id];

        public static void Process(Packet packet, ClientConnection client)
        {
            int packetID = packet.ResetReader().ReadInt();
            Type packetType = GetPacketType(packetID);

            //  Deserialize the packet
            object deserializedPacket = packet.Deserialize(packetType);

            //  Invoke the packet's handlers
            if (Instance.Handlers?.TryGetValue(packetID, out List<MethodInfo> handlers) ?? false)
            {
                foreach (MethodInfo handler in handlers)
                    handler.Invoke(null, new object[] { deserializedPacket, client });
            }
        }
    }
}
