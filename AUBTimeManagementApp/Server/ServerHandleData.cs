﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server {
    /// <summary>
    /// Here the server parse the received message and initializes the appropriate handler
    /// </summary>
    class ServerHandleData {
        public delegate void PacketF(int ConnectionID, byte[] Data);    //Signature of functions of type (int, byte[])
        public static Dictionary<int, PacketF> PacketListener;          //Maps functionID -> function; read-only hence thread-safe

        /// <summary>
        /// Maps integers to corresponding functions
        /// </summary>
        public static void InitializePacketListener() {
            PacketListener = new Dictionary<int, PacketF> {
                { (int)ClientPackages.CMsg, HandleMessage },
                { (int)ClientPackages.CLogin, HandleLogin }
            };
        }

        /// <summary>
        /// Makes sure a newly connected user is a valid user of our application
        /// </summary>
        /// <param name="ConnectionID"></param>
        /// <returns></returns>
        public static bool HandleAuth(int ConnectionID) {
            if (ServerTCP.ClientObjects[ConnectionID].buffer.Length() < 12) {
                ServerTCP.ClientObjects[ConnectionID].CloseConnection();
                return false;
            }

            int len = ServerTCP.ClientObjects[ConnectionID].buffer.ReadInteger();
            int id1 = ServerTCP.ClientObjects[ConnectionID].buffer.ReadInteger();
            int id2 = ServerTCP.ClientObjects[ConnectionID].buffer.ReadInteger();
            if (len == 8 && id1 == 19239485 && id2 == 5680973) {
                ServerTCP.ClientObjects[ConnectionID].authenticated = true;
                Console.WriteLine(ConnectionID + " was successfully authenticated");
                return true;
            }
            else {
                ServerTCP.ClientObjects[ConnectionID].CloseConnection();
                Console.WriteLine(ConnectionID + " is fake");
                return false;
            }
        }

        /// <summary>
        /// Makes sure that we process every byte of every packet received
        /// </summary>
        /// <param name="ConnectionID"></param>
        /// <param name="data"></param>
        /// In case of packets > 4096 bytes, HandleData will be called mutliple times
        /// and only when all the data is here (length>=plength), the loop will be entered
        public static void HandleData(int ConnectionID, byte[] data)        //Static method is fine since each thread has its own stack 
        {
            try {
                //Writing on the console what is received (for debugging)
                /*
                foreach (byte bb in data) { Console.Write(bb + " "); }
                Console.Write('\n');
                foreach (byte bb in data) { Console.Write((char)bb); }
                Console.Write('\n');
                */

                if (data == null) { Console.WriteLine("No data..."); return; }

                int pLength = 0;    //Packet length
                byte[] buffer = (byte[])data.Clone();   //To avoid shallow copies

                if (!ServerTCP.ClientObjects.ContainsKey(ConnectionID)) { return; }
                if (ServerTCP.ClientObjects[ConnectionID].buffer == null)
                    ServerTCP.ClientObjects[ConnectionID].buffer = new ByteBuffer();

                ServerTCP.ClientObjects[ConnectionID].buffer.WriteBytes(buffer);
                if (!ServerTCP.ClientObjects[ConnectionID].authenticated) {
                    bool a = HandleAuth(ConnectionID);
                    if (!a) { return; }
                    if (ServerTCP.ClientObjects[ConnectionID].buffer.Length() == 0) { return; }
                }

                if (ServerTCP.ClientObjects[ConnectionID].buffer.Length() < 4) {
                    Console.WriteLine("Buffer is too empty");
                    ServerTCP.ClientObjects[ConnectionID].buffer.Clear();
                    return;
                }

                if (!ServerTCP.ClientObjects.ContainsKey(ConnectionID)) { return; }
                pLength = ServerTCP.ClientObjects[ConnectionID].buffer.ReadInteger(false);  //Advances only when the whole packet is here
                while (pLength >= 4 && pLength <= ServerTCP.ClientObjects[ConnectionID].buffer.Length() - 4)    //-4 since readPos hasn't advanced yet
                {
                    ServerTCP.ClientObjects[ConnectionID].buffer.ReadInteger();

                    int packageID = ServerTCP.ClientObjects[ConnectionID].buffer.ReadInteger();
                    pLength -= 4;
                    data = ServerTCP.ClientObjects[ConnectionID].buffer.ReadBytes(pLength);

                    //Call the appropriate function in case of a correct packageID
                    if (PacketListener.TryGetValue(packageID, out PacketF packet))
                        packet.Invoke(ConnectionID, data);
                    else { Console.WriteLine("Wrong function ID"); pLength = 0; break; }

                    if (!ServerTCP.ClientObjects.ContainsKey(ConnectionID)) { return; }

                    pLength = 0;
                    if (ServerTCP.ClientObjects[ConnectionID].buffer.Length() >= 4)
                        pLength = ServerTCP.ClientObjects[ConnectionID].buffer.ReadInteger(false);
                }

                if (pLength < 4 && ServerTCP.ClientObjects.ContainsKey(ConnectionID)) { ServerTCP.ClientObjects[ConnectionID].buffer.Clear(); }
            }
            catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
        }


        private static void HandleMessage(int ConnectionID, byte[] data) {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            string msg = buffer.ReadString();
            Console.WriteLine(msg);
            buffer.Dispose();
        }
        private static void HandleLogin(int ConnectionID, byte[] data) {
            ByteBuffer buffer = new ByteBuffer();
            buffer.WriteBytes(data);
            string username = buffer.ReadString();

            buffer.Dispose();
        }
    }
}