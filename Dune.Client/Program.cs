using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Steamworks;

namespace Dune.Client
{
    class Program
    {
        private const string CredentialsFile = "credentials.txt";

        private static ulong? _steamId;

        private static byte[] _ticket;

        private static string _serverUrl = "dune.dragonsdogma.com";

        private static ushort _serverPort = 12501;

        static void Main()
        {
            if (!ReadLogin())
            {
                Login().Wait();
                SaveLogin();
            }

            if (_steamId == null || _ticket == null)
            {
                Log("Error: steam login data missing");
                return;
            }

            PollUrDragonStatus(_serverUrl, _serverPort, _steamId.Value, _ticket);
        }

        static void Log(string message)
        {
            Console.WriteLine(message);
        }

        private static bool ReadLogin()
        {
            if (File.Exists(CredentialsFile))
            {
                string content = File.ReadAllText(CredentialsFile);
                string[] split = content.Split(';');
                if (split.Length == 2)
                {
                    try
                    {
                        _steamId = ulong.Parse(split[0]);
                        _ticket = Convert.FromBase64String(split[1]);
                        return true;
                    }
                    catch (FormatException)
                    {
                        return false;
                    }
                }
            }

            return false;
        }

        private static void SaveLogin()
        {
            if (_steamId != null && _ticket != null)
            {
                string content = _steamId + ";" + Convert.ToBase64String(_ticket);
                File.WriteAllText(CredentialsFile, content);
            }
        }

        private static async Task Login()
        {
            ////var appId = new AppId_t(367500);
            ////if (SteamAPI.RestartAppIfNecessary(appId))
            ////{
            ////    Debug.WriteLine("Could not init steam");
            ////    return;
            ////}

            if (!SteamAPI.Init())
            {
                Debug.WriteLine("Could not initialize steam");
                return;
            }

            SteamClient.SetWarningMessageHook((severity, text) => Console.WriteLine(text));

            if (!SteamUser.BLoggedOn())
            {
                Debug.WriteLine("Steam user not logged in");
                return;
            }

            CSteamID userSteamId = SteamUser.GetSteamID();
            ////if (SteamApps.BIsSubscribed())
            ////{
            ////    Debug.WriteLine("Steam user has no license");
            ////    return;
            ////}

            ////if (SteamApps.BIsSubscribedApp(appId))
            ////{
            ////    Debug.WriteLine("Steam user has no license");
            ////    return;
            ////}

            var cts = new CancellationTokenSource(10000);
            bool receivedTicket = false;
            byte[] rgubTicket = new byte[1024];
            SteamAPICall_t handle = SteamUser.RequestEncryptedAppTicket(null, 0);
            var callback = CallResult<EncryptedAppTicketResponse_t>.Create((response, bIoFailure) =>
            {
                if (response.m_eResult == EResult.k_EResultOK)
                {
                    uint ticketLen;
                    SteamUser.GetEncryptedAppTicket(rgubTicket, 1024, out ticketLen);
                    Array.Resize(ref rgubTicket, (int)ticketLen);
                    receivedTicket = true;
                }
                else
                {
                    cts.Cancel();
                }
            });
            callback.Set(handle);

            while (!receivedTicket && !cts.Token.IsCancellationRequested)
            {
                SteamAPI.RunCallbacks();
                await Task.Delay(1000);
            }

            if (receivedTicket)
            {
                _steamId = userSteamId.m_SteamID;
                _ticket = rgubTicket;
            }

            SteamAPI.Shutdown();
        }

        private static void PollUrDragonStatus(string duneDragonsdogmaCom, ushort serverPort, ulong steamId, byte[] ticket)
        {
            while (true)
            {
                try
                {
                    PrintUrDragonStatus(duneDragonsdogmaCom, serverPort, steamId, ticket);
                }
                catch (Exception e)
                {
                    Log("Error: " + e);
                }

                Thread.Sleep(60 * 1000);
            }
        }

        private static void PrintUrDragonStatus(string serverUrl, ushort serverPort, ulong steamId, byte[] ticket)
        {
            var client = new DuneClient(serverUrl, serverPort, steamId, ticket);
            client.Connect();

            string urDragonStatus = GetUrDragonStatus(client);
            Console.Clear();
            Console.Write(urDragonStatus);

            client.Disconnect();
        }

        private static string GetUrDragonStatus(DuneClient client)
        {
            OnlineUrDragon dragon = new OnlineUrDragon();
            dragon.Update(client.GetCommonArea(OnlineUrDragon.AllPropertyIndices));
            return dragon.ToText();
        }
    }
}
